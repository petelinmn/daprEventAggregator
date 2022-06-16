using System.Collections.Generic;
using DaprEventAggregatorPlugin;

namespace EventAggregator.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Common;
    using Dapr.Client;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;


    /// <summary>
    /// Sample showing Dapr integration with controller.
    /// </summary>
    [ApiController]
    public class AggregatorController : ControllerBase
    {
        /// <summary>
        /// AggregatorController Constructor with logger injection
        /// </summary>
        /// <param name="logger"></param>
        public AggregatorController(ILogger<AggregatorController> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// State store name.
        /// </summary>
        public const string StoreName = "statestore";
        private readonly ILogger<AggregatorController> logger;
        async Task<List<Event>> UsingEvents(EventRequest request, Func<List<Event>, Task<List<Event>>> func)
        {
            const string eventsKey = "aggregatorEventsList";
            var daprClient = new DaprClientBuilder().Build();

            var eventList = (await daprClient.GetStateAsync<List<Event>>(StoreName, eventsKey)) ?? new List<Event>();
            if (request != null)
            {
                Console.WriteLine($"Event received:\t {request.Name}\t {request.Arg}");
                var newEvent = new Event
                {
                    Name = request.Name,
                    Arg = request.Arg,
                    ContextId = request.ContextId,
                    Id = request.Id,
                    DateTime = request.DateTime,
                    Parents = request.Parents
                };

                eventList.Add(newEvent);

                Console.WriteLine($"Events count:\t {eventList.Count}");
                await daprClient.SaveStateAsync(StoreName, eventsKey, eventList);
            }

            return await func(eventList);
        }
 
        [Aggregator]
        [HttpPost("event")]
        public async Task Event(EventRequest eventRequest) =>
            await UsingEvents(eventRequest, async (events) =>
            {
                var configuration = await ConfigurationReader.Read();

                var dtNow = DateTime.Now;

                foreach (var pubsubElement in configuration.PubSubs)
                {
                    foreach (var complexEvent in pubsubElement.ComplexEvents)
                    {
                        if (!complexEvent.EventQueue.Any(e => e == eventRequest.Name))
                            continue;

                        var latestEvents = events
                            .Where(e => e.ContextId == eventRequest.ContextId)
                            .Where(e => dtNow - e.DateTime < complexEvent.Duration).ToList();
                        var latestRelatedEvents = latestEvents
                            .Where(e => complexEvent.EventQueue.Any(eq => eq == e.Name)).ToList();

                        if (complexEvent.EventsCount.HasValue)
                        {
                            if (complexEvent.EventsCount > latestRelatedEvents.Count(e => e.Name == eventRequest.Name))
                                continue;

                            latestRelatedEvents = latestRelatedEvents
                                .TakeLast(complexEvent.EventsCount.Value).ToList();
                        }

                        var latestRelatedEventsDistincted = new List<Event>();

                        for (var i = latestRelatedEvents.Count - 1; i >= 0; i--)
                        {
                            if (!latestRelatedEventsDistincted.Any(l => l.Name == latestRelatedEvents[i].Name))
                            {
                                latestRelatedEventsDistincted.Add(latestRelatedEvents[i]);
                            }
                        }

                        var allEventsWork = () => complexEvent.Mandatory == "All" &&
                            complexEvent.EventQueue.All(eName => latestRelatedEventsDistincted.Exists(e => e.Name == eName));
                        var atLeastOneEventWorks = () => complexEvent.Mandatory == "AtLeastOne" &&
                            latestRelatedEventsDistincted.Count > 0;

                        if (!string.IsNullOrEmpty(complexEvent.Mandatory) && !allEventsWork() && !atLeastOneEventWorks())
                            continue;

                        var dontPublishNewEvent = !JsExpression.Condition(complexEvent.Condition, "e",
                            latestRelatedEvents.Select(e => e.Arg).ToList());

                        if (dontPublishNewEvent)
                            continue;

                        var daprClient = new DaprClientBuilder().Build();
                        var val = JsonConvert.SerializeObject(latestRelatedEvents.Select(e => JsonConvert.DeserializeObject(e.Arg)).ToArray());
                        await daprClient.PublishEventAsync(pubsubElement.Name, complexEvent.Name, new EventRequest
                        {
                            Name = complexEvent.Name,
                            Arg = val,
                            ContextId = eventRequest.ContextId,
                            Id = Guid.NewGuid(),
                            Parents = latestRelatedEvents.Select(e => e.Id).ToArray()
                        }); 

                        Console.WriteLine($"Event published:\t {complexEvent.Name}, pubsub: {pubsubElement.Name}");
                    }
                }

                return events;
            });

        [HttpGet("event/{contextId}")]
        public async Task<List<Event>> GetEvents(Guid contextId) =>
            await UsingEvents(null, async (events) =>
                await Task.Run(() => events.Where(e => e.ContextId == contextId).ToList()));
    }
}

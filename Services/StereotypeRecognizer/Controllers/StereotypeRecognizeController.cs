using System.Collections.Generic;
using DaprEventAggregatorPlugin;

namespace StereotypeRecognizer.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Common;
    using Dapr.Actors;
    using Dapr.Actors.Client;
    using Dapr.Client;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using WorkerManager.Actors.Contract;


    /// <summary>
    /// Sample showing Dapr integration with controller.
    /// </summary>
    [ApiController]
    public class StereotypeRecognizeController : ControllerBase
    {
        /// <summary>
        /// AggregatorController Constructor with logger injection
        /// </summary>
        /// <param name="logger"></param>
        public StereotypeRecognizeController(ILogger<StereotypeRecognizeController> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// State store name.
        /// </summary>
        public const string StoreName = "statestore";
        private readonly ILogger<StereotypeRecognizeController> logger;
        async Task<List<Event>> UsingEvents(EventRequest request, Func<List<Event>, Task<List<Event>>> func)
        {
            const string eventsKey = "StereotypesEventsList";
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
        public async Task Event(EventRequest request) =>
            await UsingEvents(request, async (_) =>
            {
                try
                {
                    Console.WriteLine(request.Name);
                    var workerManagerActor = ActorProxy.Create<IWorkerManagerActor>(
                    new ActorId($"WorkerManagerActor_{request.Name}"), "WorkerManagerActor");

                    var stereotypeConfiguration = await ConfigurationReader.Read();
                    var stereotypesToCheck = stereotypeConfiguration
                        .PubSubs.SelectMany(i => i.Stereotypes
                        .Where(s => s.Events?.Any(e => e == request.Name) == true));
                    Console.WriteLine(JsonConvert.SerializeObject(stereotypesToCheck));
                    foreach (var stereotype in stereotypesToCheck)
                    {
                        Console.WriteLine(string.Join(", ", stereotype.Actions));
                        foreach (var action in stereotype.Actions)
                        {
                            await workerManagerActor.Register(new[] { action }, request.ContextId, request.Name);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                return _;
            });


        [HttpGet("event/{contextId}")]
        public async Task<List<Event>> GetEvents(Guid contextId) =>
            await UsingEvents(null, async (events) =>
                await Task.Run(() => events.Where(e => e.ContextId == contextId).ToList()));
    }
}

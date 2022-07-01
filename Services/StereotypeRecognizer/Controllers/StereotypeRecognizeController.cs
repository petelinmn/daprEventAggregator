using System.Collections.Generic;
using DaprEventAggregatorPlugin;

namespace StereotypeRecognizer.Controllers
{
    using System;
    using System.Drawing;
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

        async Task<List<Stereotype>> GetStereotypes()
        {
            const string eventsKey = "StereotypesEventsList";
            var daprClient = new DaprClientBuilder().Build();

            return (await daprClient.GetStateAsync<List<Stereotype>>(StoreName, eventsKey))
                ?? new List<Stereotype>();
        }

        /// <summary>
        /// State store name.
        /// </summary>
        public const string StoreName = "statestore";
        private readonly ILogger<StereotypeRecognizeController> logger;
        async Task<List<Stereotype>> UsingEvents(EventRequest request, Func<Stereotype, Task<List<Stereotype>>> func)
        {
            const string eventsKey = "StereotypesEventsList";
            var daprClient = new DaprClientBuilder().Build();

            var eventList = (await GetStereotypes());
            if (request != null)
            {
                Console.WriteLine($"Stereotype received:\t {request.Name}\t {request.Arg}");
                var newEventTemplate = new Stereotype
                {
                    Name = request.Name,
                    Arg = request.Arg,
                    ContextId = request.ContextId,
                    Id = request.Id,
                    DateTime = request.DateTime,
                    Parents = request.Parents,
                };

                var newItems = await func(newEventTemplate);

                if (newItems?.Count > 0)
                    eventList.AddRange(newItems);

                Console.WriteLine($"Events count:\t {eventList.Count}");
                await daprClient.SaveStateAsync(StoreName, eventsKey, eventList);
            }

            return eventList;
        }
 
        [Aggregator]
        [HttpPost("stereotype")]
        public async Task Stereotype(EventRequest request) =>
            await UsingEvents(request, async (stereotypesTemplate) =>
            {
                var result = new List<Stereotype>();
                try
                {
                    var workerManagerActor = ActorProxy.Create<IWorkerManagerActor>(
                    new ActorId($"WorkerManagerActor_{request.Name}"), "WorkerManagerActor");

                    var stereotypeConfiguration = await ConfigurationReader.Read();
                    var stereotypesToCheck = stereotypeConfiguration
                        .PubSubs.SelectMany(i => i.Stereotypes
                        .Where(s => s.Events?.Any(e => e == request.Name) == true));

                    foreach (var stereotype in stereotypesToCheck)
                    {
                        var stereotypeToSave = (Stereotype)stereotypesTemplate.Clone();
                        stereotypeToSave.Name = stereotype.Name;
                        if (StereotypeCheck(request.Arg, stereotypeToSave, stereotype))
                        {
                            stereotypeToSave.IsConfirmed = true;
                            foreach (var action in stereotype.Actions)
                            {
                                await workerManagerActor.Register(action,
                                    new string[] { }, request.ContextId, new Guid[] { stereotypeToSave.Id });
                            }
                        }

                        Console.WriteLine($"Stereotype confirmed: {stereotypeToSave.IsConfirmed}");

                        result.Add(stereotypeToSave);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                return result;
            });

        private static bool StereotypeCheck(string arg, Stereotype stereotypeToSave, StereotypeRecognizer.Stereotype stereotype)
        {
            var obj = ArgDict.GetData(arg, new Type[]
            {
                new Dictionary<string, string>().GetType(),
                new List<Dictionary<string, string>>().GetType(),
                new List<List<Dictionary<string, string>>>().GetType(),
            });

            var argDict = obj as ArgDict;
            var listOfArgDict = obj as List<Dictionary<string, string>>;
            var listOfListOfArgDict = obj as List<List<Dictionary<string, string>>>;

            if (listOfListOfArgDict != null || listOfArgDict != null)
            {
                //var charts = listOfListOfArgDict != null
                //    ? ArgDict.FlattenToDictPoints(listOfListOfArgDict)
                //    : ArgDict.FlattenToDictPoints(listOfArgDict);
                var charts2 = listOfListOfArgDict != null
                    ? ArgDict.FlattenToDictPoints2(listOfListOfArgDict)
                    : ArgDict.FlattenToDictPoints2(listOfArgDict);

                Func<Dictionary<string, string>, Dictionary<string, List<PointF>>> getBounds2 = boundFuncs =>
                    boundFuncs == null
                        ? null
                        : charts2.ToDictionary(chart => chart.Key,
                            chart => !boundFuncs.ContainsKey(chart.Key)
                                ? null
                                : chart.Value.Where(prop => prop.Key == "Value")
                                    .FirstOrDefault().Value
                                        .Select((_,x) =>
                                            new PointF(x, NCalcExpression.Calculate(boundFuncs[chart.Key], x,
                                                chart.Value.Where(v => v.Key != "Name").ToDictionary(v => v.Key, v => v.Value.Skip(x).FirstOrDefault())))).ToList());

                stereotypeToSave.UpperBounds = getBounds2(stereotype.UpperBounds);
                stereotypeToSave.LowerBounds = getBounds2(stereotype.LowerBounds);

                var result = false;
                foreach (var chart in charts2)
                {
                    var accuracy = 1;
                    var hitCount = 0;

                    if (!string.IsNullOrWhiteSpace(stereotype.Accuracy))
                    {
                        var confAccuracy = stereotype.Accuracy;
                        var isPercent = confAccuracy.Contains('%');
                        if (isPercent)
                        {
                            confAccuracy = confAccuracy.Replace("%", "").Trim();
                        }
                        Console.WriteLine($"isPercent:{isPercent}");
                        Console.WriteLine(confAccuracy);
                        if (float.TryParse(confAccuracy, out var floatVal))
                        {
                            Console.WriteLine($"floatVal:{floatVal}");

                            if (isPercent)
                            {
                                accuracy = (int)(chart.Value["Value"].Count * floatVal / 100);
                            }
                            else
                            {
                                accuracy = (int)floatVal;
                            }
                        }
                    }

                    Console.WriteLine($"accuracy in conf:{stereotype.Accuracy}");

                    List<PointF> upperBound = stereotypeToSave.UpperBounds?.ContainsKey(chart.Key) == true
                        ? stereotypeToSave.UpperBounds[chart.Key] : null;
                    List<PointF> lowerBound = stereotypeToSave.LowerBounds?.ContainsKey(chart.Key) == true
                        ? stereotypeToSave.LowerBounds[chart.Key] : null;

                    if ((upperBound is null || upperBound.Count() == 0)
                        && (lowerBound is null || lowerBound.Count() == 0))
                    {
                        result = true;
                        continue;
                    }

                    for (var x = 0; x < chart.Value["Value"].Count; x++)
                    {
                        var y = chart.Value["Value"][x];

                        float? upperY = (upperBound is null || upperBound.Count() < x - 1)
                            ? null : upperBound[x].Y;

                        float? lowerY = (lowerBound is null || lowerBound.Count() < x - 1)
                            ? null : lowerBound[x].Y;

                        if (upperY.HasValue && lowerY.HasValue)
                        {
                            if (upperY > lowerY)
                            {
                                if (y < upperY && y > lowerY)
                                {
                                    hitCount++;
                                }
                            }
                            else
                            {
                                if (y < upperY || y > lowerY)
                                {
                                    hitCount++;
                                }
                            }
                        }
                        else if (upperY.HasValue)
                        {
                            if (y < upperY)
                            {
                                hitCount++;
                            }
                        }
                        else if (lowerY.HasValue)
                        {
                            if (y > lowerY)
                            {
                                hitCount++;
                            }
                        }
                    }

                    if (hitCount >= accuracy)
                    {
                        if (!stereotypeToSave.ConfirmedProperties.Contains(chart.Key))
                            stereotypeToSave.ConfirmedProperties.Add(chart.Key);

                        result = true;
                    }
                }

                return result;
            }

            return false;
        }

        [HttpGet("stereotype/{contextId}")]
        public async Task<List<Stereotype>> GetEvents(Guid contextId) =>
           (await GetStereotypes()).Where(e => e.ContextId == contextId).ToList();
    }
}

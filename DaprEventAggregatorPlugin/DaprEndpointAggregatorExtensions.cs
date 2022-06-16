using System.Text.Json;
using System.Text.Json.Serialization;
using Dapr;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace DaprEventAggregatorPlugin
{
    /// <summary>
    /// Contains extension methods for <see cref="IEndpointRouteBuilder" />.
    /// </summary>
    public static class DaprEndpointAggregatorExtensions
    {
        /// <summary>
        /// Maps an endpoint that will respond to requests to <c>/dapr/subscribe</c> from the
        /// Dapr runtime.
        /// </summary>
        /// <param name="endpoints">The <see cref="IEndpointRouteBuilder" />.</param>
        /// <returns>The <see cref="IEndpointConventionBuilder" />.</returns>
        public static async Task<IEndpointConventionBuilder> MapEASubscribeHandler(this IEndpointRouteBuilder endpoints,
            Func<Task<Dictionary<string, List<string>>>> getPubSubConfiguration)
        {
            return await CreateSubscribeEndPoint(endpoints, getPubSubConfiguration);
        }

        private static async Task<IEndpointConventionBuilder> CreateSubscribeEndPoint(IEndpointRouteBuilder endpoints,
            Func<Task<Dictionary<string, List<string>>>> getPubSubConfiguration,
            SubscribeOptions options = null)
        {
            if (endpoints is null)
            {
                throw new ArgumentNullException(nameof(endpoints));
            }

            var pubsubConfiguration = await getPubSubConfiguration();
            Console.WriteLine(pubsubConfiguration.Count);
            return endpoints.MapGet("dapr/subscribe", async context =>
            {
                Console.WriteLine("00000000000");
                var logger = context.RequestServices.GetService<ILoggerFactory>().CreateLogger("DaprTopicSubscription");
                var dataSource = context.RequestServices.GetRequiredService<EndpointDataSource>();
                var subscriptions = dataSource.Endpoints
                    .OfType<RouteEndpoint>()
                    .Where(e => e.Metadata.GetOrderedMetadata<ITopicMetadata>().Any(t => t.Name != null)) // only endpoints which have TopicAttribute with not null Name.
                    .SelectMany(e =>
                    {
                        Console.WriteLine(JsonConvert.SerializeObject(JsonConvert.SerializeObject(pubsubConfiguration)));
                        var topicMetadata = pubsubConfiguration
                            .Select(e => e.Value
                                .Select(atomicEvent => new TopicMetaData()
                                {
                                    PubsubName = e.Key,
                                    Name = atomicEvent
                                }))
                            .SelectMany(i => i)
                            .ToList();

                        Console.WriteLine(JsonConvert.SerializeObject(pubsubConfiguration.Select(e => e.Value)));

                        Console.WriteLine("-----1");
                        Console.WriteLine(JsonConvert.SerializeObject(topicMetadata));
                        Console.WriteLine("-----2");
                        var subs = new List<(string PubsubName, string Name, bool? EnableRawPayload, string Match, int Priority, RoutePattern RoutePattern)>();

                        for (int i = 0; i < topicMetadata.Count(); i++)
                        {
                            subs.Add((topicMetadata[i].PubsubName,
                                topicMetadata[i].Name,
                                (topicMetadata[i] as IRawTopicMetadata)?.EnableRawPayload,
                                topicMetadata[i].Match,
                                topicMetadata[i].Priority,
                                e.RoutePattern));
                        }

                        return subs;
                    })
                    .Distinct()
                    .GroupBy(e => new { e.PubsubName, e.Name })
                    .Select(e => e.OrderBy(e => e.Priority))
                    .Select(e =>
                    {
                        var first = e.First();
                        var rawPayload = e.Any(e => e.EnableRawPayload.GetValueOrDefault());
                        var rules = e.Where(e => !string.IsNullOrEmpty(e.Match)).ToList();
                        var defaultRoutes = e.Where(e => string.IsNullOrEmpty(e.Match)).Select(e => RoutePatternToString(e.RoutePattern)).ToList();
                        var defaultRoute = defaultRoutes.FirstOrDefault();

                        if (logger != null)
                        {
                            if (defaultRoutes.Count > 1)
                            {
                                logger.LogError("A default subscription to topic {name} on pubsub {pubsub} already exists.", first.Name, first.PubsubName);
                            }

                            var duplicatePriorities = rules.GroupBy(e => e.Priority)
                              .Where(g => g.Count() > 1)
                              .ToDictionary(x => x.Key, y => y.Count());

                            foreach (var entry in duplicatePriorities)
                            {
                                logger.LogError("A subscription to topic {name} on pubsub {pubsub} has duplicate priorities for {priority}: found {count} occurrences.", first.Name, first.PubsubName, entry.Key, entry.Value);
                            }
                        }

                        var subscription = new Subscription
                        {
                            Topic = first.Name,
                            PubsubName = first.PubsubName,
                            Metadata = rawPayload ? new Metadata
                            {
                                RawPayload = "true",
                            } : null,
                        };

                        // Use the V2 routing rules structure
                        if (rules.Count > 0)
                        {
                            subscription.Routes = new Routes
                            {
                                Rules = rules.Select(e => new Rule
                                {
                                    Match = e.Match,
                                    Path = RoutePatternToString(e.RoutePattern),
                                }).ToList(),
                                Default = defaultRoute,
                            };
                        }
                        // Use the V1 structure for backward compatibility.
                        else
                        {
                            subscription.Route = defaultRoute;
                        }

                        return subscription;
                    })
                    .OrderBy(e => (e.PubsubName, e.Topic));

                Console.WriteLine(JsonSerializer.Serialize(subscriptions,
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                    }));

                await context.Response.WriteAsync(JsonSerializer.Serialize(subscriptions,
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                    }));
            });
        }

        private static string RoutePatternToString(RoutePattern routePattern)
        {
            return string.Join("/", routePattern.PathSegments
                                    .Select(segment => string.Concat(segment.Parts.Cast<RoutePatternLiteralPart>()
                                    .Select(part => part.Content))));
        }

        class TopicMetaData : ITopicMetadata
        {
            public string Name { get; set; }

            public string PubsubName { get; set; }

            public string Match { get; set; }

            public int Priority { get; set; }
        }


        /// <summary>
        /// This class defines subscribe endpoint response
        /// </summary>
        class Subscription
        {
            /// <summary>
            /// Gets or sets the topic name.
            /// </summary>
            public string Topic { get; set; }

            /// <summary>
            /// Gets or sets the pubsub name
            /// </summary>
            public string PubsubName { get; set; }

            /// <summary>
            /// Gets or sets the route
            /// </summary>
            public string Route { get; set; }

            /// <summary>
            /// Gets or sets the routes
            /// </summary>
            public Routes Routes { get; set; }

            /// <summary>
            /// Gets or sets the metadata.
            /// </summary>
            public Metadata? Metadata { get; set; }
        }

        /// <summary>
        /// This class defines the metadata for subscribe endpoint.
        /// </summary>
        class Metadata
        {
            /// <summary>
            /// Gets or sets the raw payload
            /// </summary>
            public string RawPayload { get; set; }
        }

        class Routes
        {
            /// <summary>
            /// Gets or sets the default route
            /// </summary>
            public string Default { get; set; }

            /// <summary>
            /// Gets or sets the routing rules
            /// </summary>
            public List<Rule> Rules { get; set; }
        }

        class Rule
        {
            /// <summary>
            /// Gets or sets the CEL expression to match this route.
            /// </summary>
            public string Match { get; set; }

            /// <summary>
            /// Gets or sets the path of the route.
            /// </summary>
            public string Path { get; set; }
        }
    }
}
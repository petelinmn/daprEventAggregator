using Dapr;

namespace DaprEventAggregatorPlugin
{
    using System;

    /// <summary>
    /// AggregatorAttribute describes an endpoint as a subscriber to a topic.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class AggregatorAttribute : Attribute, ITopicMetadata, IRawTopicMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregatorAttribute" /> class.
        /// </summary>
        public AggregatorAttribute()
        {
            this.Name = "Aggregator";
        }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public string PubsubName { get; }

        /// <inheritdoc/>
        public bool? EnableRawPayload { get; }

        /// <inheritdoc/>
        public new string Match { get; }

        /// <inheritdoc/>
        public int Priority { get; }
    }
}

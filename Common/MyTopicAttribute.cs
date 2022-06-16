using Dapr;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Common;

/// <summary>
/// TopicAttribute describes an endpoint as a subscriber to a topic.
/// </summary>
[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public class MyTopicAttribute : TopicAttribute
{

    [Range(1, 100, ErrorMessage = "Price must be between $1 and $100")]
    public decimal Price { get; set; }
    /// <summary>
    /// Initializes a new instance of the <see cref="TopicAttribute" /> class.
    /// </summary>
    /// <param name="pubsubName">The name of the pubsub component to use.</param>
    /// <param name="name">The topic name.</param>
    public MyTopicAttribute(string pubsubName, string name) : base(pubsubName, name)
    {
        Console.WriteLine("MYTOPIC_ATTRIBUTE");

        //this.Name = name;
        //this.PubsubName = pubsubName;
    }
}


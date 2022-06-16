using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EventAggregator;

public class EventAggregatorConfiguration
{
    public List<PubSubConfiguration> PubSubs { get; set; }
}

public class PubSubConfiguration
{
    public string Name { get; set; }
    public List<ComplexEvent> ComplexEvents { get; set; }
}

public class ComplexEvent
{
    public string Name { get; set; }
    public List<string> EventQueue { get; set; }
    public int? EventsCount { get; set; }
    public List<string> Rules { get; set; }
    public List<string> Common { get; set; }
    public string Condition { get; set; }
    public string Mandatory { get; set; } = "All";
    public TimeSpan Duration { get; set; }
}

internal class ConfigurationReader
{
    public static async Task<EventAggregatorConfiguration> Read()
    {
        var configurationFile = "AggregatorConfiguration.json";
        if (!System.IO.File.Exists(configurationFile))
            throw new Exception("Configuration doesn't exist");

        var configurationRaw = await System.IO.File.ReadAllTextAsync(configurationFile);
        var configuration = JsonConvert.DeserializeObject<EventAggregatorConfiguration>(configurationRaw);

        return configuration;
    }
}
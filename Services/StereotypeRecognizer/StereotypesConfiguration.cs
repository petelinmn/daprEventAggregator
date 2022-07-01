using System;
using Common;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace StereotypeRecognizer;

public class StereotypesConfiguration
{
    public List<PubSubConfiguration> PubSubs { get; set; }
}

public class PubSubConfiguration
{
    public string Name { get; set; }
    public List<Stereotype> Stereotypes { get; set; }
}

public class Stereotype
{
    public string Name { get; set; }
    public string ArgumentName { get; set; }
    public string Condition { get; set; }
    public List<string> Events { get; set; }
    public Dictionary<string, string> UpperBounds { get; set; }
    public Dictionary<string, string> LowerBounds { get; set; }
    public string Accuracy { get; set; }
    public List<string> Actions { get; set; }
    public bool Check(string argument)
    {
        if (string.IsNullOrEmpty(Condition))
            return true;

        return JsExpression.Condition(Condition, ArgumentName, argument);
    }
}


public class Event
{
    public Guid Id { get; set; } = Guid.Empty;
    public Guid ContextId { get; set; } = Guid.Empty;
    public Guid[] Parents { get; set; } = new Guid[0];
    public DateTime DateTime { get; set; }
    public string Name { get; set; }
    public string Arg { get; set; }
    public override string ToString()
    {
        return $"{DateTime.ToString("HH:mm:ss")} - {Name}" + " " + $"{Arg}";
    }
}

internal class ConfigurationReader
{
    public static async Task<StereotypesConfiguration> Read()
    {
        var configurationFile = "StereotypesConfiguration.json";
        if (!System.IO.File.Exists(configurationFile))
            throw new Exception("Configuration doesn't exist");

        var configurationRaw = await System.IO.File.ReadAllTextAsync(configurationFile);
        var configuration = JsonConvert.DeserializeObject<StereotypesConfiguration>(configurationRaw);

        return configuration;
    }
}
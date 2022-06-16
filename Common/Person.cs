using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Common;
public class Person
{
    public string Name { get; set; }
    public DateTime Birthday { get; set; }
    public List<string> AppliedStereoTypes { get; set; } = new List<string>();
    public Dictionary<DateTime, string> AppliedActions { get; set; } = new Dictionary<DateTime, string>();
    public CardiovascularSystem CardiovascularSystem { get; set; } = new CardiovascularSystem();

    public int Age
    {
        get => (int)((DateTime.Now - Birthday).Days / 365);
    }

    public override string ToString()
    {
        return $"{Name}, {Age} y.o. {(CardiovascularSystem?.Measurements?.Count > 0 ? $"Measurements Count: {CardiovascularSystem.Measurements.Count}" : "")}";
    }
}

public class CardiovascularSystem
{
    public List<Measure> Measurements { get; set; } =
        new List<Measure>();
}

public enum MeasureType
{
    Pulse,
    Pressure
}

public class Measure
{
    public MeasureType Type { get; set; }
    public DateTime DateTime { get; set; }
    public string Name { get => Type.ToString(); }
    public string Value { get; set; }

    public override string ToString()
    {
        return $"{DateTime.ToString("HH:mm:ss")}\t{Type}\t{Value}";
    }
}

public class CardiovascularMeasurement
{
    public BloodPressure? BloodPressure { get; set; }
    public int? Pulse { get; set; }

    public override string ToString()
    {
        return $"Pulse:{Pulse}, Pressure: {BloodPressure}";
    }
}

public class BloodPressure
{
    public int DiastolicValue { get; set; }
    public int SystolicValue { get; set; }

    public override string ToString()
    {
        return $"{SystolicValue}-{DiastolicValue}";
    }
}
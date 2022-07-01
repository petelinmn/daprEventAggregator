using NCalc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StereotypeRecognizer
{
    public class NCalcExpression
    {
        static Dictionary<string, float> _values = new Dictionary<string, float>();
        private static Dictionary<string, float> Values
        {
            get => _values;
            set => _values = value;
        }

        public float Prop(string name)
        {
            return Values.ContainsKey(name) ? Values[name] : 0;
        }

        public float X { get; set; }

        public float Sin(float x)
        {
            return (float)Math.Sin(x);
        }

        public float Cos(float x)
        {
            return (float)Math.Cos(x);
        }

        public float Pow(float x, double y)
        {
            return (float)Math.Pow(x, y);
        }

        public static float Calculate(string expression, float XValue, Dictionary<string, float?> values)
        {
            Console.WriteLine($"Calculate expression:{expression}");
            Console.WriteLine(JsonConvert.SerializeObject(values, Formatting.Indented));
            Values = values.ToDictionary(v => v.Key.ToLower(), v => v.Value.HasValue ? v.Value.Value : 0);

            var expr = new Expression(expression.ToLower().Replace("{", "Prop('").Replace("}", "')"));
            var f = expr.ToLambda<NCalcExpression, float>();

            var context = new NCalcExpression { X = XValue };
            return f(context);
        }
    }
}

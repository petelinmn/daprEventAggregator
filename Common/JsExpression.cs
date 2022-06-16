using Jint;
using Jint.Native;
using Newtonsoft.Json;
using System;
namespace Common
{
    public class JsExpression
    {
        static JsValue GetJsValue(string command, string argumentName, object argument)
        {
            if (argumentName == null)
                argumentName = "e";

            var isEnumerable = argument is IEnumerable<string>;

            var engine = new Engine()
                .SetValue("log", new Action<object>(Console.WriteLine))
                .SetValue("result", "");

            var entireCommand =
                (isEnumerable 
                    ? $"const {argumentName} = [{string.Join(",", (argument as IEnumerable<object>).Select(i => $"JSON.parse({JsonConvert.SerializeObject(i)})"))}]; " 
                    : $"{argumentName} = JSON.parse({JsonConvert.SerializeObject(argument)}); ") +
                @$"result = ({command})";

            return engine.Execute(entireCommand).GetValue("result");
        }

        public static bool Condition(string command, string argumentName, object argument) =>
            GetJsValue(command, argumentName, argument).AsBoolean();

    }
}

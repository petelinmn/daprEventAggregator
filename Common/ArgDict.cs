using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class ArgDict
    {
        public string? Name { get; set; }
        public string? Value { get; set; }

        public static object? GetData(string arg, Type[] types, int nextIndex = 0)
        {
            try
            {
                if (nextIndex >= types.Length)
                    return null;

                return JsonConvert.DeserializeObject(arg, types[nextIndex]);
            }
            catch { }

            return GetData(arg, types, ++nextIndex);
        }

        public static Dictionary<string, Dictionary<string, float?>> GetData2(string arg)
        {
            Func<Dictionary<string, string>, Dictionary<string, float?>> toFloatValuesDict =
                dict =>
                {
                    var key = dict["Name"];
                    var value = dict.Where(i => i.Key != "Name")
                        .ToDictionary(i => i.Key, i =>
                        {
                            if (float.TryParse(i.Value, out var floatValue))
                                return floatValue;

                            return (float?)null;
                        });

                    return value;
                };

            var listOfDict = GetListOfDict(arg);

            var result = new Dictionary<string, Dictionary<string, float?>>();

            foreach (var item in listOfDict)
            {
                //if (!item.ContainsKey("Name"))
                //    continue;

                var key = item["Name"];

                var vals = toFloatValuesDict(item);

                if (result.ContainsKey(key))
                    result[key] = vals;
                else
                    result.Add(key, vals);
            }

            return result;
        }

        private static List<Dictionary<string, string>> GetListOfDict(string arg)
        {
            var dataObj = GetData(arg, new Type[]
            {
                new Dictionary<string, string>().GetType(),
                new List<Dictionary<string, string>>().GetType(),
                new List<List<Dictionary<string, string>>>().GetType(),
                new List<List<List<Dictionary<string, string>>>>().GetType(),
                new List<List<List<List<Dictionary<string, string>>>>>().GetType(),
            });

            var listOfDict = new List<Dictionary<string, string>>();

            var simpleList = dataObj as Dictionary<string, string>;
            if (simpleList != null)
            {
                listOfDict = new List<Dictionary<string, string>>()
                {
                    simpleList
                };
            }

            var doubleList = dataObj as List<Dictionary<string, string>>;
            if (doubleList != null)
            {
                listOfDict = doubleList;
            }

            var list3 = dataObj as List<List<Dictionary<string, string>>>;
            if (list3 != null)
            {
                listOfDict = list3.SelectMany(i => i).ToList();
            }

            var list4 = dataObj as List<List<List<Dictionary<string, string>>>>;
            if (list4 != null)
            {
                listOfDict = list4.SelectMany(i => i).SelectMany(i => i).ToList();
            }

            var list5 = dataObj as List<List<List<List<Dictionary<string, string>>>>>;
            if (list5 != null)
            {
                listOfDict = list5.SelectMany(i => i).SelectMany(i => i).SelectMany(i => i).ToList();
            }

            return listOfDict;
        }

        public static Dictionary<string, List<float>> FlattenToDict(List<List<Dictionary<string, string>>> data)
        {
            var charts = new Dictionary<string, List<float>>();
            foreach (var item in data.SelectMany(i => i))
            {
                if (!item.ContainsKey("Name") || item["Name"] == null)
                    continue;

                var floatVal = 0f;
                if (item.ContainsKey("Value"))
                    float.TryParse(item["Value"].ToString(), out floatVal);

                if (!charts.ContainsKey(item["Name"]))
                {
                    charts.Add(item["Name"], new List<float>() { floatVal });
                }
                else
                {
                    charts[item["Name"]].Add(floatVal);
                }
            }

            return charts;
        }

        public static Dictionary<string, List<PointF>> FlattenToDictPoints(List<List<Dictionary<string, string>>> data)
        {
            return FlattenToDict(data).ToDictionary(i => i.Key,
                i => i.Value.Select((y, x) => new PointF(x, y)).ToList());
        }

        public static Dictionary<string, List<PointF>> FlattenToDictPoints(List<Dictionary<string, string>> data)
        {
            var first = data.FirstOrDefault();
            if (first == null || !first.ContainsKey("Name"))
                return new Dictionary<string, List<PointF>>();

            var key = first["Name"];
            var list = data.Select((i, x) =>
            {
                if (i.ContainsKey("Value") && float.TryParse(i["Value"].ToString(), out var val))
                    return new PointF(x, val);
                else
                    return new PointF(x, 0f);
            }).ToList();

            var result = new Dictionary<string, List<PointF>>();
            result.Add(key, list);

            return result;
        }

        public static Dictionary<string, Dictionary<string, List<float?>>> FlattenToDictPoints2(List<List<Dictionary<string, string>>> data)
        {
            return FlattenToDictPoints2(data.SelectMany(i => i).ToList());
        }

        public static Dictionary<string, Dictionary<string, List<float?>>> FlattenToDictPoints2(List<Dictionary<string, string>> data)
        {
            var first = data.FirstOrDefault();
            if (first == null || !first.ContainsKey("Name"))
                return new Dictionary<string, Dictionary<string, List<float?>>>();

            var result = new Dictionary<string, Dictionary<string, List<float?>>>();
            var propsGroupByName = new Dictionary<string, List<string>>();
            foreach (var item in data)
            {
                if (!item.ContainsKey("Name"))
                    continue;

                var key = item["Name"];
                var list = new List<string>();
                if (!propsGroupByName.ContainsKey(key))
                    propsGroupByName.Add(key, list);
                else
                    list = propsGroupByName[key];


                foreach (var itemOfItem in item)
                {
                    if (!list.Contains(itemOfItem.Key))
                    {
                        list.Add(itemOfItem.Key);
                    }
                }
            }

            foreach (var item in data)
            {
                var obj = new Dictionary<string, List<float?>>();

                var key = item["Name"];
                if (!result.ContainsKey(key))
                    result.Add(key, obj);
                else
                    obj = result[key];

                var props = propsGroupByName[key];

                foreach (var propName in props)
                {
                    if (propName == "Name")
                        continue;
                    if (!obj.ContainsKey(propName))
                        obj.Add(propName, new List<float?>());

                    obj[propName].Add(item.ContainsKey(propName) && float.TryParse(item[propName], out var floatVal) ? floatVal : null);
                }
            }

            return result;
        }
    }
}

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace CS_Json
{
    internal class Program
    {
        private static bool IsValidJson(string s)
        {
            if ((s.StartsWith("{") && s.EndsWith("}")) || (s.StartsWith("[") && s.EndsWith("]")))
            {
                try
                {
                    var obj = JToken.Parse(s);
                    return true;
                }
                catch(JsonException e)
                {
                    WriteLine(e.Message);
                    return false;
                }
                catch (System.Exception e)
                {
                    WriteLine(e.Message);
                    return false;
                }
            }
            else
            {
                WriteLine("Not valid!");
                return false;
            }
        }

        private static IEnumerable<dynamic> Read(string json)
        {
            if (IsValidJson(json))
                try
                {
                    return JsonConvert.DeserializeObject<IEnumerable<dynamic>>(json);
                }
                catch (System.Exception e)
                {
                    WriteLine(e.Message);
                    return null;
                }
            return null;
        }

        private static void RecombiningTypeA(IEnumerable<dynamic> input)
        {
            var dict = new Dictionary<object, List<object>>();
            foreach (var item in input.FirstOrDefault())
            {
                dict.Add(item, new List<object>());
            }
            for (int i = 0; i < dict.Count; i++)
            {
                for (int j = 1; j < input.Count(); j++)
                {
                    try
                    {
                        dict.ElementAt(i).Value.Add(input.ElementAt(j)[i]);
                    }
                    catch (System.Exception e)
                    {
                        dict.ElementAt(i).Value.Add(null);
                    }
                }                
            }

            WriteLine(JsonConvert.SerializeObject(dict));
        }

        private static bool IsPropertyExist(dynamic item, string name)
        {
            if (item is JObject)
                return ((IDictionary<string, JToken>)item).ContainsKey(name);

            return item.GetType().GetProperty(name) != null;
        }

        private static void RecombiningTypeB(IEnumerable<dynamic> input)
        {
            var dict = new Dictionary<object, List<object>>();

            // Creating keys
            foreach (var item in input)
            {
                foreach (var i in item)
                {
                    if (dict.ContainsKey(i.Name))
                        continue;
                    dict.Add(i.Name, new List<object>());
                }
            }

            // Filling values
            foreach (var element in dict)
            {
                var temp = new List<object>();
                foreach (var item in input)
                {
                    if (IsPropertyExist(item, element.Key.ToString()))
                        temp.Add(item[element.Key].Value);
                    else
                        temp.Add(null);
                }
                element.Value.AddRange(temp);
            }

            WriteLine(JsonConvert.SerializeObject(dict));
        }

        private static void Main(string[] args)
        {
            var flag = false;
            do
            {
                WriteLine();
                WriteLine("Enter json string (enter 'e' to exit): ");
                var s = ReadLine().Trim();
                if (s == "e" || s == "E")
                {
                    flag = true;
                    continue;
                }

                s = System.Text.RegularExpressions.Regex.Replace(s,
                    @"((([A-Za-z]+[\w@]*|[\d.]+[A-Za-z]+[\w@]*|\S*@+\S*)\b(?<!\bnull)\b)+|(""+[\d.]+|[\d.]+"")+|(""[\w]+|[\w]""+|[\w]+@+)+)",
                    @"""$1""");
                s = System.Text.RegularExpressions.Regex.Replace(s, @"""+", @"""");
                
                var json = Read(s);
                if (json != null)
                {
                    if ((json.FirstOrDefault()).GetType() == typeof(JArray))
                        RecombiningTypeA(json);
                    else if ((json.FirstOrDefault()).GetType() == typeof(JObject))
                        RecombiningTypeB(json);
                }
            } while (!flag);
        }
    }
}
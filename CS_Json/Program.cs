using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace CS_Json
{
    internal class Program
    {
        private static IEnumerable<dynamic> Read(string json)
        {
            return JsonConvert.DeserializeObject<IEnumerable<dynamic>>(json);
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
                dict.ElementAt(i).Value.AddRange(input.Skip(1).Select(e => e[i]).ToList());
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
            WriteLine("Enter json string: ");
            var json = Read(ReadLine());
            if ((json.FirstOrDefault()).GetType() == typeof(JArray))
            {
                RecombiningTypeA(json);
            }
            else
                RecombiningTypeB(json);
            ReadKey();
        }
    }
}
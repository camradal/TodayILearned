using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace TodayILearned.Core
{
    public class Serializer
    {
        public Serializer()
        {
        }

        public static IEnumerable<ItemViewModel> GetItems(JArray tokens)
        {
            var items = new List<ItemViewModel>();
            foreach (JToken token in tokens)
            {
                string title = ProcessString(token["Title"].Value<string>());
                var itemViewModel = new ItemViewModel
                {
                    Title = title,
                    Url = token["Url"].Value<string>(),
                    Domain = token["Domain"].Value<string>(),
                    Thumbnail = token["Thumbnail"].Value<string>()
                };
                items.Add(itemViewModel);
            }
            return items;
        }

        public static IEnumerable<ItemViewModel> GetItems(JObject json)
        {
            var items = new List<ItemViewModel>();
            JToken tokens = json["data"]["children"];
            foreach (JToken token in tokens)
            {
                string title = ProcessString(token["data"]["title"].Value<string>());
                var itemViewModel = new ItemViewModel
                {
                    Title = title,
                    Url = token["data"]["url"].Value<string>(),
                    Domain = token["data"]["domain"].Value<string>(),
                    Thumbnail = token["data"]["thumbnail"].Value<string>()
                };
                items.Add(itemViewModel);
            }
            return items;
        }

        private static string ProcessString(string value)
        {
            const string tilThat = "TIL that";
            const string til = "TIL";
            if (value.StartsWith(tilThat)) value = value.Substring(tilThat.Length);
            if (value.StartsWith(til)) value = value.Substring(til.Length);
            value = value.TrimStart(new[] { ' ', '-', '.', ':' });
            value = char.ToUpper(value[0]) + value.Substring(1);
            return value;
        }
    }
}
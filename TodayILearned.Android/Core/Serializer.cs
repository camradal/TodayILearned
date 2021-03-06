﻿using System;
using System.Collections.Generic;
using System.Net;

using Android.Text;

using Newtonsoft.Json.Linq;

namespace TodayILearned.Core
{
    public class Serializer
    {
        public static IEnumerable<ItemViewModel> GetItems(JArray tokens)
        {
            var items = new List<ItemViewModel>();
            foreach (JToken token in tokens)
            {
                string title = ProcessString(token["Title"].Value<string>());
                string thumbnail = token["Thumbnail"].Value<string>();
                var itemViewModel = new ItemViewModel
                {
                    Title = title,
                    Url = token["Url"].Value<string>(),
                    Domain = token["Domain"].Value<string>()
                };
                
                if (thumbnail != "default")
                {
                	itemViewModel.Thumbnail = thumbnail;
                }
                
                items.Add(itemViewModel);
            }
            return items;
        }

        public static IList<ItemViewModel> GetItems(JObject json)
        {
            var items = new List<ItemViewModel>();
            JToken tokens = json["data"]["children"];
            foreach (JToken token in tokens)
            {
                string title = ProcessString(token["data"]["title"].Value<string>());
                string thumbnail = token["Thumbnail"].Value<string>();
                var itemViewModel = new ItemViewModel
                {
                    Title = title,
                    Url = token["data"]["url"].Value<string>(),
                    Domain = token["data"]["domain"].Value<string>()
                };
                
                if (thumbnail != "default")
                {
                	itemViewModel.Thumbnail = thumbnail;
                }
                
				if (!token["data"]["stickied"].Value<bool>())
				{
					items.Add(itemViewModel);
				}
            }
            return items;
        }

        private static string ProcessString(string value)
        {
			value = value.Trim(new[] { ' ', '-', '.', ':', ';', ',', '/', '\n', '[' });
			if (value.StartsWith("TIL", StringComparison.OrdinalIgnoreCase))
			{
				value = value.Substring("TIL".Length);
				value = Trim(value);
			}
			if (value.StartsWith("Today I Learned", StringComparison.OrdinalIgnoreCase))
			{
				value = value.Substring("Today I Learned".Length);
				value = Trim(value);
			}

			if (value[0] == '"')
			{
				value = "\"" + char.ToUpper(value[1]) + value.Substring(2);
			}
			else
			{
				value = char.ToUpper(value[0]) + value.Substring(1);
			}
			return value;
		}

		private static string Trim(string value)
		{
			value = value.TrimStart(new[] {' ', '-', '.', ':', ';', ',', '/', ']'});
			if (value.StartsWith("that", StringComparison.OrdinalIgnoreCase) && !value.StartsWith("that's", StringComparison.OrdinalIgnoreCase))
			{
				value = value.Substring("that".Length);
				value = value.TrimStart(new[] {' ', '-', '.', ':', ';', ','});
			}
			if (value.StartsWith("of ", StringComparison.OrdinalIgnoreCase))
			{
				value = value.Substring("of ".Length);
				value = value.TrimStart(new[] {' ', '-', '.', ':', ';', ','});
			}
			return value;
		}
    }
}

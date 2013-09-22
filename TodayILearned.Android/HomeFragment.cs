using System.Linq;
using System.Net;

using Android.OS;

using Newtonsoft.Json.Linq;

using TodayILearned.Core;

using ListFragment = Android.Support.V4.App.ListFragment;

namespace TodayILearned.AndroidApp
{
    internal class HomeFragment : ListFragment
    {
        public override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            var triviaTask = new WebClient().DownloadStringTaskAsync("http://reddit.com/r/todayilearned.json");

            var result = JObject.Parse(await triviaTask);
            var items = Serializer.GetItems(result);
            var lastItem = result["data"]["after"].ToString();

            ListAdapter = new EndlessTriviaItemAdapter(new TriviaItemAdapter(Activity, items.ToList()), lastItem);
        }
    }
}
using System.Linq;
using System.Net;

using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using TodayILearned.Core;

using ListFragment = Android.Support.V4.App.ListFragment;

namespace TodayILearned.AndroidApp
{
    internal class HomeFragment : ListFragment
    {
        private TriviaItemAdapter _triviaItemAdapter;

        public override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            var triviaTask = new WebClient().DownloadStringTaskAsync("http://reddit.com/r/todayilearned.json");

            var result = JObject.Parse(await triviaTask);
            var items = Serializer.GetItems(result);
            var lastItem = result["data"]["after"].ToString();

            _triviaItemAdapter = new TriviaItemAdapter(Activity, items.ToList());
            ListAdapter = new EndlessTriviaItemAdapter(_triviaItemAdapter, lastItem);
        }

        public override void OnListItemClick(ListView l, View v, int position, long id)
        {
            var item = _triviaItemAdapter.GetItem(position);
            
            var intent = new Intent(Activity, typeof (TriviaDetailsActivity));
            string json = JsonConvert.SerializeObject(item);
            intent.PutExtra("json", json);
            
            StartActivity(intent);
        }
    }
}
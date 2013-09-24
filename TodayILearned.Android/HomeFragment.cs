using System;
using System.Linq;
using System.Net;

using Android.Content;
using Android.Net;
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
        private bool _error;

        public override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            if (Utils.IsNetworkConnected(Activity))
            {
                try
                {
                    var triviaTask = new WebClient().DownloadStringTaskAsync("http://reddit.com/r/todayilearned.json");

                    var result = JObject.Parse(await triviaTask);
                    var items = Serializer.GetItems(result);
                    var lastItem = result["data"]["after"].ToString();

                    _triviaItemAdapter = new TriviaItemAdapter(Activity, items.ToList());
                    ListAdapter = new EndlessTriviaItemAdapter(_triviaItemAdapter, lastItem, "http://reddit.com/r/todayilearned.json?after={0}");
                }
                catch (Exception e)
                {
                    _error = true;
                    Toast.MakeText(Activity, e.Message, ToastLength.Short).Show();
                }
            }
            else
            {
                _error = true;
                Toast.MakeText(Activity, "No network connection", ToastLength.Short).Show();
            }
        }

        public override void OnStart()
        {
            base.OnStart();

            if (_error)
            {
                SetListShown(true);
            }

            ListView.FastScrollEnabled = true;
        }

        public override void OnListItemClick(ListView l, View v, int position, long id)
        {
            var item = _triviaItemAdapter.GetItem(position);

            var intent = new Intent(Activity, typeof(TriviaDetailsActivity));
            string json = JsonConvert.SerializeObject(item);
            intent.PutExtra("json", json);

            StartActivity(intent);
        }
    }

    class Utils
    {
        public static bool IsNetworkConnected(Context context)
        {
            var cm = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
            return cm.ActiveNetworkInfo != null;
        }
    }
}
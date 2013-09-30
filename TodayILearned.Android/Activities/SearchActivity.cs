using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Java.Util.Jar;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using TodayILearned.Core;

namespace TodayILearned.AndroidApp
{
    [Activity(Label = "Search", Icon = "@drawable/ic_launcher")]
    [IntentFilter(new[] { Intent.ActionSearch })]
    [MetaData("android.app.searchable", Resource = "@xml/searchable")]
    public class SearchActivity : ListActivity
    {
        private string SearchUrl = "http://www.reddit.com/r/todayilearned/search.json?sort=relevance&restrict_sr=on&t=all&q=";
        private TriviaItemAdapter _triviaItemAdapter;

        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            RequestWindowFeature(WindowFeatures.IndeterminateProgress);

            if (Intent.Action == Intent.ActionSearch)
            {
                if (Utils.IsNetworkConnected(this))
                {
                    try
                    {
                        SetProgressBarIndeterminateVisibility(true);
                        var query = Intent.GetStringExtra(SearchManager.Query);
                        var triviaTask = new WebClient().DownloadStringTaskAsync(SearchUrl + query);

                        var result = JObject.Parse(await triviaTask);
                        var items = Serializer.GetItems(result);
                        var lastItem = result["data"]["after"].ToString();

                        _triviaItemAdapter = new TriviaItemAdapter(this, items.ToList());
                        ListAdapter = new EndlessTriviaItemAdapter(_triviaItemAdapter, lastItem, SearchUrl + query + "&after={0}");
                    }
                    catch (Exception e)
                    {
                        Toast.MakeText(this, e.Message, ToastLength.Short).Show();
                    }
                }
                else
                {
                    Toast.MakeText(this, "No network connection", ToastLength.Short).Show();
                }
                SetProgressBarIndeterminateVisibility(false);
            }
        }

        protected override void OnStart()
        {
            base.OnStart();
            ListView.FastScrollEnabled = true;
        }

        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            var item = _triviaItemAdapter.GetItem(position);

            var intent = new Intent(this, typeof(TriviaDetailsActivity));
            string json = JsonConvert.SerializeObject(item);
            intent.PutExtra("json", json);

            StartActivity(intent);
        }
    }
}
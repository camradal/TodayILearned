using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Java.Util.Jar;

namespace TodayILearned.AndroidApp
{
    [Activity(Label = "Search")]
    [IntentFilter(new[] { Intent.ActionSearch })]
    [MetaData("android.app.searchable", Resource = "@xml/searchable")]
    public class SearchActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            if (Intent.Action == Intent.ActionSearch)
            {
                var query = Intent.GetStringExtra(SearchManager.Query);
                Toast.MakeText(this,query,ToastLength.Short).Show();
            }
        }
    }
}
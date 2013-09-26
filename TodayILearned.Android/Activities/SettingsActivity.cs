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

namespace TodayILearned.AndroidApp
{
    [Activity(Label = "Settings", Icon = "@drawable/ic_launcher")]
    public class SettingsActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            FragmentManager.BeginTransaction()
                .Replace(Android.Resource.Id.Content, new SettingsFragment())
                .Commit();
        }
    }
}
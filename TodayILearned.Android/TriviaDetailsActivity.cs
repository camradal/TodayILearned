using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;

namespace TodayILearned.AndroidApp
{
    [Activity(Label = "")]
    public class TriviaDetailsActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Window.RequestFeature(WindowFeatures.Progress);
            SetContentView(Resource.Layout.TriviaDetails);

            FindViewById<TextView>(Resource.Id.titleTextView).Text = Intent.Extras.GetString("title");

            var webView = FindViewById<WebView>(Resource.Id.webView);

            webView.SetWebViewClient(new WebViewClient());
            webView.SetWebChromeClient(new ProgressClient(this));
            webView.Settings.JavaScriptEnabled = true;

            webView.LoadUrl(Intent.Extras.GetString("url"));
        }

        class ProgressClient : WebChromeClient
        {
            private readonly Activity _activity;

            public ProgressClient(Activity activity)
            {
                _activity = activity;
            }

            public override void OnProgressChanged(WebView view, int newProgress)
            {
                if (_activity != null)
                {
                    _activity.SetProgress(newProgress * 100);
                }
            }
        }
    }
}
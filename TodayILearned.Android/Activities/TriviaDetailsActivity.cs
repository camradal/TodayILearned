using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;

using Newtonsoft.Json;

using File = Java.IO.File;

namespace TodayILearned.AndroidApp
{
    [Activity(Label = "Trivia details", Icon = "@drawable/ic_launcher")]
    public class TriviaDetailsActivity : Activity
    {
        private ItemViewModel _trivia;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Window.RequestFeature(WindowFeatures.Progress);
            SetContentView(Resource.Layout.TriviaDetails);

            var json = Intent.Extras.GetString("json");
            _trivia = JsonConvert.DeserializeObject<ItemViewModel>(json);
            FindViewById<TextView>(Resource.Id.titleTextView).Text = _trivia.Title;

            var webView = FindViewById<WebView>(Resource.Id.webView);

            webView.SetWebViewClient(new WebViewClient());
            webView.SetWebChromeClient(new ProgressClient(this));
            webView.Settings.JavaScriptEnabled = true;

            webView.LoadUrl(_trivia.Url);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.DetailsMenu, menu);

            var shareMenuItem = menu.FindItem(Resource.Id.share);
            var shareActionProvider = (ShareActionProvider)shareMenuItem.ActionProvider;
            shareActionProvider.SetShareIntent(CreateIntent());

            ToggleFavouritesMenu(menu);

            return base.OnCreateOptionsMenu(menu);
        }

        private void ToggleFavouritesMenu(IMenu menu)
        {
            var file = new File(FilesDir, _trivia.GetHashCode() + ".json");
            var isFavourite = file.Exists();

            menu.FindItem(Resource.Id.addFavourites).SetVisible(!isFavourite);
            menu.FindItem(Resource.Id.removeFavourites).SetVisible(isFavourite);
        }

        private Intent CreateIntent()
        {
            string message = _trivia.Title + "\n\n" + _trivia.Url + "\n\nShared via Trivia Buff";

            var urlIntent = new Intent(Intent.ActionSend);
            urlIntent.SetType("text/plain");
            urlIntent.PutExtra(Intent.ExtraSubject, _trivia.Title);
            urlIntent.PutExtra(Intent.ExtraText, message);
            return urlIntent;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.addFavourites)
            {
                using (var stream = OpenFileOutput(_trivia.GetHashCode() + ".json", FileCreationMode.Private))
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.Write(JsonConvert.SerializeObject(_trivia));
                    }
                }

                InvalidateOptionsMenu();
                Toast.MakeText(this, "Added to favorites", ToastLength.Short).Show();
            }

            if (item.ItemId == Resource.Id.removeFavourites)
            {
                var file = new File(FilesDir, _trivia.GetHashCode() + ".json");
                file.Delete();

                InvalidateOptionsMenu();
                Toast.MakeText(this, "Removed from favorites", ToastLength.Short).Show();
            }

            return base.OnOptionsItemSelected(item);
        }
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
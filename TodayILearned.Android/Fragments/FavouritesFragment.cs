using System.Collections.Generic;
using System.IO;
using System.Linq;

using Android.Content;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

using Newtonsoft.Json;

namespace TodayILearned.AndroidApp
{
    internal class FavouritesFragment : ListFragment
    {
        private TriviaItemAdapter _triviaItemAdapter;

        public override void OnStart()
        {
            base.OnStart();

            var files = Directory.GetFiles(Activity.FilesDir.AbsolutePath, "*.json");

            var items = files.Select(file => JsonConvert.DeserializeObject<ItemViewModel>(File.ReadAllText(file))).ToList();

            _triviaItemAdapter = new TriviaItemAdapter(Activity, items);
            ListAdapter = _triviaItemAdapter;
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
}
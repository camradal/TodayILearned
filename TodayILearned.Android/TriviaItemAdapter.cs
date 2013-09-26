using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Android.App;
using Android.Preferences;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;

using EndlessScrollPort;

using Newtonsoft.Json.Linq;

using TodayILearned.Core;

using UrlImageViewHelper;

namespace TodayILearned.AndroidApp
{
    public class TriviaItemAdapter : ArrayAdapter<ItemViewModel>
    {
        List<ItemViewModel> items;
        Activity context;

        public TriviaItemAdapter(Activity context, List<ItemViewModel> items)
            : base(context, Android.Resource.Layout.SimpleListItem1, items)
        {
            this.context = context;
            this.items = items;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override int Count
        {
            get { return items.Count; }
        }

        public override void AddAll(ICollection collection)
        {
            base.AddAll(collection);
            items.AddRange(collection.Cast<ItemViewModel>());
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];
            View view = convertView ?? context.LayoutInflater.Inflate(Resource.Layout.TriviaItem, null);

            var preferences = PreferenceManager.GetDefaultSharedPreferences(context);
            var size = preferences.GetString("pref_size", "0");

            var titleView = view.FindViewById<TextView>(Resource.Id.Text1);
            var domainView = view.FindViewById<TextView>(Resource.Id.Text2);

            if (size == "0")
            {
                titleView.SetTextAppearance(context, Android.Resource.Style.TextAppearanceSmall);
                domainView.SetTextAppearance(context, Android.Resource.Style.TextAppearanceSmall);
            }
            else
            {
                titleView.SetTextAppearance(context, Android.Resource.Style.TextAppearanceMedium);
                domainView.SetTextAppearance(context, Android.Resource.Style.TextAppearanceMedium);
            }

            titleView.Text = item.Title;
            domainView.Text = item.Domain;

            view.FindViewById<ImageView>(Resource.Id.Image).SetUrlDrawable(item.Thumbnail, Resource.Drawable.ic_launcher,
                                                            UrlImageViewHelper.UrlImageViewHelper.CACHE_DURATION_INFINITE);
            return view;
        }
    }

    class EndlessTriviaItemAdapter : EndlessAdapterBase
    {
        private string _after;
        private readonly RotateAnimation _rotateAnimation = null;
        private View _pendingItemView = null;
        private List<ItemViewModel> _newItems;
        private string _dataUrl;

        public EndlessTriviaItemAdapter(IListAdapter wrapped, string after, string dataUrl)
            : base(wrapped)
        {
            _after = after;
            _dataUrl = dataUrl;

            _rotateAnimation = new RotateAnimation(0f, 360f, Dimension.RelativeToSelf, 0.5f, Dimension.RelativeToSelf, 0.5f);

            _rotateAnimation.Duration = 600;
            _rotateAnimation.RepeatMode = RepeatMode.Restart;
            _rotateAnimation.RepeatCount = Animation.Infinite;
        }


        protected override async Task<bool> LoadData()
        {
            var triviaTask = new WebClient().DownloadStringTaskAsync(string.Format(_dataUrl, _after));

            var result = JObject.Parse(await triviaTask);
            _newItems = Serializer.GetItems(result).ToList();

            _after = result["data"]["after"].ToString();

            return _newItems.Count > 0;
        }

        protected override void AppendCachedData()
        {
            ((ArrayAdapter<ItemViewModel>)WrappedAdapter).AddAll(_newItems);
        }

        protected override View GetPendingItemView(ViewGroup parent)
        {
            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.TriviaItem, null);

            row.FindViewById(Resource.Id.Text).Visibility = ViewStates.Gone;
            row.FindViewById(Resource.Id.Image).Visibility = ViewStates.Gone;

            _pendingItemView = row.FindViewById(Resource.Id.throbber);
            _pendingItemView.Visibility = ViewStates.Visible;

            StartProgressAnimation();

            return row;
        }

        private void StartProgressAnimation()
        {
            if (_pendingItemView != null)
            {
                _pendingItemView.StartAnimation(_rotateAnimation);
            }
        }
    }
}
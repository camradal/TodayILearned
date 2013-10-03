using System;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Android.Content;
using Android.Net;
using Android.OS;
using Android.Provider;
using Android.Views;
using Android.Widget;

using Com.Google.Ads;

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
        private AdView _adView;
        private string _message;

        private const int InternalEmptyID = 0x00ff0001;
        private const int InternalProgressContainerID = 0x00ff0002;
        private const int InternalListContainerID = 0x00ff0003;

        public override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;
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
                    _message = e.Message;
                }
            }
            else
            {
                _error = true;
                _message = "No network connection";
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.TriviaList, container, false);


            //http://stackoverflow.com/a/12145683/239438
            view.FindViewById(Resource.Id.empty_id).Id = InternalEmptyID;
            view.FindViewById(Resource.Id.progress_container_id).Id = InternalProgressContainerID;
            view.FindViewById(Resource.Id.list_container_id).Id = InternalListContainerID;

            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            _adView = view.FindViewById<AdView>(Resource.Id.ad);

            var adRequest = new AdRequest();

#if DEBUG
            adRequest.SetTesting(true);
            adRequest.AddTestDevice(GetDeviceId());
            adRequest.AddTestDevice(AdRequest.TestEmulator);
#endif

            // Start loading the ad in the background.
            _adView.LoadAd(adRequest);
        }

        public override void OnStart()
        {
            base.OnStart();

            if (_error)
            {
                SetListShown(true);
                Toast.MakeText(Activity, _message, ToastLength.Short).Show();
            }
        }

        public override void OnDestroy()
        {
            // Destroy the AdView.
            _adView.Destroy();

            base.OnDestroy();
        }

        public override void OnListItemClick(ListView l, View v, int position, long id)
        {
            var item = _triviaItemAdapter.GetItem(position);

            var intent = new Intent(Activity, typeof(TriviaDetailsActivity));
            string json = JsonConvert.SerializeObject(item);
            intent.PutExtra("json", json);

            StartActivity(intent);
        }

        public string GetDeviceId()
        {
            String id = Settings.Secure.GetString(Activity.ContentResolver, Settings.Secure.AndroidId);

            return CalculateMD5Hash(id);
        }

        public string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            var sb = new StringBuilder();
            foreach (byte t in hash)
            {
                sb.Append(t.ToString("X2"));
            }
            return sb.ToString();
        }

        public void HideAd()
        {
            _adView.Visibility = ViewStates.Gone;
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
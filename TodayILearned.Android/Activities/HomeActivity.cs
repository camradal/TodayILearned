using System;
using System.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;

using Xamarin.InAppBilling;
using Xamarin.InAppBilling.Model;
using Xamarin.InAppBilling.Utilities;

using FragmentTransaction = Android.App.FragmentTransaction;

namespace TodayILearned.AndroidApp
{
    [Activity(Label = "Trivia Buff", MainLauncher = true, Icon = "@drawable/ic_launcher")]
    public class HomeActivity : FragmentActivity, ActionBar.ITabListener, ViewPager.IOnPageChangeListener
    {
        /// <summary>
        /// Replace with public key from android developer console.
        /// </summary>
        private const string PublicKey = "";

        /// <summary>
        /// Replace with the produck sku for removing ads from developer console.
        /// </summary>
        private const string RemoveAdsSku = "";

        ViewPager _pager;
        private InAppBillingServiceConnection _serviceConnection;
        private IInAppBillingHelper _billingHelper;

        #region Lifecycle

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            PreferenceManager.SetDefaultValues(this, Resource.Xml.preferences, false);

            _pager = FindViewById<ViewPager>(Resource.Id.pager);
            _pager.Adapter = new SectionsPagerAdapter(SupportFragmentManager);
            _pager.SetOnPageChangeListener(this);

            ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
            ActionBar.AddTab(ActionBar.NewTab().SetText("New").SetTabListener(this).SetTag("New"));
            ActionBar.AddTab(ActionBar.NewTab().SetText("Favourites").SetTabListener(this).SetTag("Favourites"));


            _serviceConnection = new InAppBillingServiceConnection(this, PublicKey);
            _serviceConnection.OnConnected += HandleOnConnected;

            _serviceConnection.Connect();
        }

        protected override void OnDestroy()
        {
            if (_serviceConnection != null)
            {
                _serviceConnection.Disconnected();
            }

            base.OnDestroy();
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutInt("selected", ActionBar.SelectedNavigationIndex);
        }

        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            if (savedInstanceState.ContainsKey("selected"))
            {
                ActionBar.SetSelectedNavigationItem(savedInstanceState.GetInt("selected"));
            }
        }

        #endregion

        #region Menu

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.MainMenu, menu);

            var searchManager = (SearchManager)GetSystemService(SearchService);
            IMenuItem item = menu.FindItem(Resource.Id.action_search);
            var searchView = (SearchView)item.ActionView;

            var name = new ComponentName(this, "todayilearned.androidapp.SearchActivity");
            var info = searchManager.GetSearchableInfo(name);
            searchView.SetSearchableInfo(info);
            searchView.SetIconifiedByDefault(false);

            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.action_settings)
            {
                StartActivity(typeof(SettingsActivity));
            }

            if (item.ItemId == Resource.Id.action_buy)
            {
                if (_billingHelper != null)
                {
                    _billingHelper.LaunchPurchaseFlow(RemoveAdsSku, ItemType.InApp, "");
                }
                else
                {
                    Toast.MakeText(this, "In app purchase not available", ToastLength.Long).Show();
                }
                ToggleAdIfNeeded();
            }

            return base.OnOptionsItemSelected(item);
        }

        #endregion

        private void HandleOnConnected(object sender, EventArgs e)
        {
            _billingHelper = _serviceConnection.BillingHelper;

            ToggleAdIfNeeded();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            _billingHelper.HandleActivityResult(requestCode, resultCode, data);

            ToggleAdIfNeeded();
        }

        private void ToggleAdIfNeeded()
        {
            if (_billingHelper != null)
            {
                var purchases = _billingHelper.GetPurchases(ItemType.InApp);
                if (purchases.Any())
                {
                    var fragmentByTag = SupportFragmentManager.FindFragmentByTag("android:switcher:" + Resource.Id.pager + ":0");
                    var fragment = (HomeFragment)fragmentByTag;
                    fragment.HideAd();
                }
            }
        }

        #region TabListener

        public void OnTabReselected(ActionBar.Tab tab, FragmentTransaction ft)
        {

        }

        public void OnTabSelected(ActionBar.Tab tab, FragmentTransaction ft)
        {
            if (_pager != null)
            {
                _pager.CurrentItem = tab.Position;
            }
        }

        public void OnTabUnselected(ActionBar.Tab tab, FragmentTransaction ft)
        {

        }

        #endregion

        #region ViewPager

        public void OnPageScrollStateChanged(int state)
        {

        }

        public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
        {

        }

        public void OnPageSelected(int position)
        {
            ActionBar.SetSelectedNavigationItem(position);
        }

        #endregion

    }
}
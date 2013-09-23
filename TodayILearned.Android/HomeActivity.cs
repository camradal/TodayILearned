using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;

using FragmentTransaction = Android.App.FragmentTransaction;

namespace TodayILearned.AndroidApp
{
    [Activity(Label = "Trivia Buff", MainLauncher = true, Icon = "@drawable/icon")]
    public class HomeActivity : FragmentActivity, ActionBar.ITabListener, ViewPager.IOnPageChangeListener
    {
        ViewPager pager;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            pager = FindViewById<ViewPager>(Resource.Id.pager);
            pager.Adapter = new SectionsPagerAdapter(SupportFragmentManager);
            pager.SetOnPageChangeListener(this);

            ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
            ActionBar.AddTab(ActionBar.NewTab().SetText("New").SetTabListener(this).SetTag("New"));
            ActionBar.AddTab(ActionBar.NewTab().SetText("Favourites").SetTabListener(this).SetTag("Favourites"));
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


        public void OnTabReselected(ActionBar.Tab tab, FragmentTransaction ft)
        {

        }

        public void OnTabSelected(ActionBar.Tab tab, FragmentTransaction ft)
        {
            if (pager != null)
            {
                pager.CurrentItem = tab.Position;
            }
        }

        public void OnTabUnselected(ActionBar.Tab tab, FragmentTransaction ft)
        {

        }


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
    }
}
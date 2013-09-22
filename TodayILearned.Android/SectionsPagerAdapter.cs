using Android.Support.V4.App;

namespace TodayILearned.Android
{
    class SectionsPagerAdapter : FragmentPagerAdapter
    {
        public SectionsPagerAdapter(FragmentManager manager)
            : base(manager)
        {

        }

        public override int Count
        {
            get
            {
                return 2;
            }
        }

        public override Fragment GetItem(int position)
        {
            if (position == 0)
            {
                return new HomeFragment();
            }

            if (position == 1)
            {
                return new FavouritesFragment();
            }

            return null;
        }
    }
}
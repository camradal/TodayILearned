using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Text;
using Android.Views;
using Android.Widget;

namespace TodayILearned.AndroidApp
{
    public class SettingsFragment : PreferenceFragment, ISharedPreferencesOnSharedPreferenceChangeListener
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            AddPreferencesFromResource(Resource.Xml.preferences);
        }

        public override void OnResume()
        {
            base.OnResume();

            PreferenceScreen.SharedPreferences.RegisterOnSharedPreferenceChangeListener(this);
        }

        public override void OnPause()
        {
            base.OnPause();

            PreferenceScreen.SharedPreferences.UnregisterOnSharedPreferenceChangeListener(this);
        }

        public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
        {
            var size = sharedPreferences.GetString(key, "0");
            FindPreference(key).Summary = size == "0" ? "Small" : "Medium";
        }
    }
}
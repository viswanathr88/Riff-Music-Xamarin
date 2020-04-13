using Android.OS;

namespace OnePlayer.Droid.UI.Settings
{
    public class SettingsFragment : Android.Support.V7.Preferences.PreferenceFragmentCompat
    {
        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            PreferenceManager.SharedPreferencesName = "com.oneplayer.droid.app.preferences";
            SetPreferencesFromResource(Resource.Xml.fragment_settings, rootKey);
        }
    }
}
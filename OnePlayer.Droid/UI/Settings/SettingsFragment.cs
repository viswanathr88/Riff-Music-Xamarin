using Android.OS;
using Android.Views;
using OnePlayer.Authentication;
using OnePlayer.Data;
using System;

namespace OnePlayer.Droid.UI.Settings
{
    public class SettingsFragment : Android.Support.V7.Preferences.PreferenceFragmentCompat
    {
        private LoginManager loginManager;

        public SettingsFragment(LoginManager loginManager)
        {
            this.loginManager = loginManager;
        }

        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            PreferenceManager.SharedPreferencesName = "com.oneplayer.droid.app.preferences";
            SetPreferencesFromResource(Resource.Xml.fragment_settings, rootKey);
        }

        public async override void OnResume()
        {
            base.OnResume();

            try
            {
                bool loginExists = await loginManager.LoginExistsAsync();
                var signInPref = FindPreference("signIn");
                var signOutPref = FindPreference("signOut");
                if (loginExists)
                {
                    UserProfile profile = await loginManager.GetUserAsync();
                    signOutPref.Summary = $"Signed in as {profile.DisplayName} ({profile.Email})";
                    signOutPref.Visible = true;
                    signInPref.Visible = false;
                }
                else
                {
                    signInPref.Visible = true;
                    signOutPref.Visible = false;
                }

            }
            catch (Exception ex)
            {

            }
        }
    }
}
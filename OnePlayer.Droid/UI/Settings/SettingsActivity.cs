
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;

namespace OnePlayer.Droid.UI.Settings
{
    [Activity(Label = "@string/settings_activity_title")]
    public class SettingsActivity : Android.Support.V7.App.AppCompatActivity, Android.Content.ISharedPreferencesOnSharedPreferenceChangeListener
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_settings);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.settings_container, new SettingsFragment())
                .Commit();

            GetSharedPreferences("com.oneplayer.droid.app.preferences", Android.Content.FileCreationMode.Private).RegisterOnSharedPreferenceChangeListener(this);

        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    break;
                default:
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

        public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
        {
        }
    }
}
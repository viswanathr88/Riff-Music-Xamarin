using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;

namespace OnePlayer.Droid.UI
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private OnePlayer.Droid.Sync.OneDriveSyncServiceConnection syncServiceConnection = null;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);
            navigationView.SetCheckedItem(Resource.Id.nav_library);
            navigationView.Menu.PerformIdentifierAction(Resource.Id.nav_library, 0);
        }

        protected async override void OnStart()
        {
             base.OnStart();

            if (this.syncServiceConnection == null)
            {
                this.syncServiceConnection = new Sync.OneDriveSyncServiceConnection();
            }

            var appContext = ApplicationContext as IOnePlayerApp;
            bool loginExists = await appContext.LoginManager.LoginExistsAsync();

            if (!this.syncServiceConnection.IsConnected)
            {
                if (loginExists)
                {
                    Intent serviceIntent = new Intent(this, typeof(OnePlayer.Droid.Sync.OneDriveSyncService));
                    BindService(serviceIntent, this.syncServiceConnection, Bind.AutoCreate);
                }
            }
            else if (!loginExists)
            {
                UnbindService(this.syncServiceConnection);
            }
        }

        public override void OnBackPressed()
        {
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            if(drawer.IsDrawerOpen(GravityCompat.Start))
            {
                drawer.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                base.OnBackPressed();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            Android.Support.V4.App.Fragment fragment = null;

            if (id == Resource.Id.nav_home)
            {
                SetTitle(Resource.String.menu_item_home);
                fragment = new Home.HomeFragment();
            }
            else if (id == Resource.Id.nav_library)
            {
                SetTitle(Resource.String.menu_item_library);
                fragment = new MusicLibrary.MusicLibraryFragment(this);
            }
            else if (id == Resource.Id.nav_recent_plays)
            {
                SetTitle(Resource.String.menu_item_recent_plays);
                fragment = new Home.RecentPlaysFragment();
            }
            else if (id == Resource.Id.nav_settings)
            {
                var intent = new Android.Content.Intent(this, typeof(Settings.SettingsActivity));
                intent.SetFlags(Android.Content.ActivityFlags.NewTask);
                StartActivity(intent);
            }

            if (fragment != null)
            {
                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.content_frame, fragment)
                    .Commit();
            }

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawer.CloseDrawer(GravityCompat.Start);
            return true;
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}


using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using OnePlayer.Droid.UI.MusicLibrary;
using System.Threading.Tasks;

namespace OnePlayer.Droid.UI
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private OnePlayer.Droid.Sync.OneDriveSyncServiceConnection syncServiceConnection = null;
        private IMenu menu = null;
        private ImageView profilePictureImageView = null;
        private TextView userNameTextView = null;
        private TextView userEmailTextView = null;
        private bool profileSet = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Register notification channel
            CreateNotificationChannel();

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

            profilePictureImageView = navigationView.GetHeaderView(0).FindViewById<ImageView>(Resource.Id.profilePicture);
            userNameTextView = navigationView.GetHeaderView(0).FindViewById<TextView>(Resource.Id.userName);
            userEmailTextView = navigationView.GetHeaderView(0).FindViewById<TextView>(Resource.Id.userEmail);

        }

        protected async override void OnStart()
        {
             base.OnStart();

            if (this.syncServiceConnection == null)
            {
                this.syncServiceConnection = new Sync.OneDriveSyncServiceConnection();
            }

            var app = ApplicationContext as IOnePlayerApp;
            app.SyncEngine.StateChanged += SyncEngine_StateChanged;
            UpdateSyncStateIcon(this.menu, app.SyncEngine.State);

            bool loginExists = await app.LoginManager.LoginExistsAsync();
            if (loginExists && !profileSet)
            {
                var profile = await Task.Run(async () => await app.LoginManager.GetUserAsync());
                userNameTextView.Text = profile.DisplayName;
                userEmailTextView.Text = profile.Email;

                using var stream = await Task.Run(async () => await app.LoginManager.GetUserPhotoAsync());
                profilePictureImageView.SetImageBitmap(Android.Graphics.BitmapFactory.DecodeStream(stream));

                profileSet = true;
            }

            if (!this.syncServiceConnection.IsConnected && !app.Preferences.IsSyncPaused)
            {

                if (loginExists)
                {
                    app.SyncEngine.Start();
                    Intent serviceIntent = new Intent(this, typeof(OnePlayer.Droid.Sync.OneDriveSyncService));
                    BindService(serviceIntent, this.syncServiceConnection, Bind.AutoCreate);
                }
            }
            else if (!loginExists || app.Preferences.IsSyncPaused)
            {
                if (this.syncServiceConnection.IsConnected)
                {
                    Intent serviceIntent = new Intent(this, typeof(OnePlayer.Droid.Sync.OneDriveSyncService));
                    UnbindService(this.syncServiceConnection);
                    StopService(serviceIntent);
                }
                app.SyncEngine.Stop();
            }
        }

        protected override void OnStop()
        {
            base.OnStop();

            var app = ApplicationContext as IOnePlayerApp;
            app.SyncEngine.StateChanged -= SyncEngine_StateChanged;
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
            base.OnCreateOptionsMenu(menu);
            this.menu = menu;

            var app = ApplicationContext as IOnePlayerApp;
            UpdateSyncStateIcon(menu, app.SyncEngine.State);

            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.menu_action_search)
            {
                Intent intent = new Intent(this, typeof(SearchActivity));
                StartActivity(intent);
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

        private void CreateNotificationChannel()
        {
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                string channelName = GetString(Resource.String.channel_name);
                string channelDescription = GetString(Resource.String.channel_descrioption);
                var importance = Android.App.NotificationImportance.Default;

                var notificationChannel = new NotificationChannel("sync_notification_channel_id", channelName, importance)
                {
                    Description = channelDescription
                };

                NotificationManager manager = (NotificationManager)GetSystemService(NotificationService);
                manager.CreateNotificationChannel(notificationChannel);
            }
        }

        private void SyncEngine_StateChanged(object sender, OnePlayer.Sync.SyncState e)
        {
            RunOnUiThread(() =>
            {
                // Ensure menu is created
                if (this.menu != null)
                {
                    UpdateSyncStateIcon(this.menu, e);
                }
            });
        }

        private void UpdateSyncStateIcon(IMenu menu, OnePlayer.Sync.SyncState state)
        {
            if (menu == null)
            {
                return;
            }

            var menuItem = this.menu.FindItem(Resource.Id.menu_action_sync_status);
            if (menuItem.ActionView != null)
            {
                menuItem.ActionView.ClearAnimation();
                menuItem.SetActionView(null);
            }

            if (state == OnePlayer.Sync.SyncState.Uptodate)
            {
                menuItem.SetIcon(Resource.Drawable.ic_sync_uptodate);
            }
            else if (state == OnePlayer.Sync.SyncState.Syncing)
            {
                // Get the sync image view
                var imageView = LayoutInflater.Inflate(Resource.Layout.sync_icon_image_view, null);

                // Create a rotating animation
                Android.Views.Animations.Animation anim = Android.Views.Animations.AnimationUtils.LoadAnimation(this, Resource.Animation.rotation);
                anim.RepeatCount = Android.Views.Animations.Animation.Infinite;
                anim.RepeatMode = Android.Views.Animations.RepeatMode.Restart;
                anim.Duration = 1500;

                // Start the animation
                imageView.StartAnimation(anim);
                menuItem.SetActionView(imageView);
            }
            else if (state == OnePlayer.Sync.SyncState.Stopped)
            {
                menuItem.SetIcon(Resource.Drawable.ic_sync_disabled);
            }
            else if (state == OnePlayer.Sync.SyncState.NotSyncing)
            {
                menuItem.SetIcon(Resource.Drawable.ic_sync_problem);
            }
            else
            {
                menuItem.SetIcon(Resource.Drawable.ic_sync_started);
            }
        }
    }
}


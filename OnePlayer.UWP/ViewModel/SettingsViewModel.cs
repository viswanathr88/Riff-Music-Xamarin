using OnePlayer.Authentication;
using OnePlayer.Data;
using OnePlayer.Sync;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace OnePlayer.UWP.ViewModel
{
    public sealed class SettingsViewModel : DataViewModel
    {
        private readonly ILoginManager loginManager;
        private readonly IPreferences preferences;
        private readonly SyncEngine syncEngine;
        private UserProfile user;
        private ImageSource userPhoto;

        public SettingsViewModel(ILoginManager loginManager, IPreferences preferences, SyncEngine syncEngine)
        {
            this.loginManager = loginManager ?? throw new ArgumentNullException(nameof(loginManager));
            this.preferences = preferences ?? throw new ArgumentNullException(nameof(preferences));
            this.syncEngine = syncEngine ?? throw new ArgumentNullException(nameof(syncEngine));
        }

        public UserProfile User
        {
            get => user;
            private set => SetProperty(ref this.user, value);
        }

        public ImageSource UserPhoto
        {
            get => userPhoto;
            private set => SetProperty(ref this.userPhoto, value);
        }

        public bool IsSyncPaused
        {
            get => this.preferences.IsSyncPaused;
            private set
            {
                if (IsSyncPaused != value)
                {
                    preferences.IsSyncPaused = value;
                    RaisePropertyChanged(nameof(IsSyncPaused));
                }
            }
        }

        public void PauseSync()
        {
            if (!IsSyncPaused)
            {
                syncEngine.Stop();
                RaisePropertyChanged(nameof(IsSyncPaused));
            }
        }

        public void UnpauseSync()
        {
            if (IsSyncPaused)
            {
                IsSyncPaused = false;
                syncEngine.Start();
            }
        }

        public async override Task LoadAsync()
        {
            IsLoading = true;

            // Get user
            User = await this.loginManager.GetUserAsync();

            // Get User Photo
            using (var stream = await this.loginManager.GetUserPhotoAsync())
            {
                using (var raStream = stream.AsRandomAccessStream())
                {
                    var bImage = new BitmapImage
                    {
                        DecodePixelHeight = 96,
                        DecodePixelWidth = 96
                    };

                    await bImage.SetSourceAsync(raStream);
                    UserPhoto = bImage;
                }
            }



            IsLoading = false;
            IsLoaded = true;
        }

        public Task SignOutAsync()
        {
            return this.loginManager.SignOutAsync();
        }
    }
}

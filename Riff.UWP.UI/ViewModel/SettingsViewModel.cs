using Mirage.ViewModel;
using Riff.Authentication;
using Riff.Data;
using Riff.UWP.Storage;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Riff.UWP.ViewModel
{
    public sealed class SettingsViewModel : DataViewModel
    {
        private readonly ILoginManager loginManager;
        private readonly IAppPreferences preferences;
        private UserProfile user;
        private ImageSource userPhoto;
        private bool isLoggedIn = false;

        public SettingsViewModel(ILoginManager loginManager, IAppPreferences preferences)
        {
            this.loginManager = loginManager ?? throw new ArgumentNullException(nameof(loginManager));
            this.preferences = preferences ?? throw new ArgumentNullException(nameof(preferences));
        }

        public bool IsLoggedIn
        {
            get => isLoggedIn;
            private set => SetProperty(ref this.isLoggedIn, value);
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

        public Theme AppTheme
        {
            get => this.preferences.AppTheme;
            set
            {
                if (this.preferences.AppTheme != value)
                {
                    this.preferences.AppTheme = value;
                    RaisePropertyChanged(nameof(AppTheme));
                }
            }
        }

        public async override Task LoadAsync()
        {
            IsLoading = true;

            // Check if login exists
            IsLoggedIn = await loginManager.LoginExistsAsync();

            if (IsLoggedIn)
            {
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
            }

            IsLoading = false;
            IsLoaded = true;
        }

        public async void LoginAsync()
        {
            await this.loginManager.LoginAsync(null);
            await LoadAsync();
        }

        public async Task SignOutAsync()
        {
            await this.loginManager.SignOutAsync();

            IsLoggedIn = await this.loginManager.LoginExistsAsync();
        }
    }
}

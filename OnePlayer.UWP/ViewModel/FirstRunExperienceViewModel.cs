using OnePlayer.Authentication;
using OnePlayer.Data;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OnePlayer.UWP.ViewModel
{
    public sealed class FirstRunExperienceViewModel : DataViewModel<VoidType>
    {
        private readonly ILoginManager loginManager;
        private bool loginRequired = false;
        private string providerUrl;
        private bool profileAndPhotoFetched = false;
        private UserProfile profile = null;
        private Stream photo = null;

        public FirstRunExperienceViewModel(ILoginManager loginManager)
        {
            this.loginManager = loginManager ?? throw new ArgumentNullException(nameof(loginManager));
            ProviderUrl = this.loginManager.GetAuthorizeUrl();
        }

        public override async Task LoadAsync(VoidType parameter)
        {
            LoginRequired = !(await loginManager.LoginExistsAsync());
        }

        public bool LoginRequired
        {
            get { return this.loginRequired; }
            private set
            {
                SetProperty(ref this.loginRequired, value);
            }
        }

        public string ProviderUrl
        {
            get
            {
                return this.providerUrl;
            }
            private set
            {
                SetProperty(ref this.providerUrl, value);
            }
        }

        public bool ProfileAndPhotoFetched
        {
            get
            {
                return this.profileAndPhotoFetched;
            }
            private set
            {
                SetProperty(ref this.profileAndPhotoFetched, value);
            }
        }

        public UserProfile Profile
        {
            get
            {
                return this.profile;
            }
            private set
            {
                SetProperty(ref this.profile, value);
            }
        }

        public Stream Photo
        {
            get
            {
                return this.photo;
            }
            private set
            {
                SetProperty(ref this.photo, value);
            }
        }

        public async Task CompleteLoginAsync(object providerCommand)
        {
            IsLoading = true;
            await this.loginManager.EndLoginAsync(providerCommand);
            LoginRequired = false;
            Profile = await this.loginManager.GetUserAsync();
            Photo = await this.loginManager.GetUserPhotoAsync();
            ProfileAndPhotoFetched = true;
            IsLoading = false;
        }
    }
}

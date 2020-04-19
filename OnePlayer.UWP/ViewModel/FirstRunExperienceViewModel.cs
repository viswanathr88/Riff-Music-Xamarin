using OnePlayer.Authentication;
using System;
using System.Threading.Tasks;

namespace OnePlayer.UWP.ViewModel
{
    public sealed class FirstRunExperienceViewModel : DataViewModel<VoidType>
    {
        private readonly ILoginManager loginManager;
        private bool loginRequired = false;
        private string providerUrl;

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

        public async Task CompleteLoginAsync(string providerId)
        {
            await this.loginManager.EndLoginAsync(providerId);
        }
    }
}

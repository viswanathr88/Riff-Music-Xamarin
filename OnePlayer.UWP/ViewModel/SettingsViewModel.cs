using OnePlayer.Authentication;
using System;
using System.Threading.Tasks;

namespace OnePlayer.UWP.ViewModel
{
    public sealed class SettingsViewModel : DataViewModel<VoidType>
    {
        private readonly ILoginManager loginManager;

        public SettingsViewModel(ILoginManager loginManager)
        {
            this.loginManager = loginManager ?? throw new ArgumentNullException(nameof(loginManager));
        }

        public override Task LoadAsync(VoidType parameter)
        {
            return Task.CompletedTask;
        }

        public Task SignOutAsync()
        {
            return this.loginManager.SignOutAsync();
        }
    }
}

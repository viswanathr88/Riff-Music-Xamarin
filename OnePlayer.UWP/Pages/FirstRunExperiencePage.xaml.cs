using OnePlayer.UWP.ViewModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.ApplicationSettings;
using System;
using Windows.Security.Authentication.Web.Core;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace OnePlayer.UWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FirstRunExperiencePage : Page
    {
        public FirstRunExperiencePage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            AccountsSettingsPane.GetForCurrentView().AccountCommandsRequested += BuildPaneAsync;

            var vm = DataContext as FirstRunExperienceViewModel;
            await vm.LoadAsync(VoidType.Empty);

            // Check the value of LoginRequired now
            if (!vm.LoginRequired)
            {
                // Navigate to Main Page
                Frame.Navigate(typeof(MainPage), VoidType.Empty, new EntranceNavigationTransitionInfo());
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            AccountsSettingsPane.GetForCurrentView().AccountCommandsRequested -= BuildPaneAsync;
        }

        private async void BuildPaneAsync(AccountsSettingsPane sender, AccountsSettingsPaneCommandsRequestedEventArgs args)
        {
            var vm = DataContext as FirstRunExperienceViewModel;
            var deferral = args.GetDeferral();

            var msaProvider = await WebAuthenticationCoreManager.FindAccountProviderAsync(
                vm.ProviderUrl, "consumers");

            var command = new WebAccountProviderCommand(msaProvider, GetMsaTokenAsync);

            args.WebAccountProviderCommands.Add(command);

            deferral.Complete();
        }

        private async void GetMsaTokenAsync(WebAccountProviderCommand command)
        {
            var vm = DataContext as FirstRunExperienceViewModel;
            await vm.CompleteLoginAsync(command.WebAccountProvider.Id);
        }

        private void LoginButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            AccountsSettingsPane.Show();
        }
    }
}

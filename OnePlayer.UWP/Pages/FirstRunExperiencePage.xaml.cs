using OnePlayer.UWP.ViewModel;
using System;
using Windows.Security.Authentication.Web.Core;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace OnePlayer.UWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FirstRunExperiencePage : Page, ISupportViewModel<FirstRunExperienceViewModel>
    {
        public FirstRunExperienceViewModel ViewModel => DataContext as FirstRunExperienceViewModel;

        public FirstRunExperiencePage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            AccountsSettingsPane.GetForCurrentView().AccountCommandsRequested += BuildPaneAsync;

            await ViewModel.LoadAsync(VoidType.Empty);

            // Check the value of LoginRequired now
            if (!ViewModel.LoginRequired)
            {
                // Navigate to Main Page
                Frame.Navigate(typeof(MainPage), null , new EntranceNavigationTransitionInfo());
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
            AccountsSettingsPane.GetForCurrentView().AccountCommandsRequested -= BuildPaneAsync;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.ProfileAndPhotoFetched) && ViewModel.ProfileAndPhotoFetched)
            {
                // Navigate to Main Page
                Frame.Navigate(typeof(MainPage), null, new EntranceNavigationTransitionInfo());
            }
        }

        private async void BuildPaneAsync(AccountsSettingsPane sender, AccountsSettingsPaneCommandsRequestedEventArgs args)
        {
            var deferral = args.GetDeferral();

            // Add consumer account provider
            var msaProvider = await WebAuthenticationCoreManager.FindAccountProviderAsync(ViewModel.ProviderUrl, "consumers");
            args.WebAccountProviderCommands.Add(new WebAccountProviderCommand(msaProvider, GetMsaTokenAsync));

            deferral.Complete();
        }

        private async void GetMsaTokenAsync(WebAccountProviderCommand command)
        {
            try
            {
                await ViewModel.CompleteLoginAsync(command);
            }
            catch (Exception ex)
            {

            }
        }

        private void LoginButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            AccountsSettingsPane.Show();
        }
    }
}

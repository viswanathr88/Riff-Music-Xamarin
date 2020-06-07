using Riff.UWP.Storage;
using Riff.UWP.ViewModel;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Riff.UWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : NavViewPageBase, ISupportViewModel<SettingsViewModel>
    {
        public SettingsPage()
        {
            this.InitializeComponent();
            HeaderText = ResourceLoader.GetForCurrentView().GetString("SettingsPageHeader");
        }

        public SettingsViewModel ViewModel => DataContext as SettingsViewModel;

        public override IDataViewModel DataViewModel => ViewModel;

        private async void SignOutButton_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.SignOutAsync();
        }

        private bool IsChecked(Theme x, Theme y)
        {
            return x == y;
        }

        private void SetTheme(bool? value)
        {
            if (value.HasValue && value.Value)
            {
                if (ThemeSettingsWindowsDefaultButton.IsChecked.HasValue && ThemeSettingsWindowsDefaultButton.IsChecked.Value)
                {
                    ViewModel.AppTheme = Theme.Default;
                }
                else if (ThemeSettingsLightButton.IsChecked.HasValue && ThemeSettingsLightButton.IsChecked.Value)
                {
                    ViewModel.AppTheme = Theme.Light;
                }
                else if (ThemeSettingsDarkButton.IsChecked.HasValue && ThemeSettingsDarkButton.IsChecked.Value)
                {
                    ViewModel.AppTheme = Theme.Dark;
                }
            }
        }
    }
}

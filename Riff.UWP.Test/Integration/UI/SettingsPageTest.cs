using GalaSoft.MvvmLight.Ioc;
using Moq;
using Riff.Authentication;
using Riff.Data;
using Riff.UWP.Pages;
using Riff.UWP.Storage;
using Riff.UWP.Test.Infra;
using Riff.UWP.ViewModel;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Xunit;

namespace Riff.UWP.Test.UI
{
    [Collection("UITests")]
    public class SettingsPageTest : IAsyncLifetime, IDisposable
    {
        private readonly Mock<ILoginManager> mockLoginManager;
        private readonly Mock<IAppPreferences> mockPreferences;
        private readonly UITree view;

        public SettingsPageTest()
        {
            mockLoginManager = new Mock<ILoginManager>();
            mockPreferences = new Mock<IAppPreferences>();
            view = new UITree();

            SimpleIoc.Default.Register(() => mockLoginManager.Object);
            SimpleIoc.Default.Register(() => mockPreferences.Object);
            SimpleIoc.Default.Register<SettingsViewModel>();
        }

        public void Dispose()
        {
            SimpleIoc.Default.Unregister<ILoginManager>();
            SimpleIoc.Default.Unregister<IAppPreferences>();
            SimpleIoc.Default.Unregister<SettingsViewModel>();
            SimpleIoc.Default.Reset();
        }

        public async Task DisposeAsync()
        {
            await view.UnloadAllPages();
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        [Fact]
        public async Task Navigate_NotLoggedIn_ValidateSignInButtonExists()
        {
            mockLoginManager.Setup(manager => manager.LoginExistsAsync()).Returns(Task.FromResult(false));

            await view.LoadPage<SettingsPage>(null);
            await view.WaitForElement<Button>("LoginButton");
            Assert.True(await view.ElementExists<Button>("LoginButton"));
        }

        [Fact]
        public async Task Navigate_LoggedIn_ValidateLogOutButtonExists()
        {
            mockLoginManager.Setup(manager => manager.LoginExistsAsync()).Returns(Task.FromResult(true));
            mockLoginManager.Setup(manager => manager.GetUserAsync()).Returns(Task.FromResult(new UserProfile()));
            mockLoginManager.Setup(manager => manager.GetUserPhotoAsync()).Returns(Task.FromResult<Stream>(null));


            await view.LoadPage<SettingsPage>(null);
            await view.WaitForElement<Button>("SignOutButton");
            Assert.True(await view.ElementExists<Button>("SignOutButton"));
        }

        [Fact]
        public async Task Navigate_DarkTheme_ValidateDarkThemeIsChecked()
        {
            mockLoginManager.Setup(manager => manager.LoginExistsAsync()).Returns(Task.FromResult(false));
            mockPreferences.Setup(preferences => preferences.AppTheme).Returns(Theme.Dark);

            await view.LoadPage<SettingsPage>(null);
            await view.WaitForElementAndCondition<RadioButton>("ThemeSettingsDarkButton", button => button.IsChecked == true);
            await view.WaitForElementAndCondition<RadioButton>("ThemeSettingsLightButton", button => button.IsChecked == false);
            await view.WaitForElementAndCondition<RadioButton>("ThemeSettingsWindowsDefaultButton", button => button.IsChecked == false);
        }

        [Fact]
        public async Task Navigate_LightTheme_ValidateLightThemeIsChecked()
        {
            mockLoginManager.Setup(manager => manager.LoginExistsAsync()).Returns(Task.FromResult(false));
            mockPreferences.Setup(preferences => preferences.AppTheme).Returns(Theme.Light);

            await view.LoadPage<SettingsPage>(null);
            await view.WaitForElementAndCondition<RadioButton>("ThemeSettingsDarkButton", button => button.IsChecked == false);
            await view.WaitForElementAndCondition<RadioButton>("ThemeSettingsLightButton", button => button.IsChecked == true);
            await view.WaitForElementAndCondition<RadioButton>("ThemeSettingsWindowsDefaultButton", button => button.IsChecked == false);
        }

        [Fact]
        public async Task Navigate_DefaultTheme_ValidateSystemThemeIsChecked()
        {
            mockLoginManager.Setup(manager => manager.LoginExistsAsync()).Returns(Task.FromResult(false));
            mockPreferences.Setup(preferences => preferences.AppTheme).Returns(Theme.Default);

            await view.LoadPage<SettingsPage>(null);
            await view.WaitForElementAndCondition<RadioButton>("ThemeSettingsDarkButton", button => button.IsChecked == false);
            await view.WaitForElementAndCondition<RadioButton>("ThemeSettingsLightButton", button => button.IsChecked == false);
            await view.WaitForElementAndCondition<RadioButton>("ThemeSettingsWindowsDefaultButton", button => button.IsChecked == true);
        }
    }
}

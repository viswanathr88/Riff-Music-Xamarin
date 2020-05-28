using OnePlayer.UWP.Pages;
using OnePlayer.UWP.Storage;
using OnePlayer.UWP.ViewModel;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace OnePlayer.UWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            UpdateTheme(new AppPreferences().AppTheme);
        }

        public static void UpdateTheme(Theme appTheme)
        {
            App app = Application.Current as App;
            if (Window.Current.Content is FrameworkElement frameworkElement)
            {
                app.UpdateContentTheme(appTheme);
            }
            else
            {
                app.UpdateAppTheme(appTheme);
            }
        }

        private void UpdateAppTheme(Theme appTheme)
        {
            switch (appTheme)
            {
                case Theme.Dark:
                    Application.Current.RequestedTheme = ApplicationTheme.Dark;
                    break;
                case Theme.Light:
                    Application.Current.RequestedTheme = ApplicationTheme.Light;
                    break;
                default:
                    break;
            }
        }

        private void UpdateContentTheme(Theme appTheme)
        {
            if (Window.Current.Content is FrameworkElement frameworkElement)
            {
                var elementTheme = ElementTheme.Default;
                switch (appTheme)
                {
                    case Theme.Default:
                        elementTheme = ElementTheme.Default;
                        break;
                    case Theme.Light:
                        elementTheme = ElementTheme.Light;
                        break;
                    case Theme.Dark:
                        elementTheme = ElementTheme.Dark;
                        break;
                }

                frameworkElement.RequestedTheme = elementTheme;
            }
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (!(Window.Current.Content is Frame rootFrame))
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                ApplicationView.GetForCurrentView().SetPreferredMinSize(new Windows.Foundation.Size(350, 300));
                CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
                CoreApplication.GetCurrentView().TitleBar.LayoutMetricsChanged += async(sender, arg) =>
                {
                    if (rootFrame.Content == null)
                    {
                        // When the navigation stack isn't restored navigate to the first page,
                        // configuring the new page by passing required information as a navigation
                        // parameter
                        var locator = Resources["VMLocator"] as Locator;

                        if (!await locator.LoginManager.LoginExistsAsync())
                        {
                            rootFrame.Navigate(typeof(FirstRunExperiencePage), e.Arguments, null);
                        }
                        else
                        {
                            rootFrame.Navigate(typeof(MainPage), null, new EntranceNavigationTransitionInfo());
                        }
                    }
                };
            }


            // Ensure the current window is active
            Window.Current.Activate();

            UpdateTitlebarColors();
        }

        private void UpdateTitlebarColors()
        {
            var titlebar = ApplicationView.GetForCurrentView().TitleBar;
            titlebar.ButtonBackgroundColor = Windows.UI.Colors.Transparent;
            titlebar.ButtonInactiveBackgroundColor = Windows.UI.Colors.Transparent;
            titlebar.ButtonForegroundColor = (Windows.UI.Color)Resources["SystemBaseHighColor"];
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}

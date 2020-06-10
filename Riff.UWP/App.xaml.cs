using Riff.UWP.Pages;
using Riff.UWP.Storage;
using Riff.UWP.ViewModel;
using System;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace Riff.UWP
{
    public enum ErrorCommand
    {
        CopyErrorInfo
    };

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
            UnhandledException += App_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            RequiresPointerMode = ApplicationRequiresPointerMode.WhenRequested;

            if (Device.Family == DeviceFamily.Xbox)
            {
                Application.Current.FocusVisualKind = FocusVisualKind.Reveal;
            }

            Device.UpdateTheme(new AppPreferences().AppTheme);
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (!(Window.Current.Content is Frame rootFrame))
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                rootFrame.Background = new SolidColorBrush(new Windows.UI.Color() { R = 80, G = 102, B = 88, A = 255 });

                rootFrame.Navigated += RootFrame_Navigated;
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
                if (rootFrame.Content == null)
                {
                    Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;
                    
                    // On platforms that paint a titlebar, wait for it's height to be determined before initializing the app to prevent jumpy behavior
                    if (Device.Family == DeviceFamily.Desktop)
                    {
                        ApplicationView.GetForCurrentView().SetPreferredMinSize(new Windows.Foundation.Size(350, 300));
                        CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
                        CoreApplication.GetCurrentView().TitleBar.LayoutMetricsChanged += async (sender, arg) => await LoadInitialPageAsync(e, rootFrame);
                    }
                    else
                    {
                        await LoadInitialPageAsync(e, rootFrame);
                    }
                }
            }


            // Ensure the current window is active
            Window.Current.Activate();

            UpdateTitlebarColors();
        }

        private void RootFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.SourcePageType == typeof(MainPage) || e.SourcePageType == typeof(FirstRunExperiencePage))
            {
                Frame frame = sender as Frame;
                frame.Background = null;
                frame.Navigated -= RootFrame_Navigated;
            }
        }

        private async Task LoadInitialPageAsync(LaunchActivatedEventArgs e, Frame rootFrame)
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

        private void App_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (Window.Current.Content is Frame rootFrame)
            {
                if (rootFrame.Content is ShellPageBase shell)
                {
                    if (shell.CanGoBack)
                    {
                        shell.GoBack();
                        e.Handled = true;
                        return;
                    }
                }
            }

            e.Handled = false;
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

        private async void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            await ShowErrorDialog(e.Exception, string.Empty);
        }

        private async void App_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            await ShowErrorDialog(e.Exception, e.Message);
        }

        private async Task ShowErrorDialog(Exception ex, string message)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Please file an issue with the following diagnostic data:");
            builder.AppendLine("Message: ");
            builder.AppendLine(ex.Message);
            builder.AppendLine("Exception:");
            builder.AppendLine(ex.StackTrace?.ToString());
            builder.AppendLine("Inner Exception:");
            builder.AppendLine(ex.InnerException?.ToString());

            // Create a command for copying error information
            var command = new UICommand("Copy error info", CopyErrorInfoInvoked, builder.ToString());

            MessageDialog dialog = new MessageDialog(builder.ToString(), "Oops. Something went wrong");
            dialog.Commands.Add(command);
            await dialog.ShowAsync();
        }

        private void CopyErrorInfoInvoked(IUICommand command)
        {
            DataPackage package = new DataPackage();
            package.RequestedOperation = DataPackageOperation.Copy;
            package.SetText(command.Id.ToString());
            Clipboard.SetContent(package);
        }
    }
}

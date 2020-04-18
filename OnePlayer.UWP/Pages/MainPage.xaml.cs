using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace OnePlayer.UWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly List<(string Tag, Type Page)> pages = new List<(string Tag, Type Page)>
        {
            ("home", typeof(HomePage)),
            ("library", typeof(MusicLibraryPage)),
            ("nowplaying", typeof(NowPlayingPage)),
            ("playlists", typeof(PlaylistsPage)),
            ("settings", typeof(SettingsPage)),
            ("syncstatus", typeof(SyncStatusPage)),
        };

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            NavView.SelectedItem = NavView.MenuItems[1];
            NavView_Navigate(pages[0].Tag, new EntranceNavigationTransitionInfo());
        }

        private void NavView_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            string tag = args.IsSettingsInvoked ? "settings" : (string)args.InvokedItemContainer?.Tag;
            NavView_Navigate(tag, args.RecommendedNavigationTransitionInfo);
        }

        private void NavView_Navigate(string tag, NavigationTransitionInfo transitionInfo)
        {
            if (string.IsNullOrEmpty(tag))
            {
                // Do nothing;
                return;
            }

            var item = pages.FirstOrDefault(p => p.Tag.Equals(tag));
            var page = item.Page;

            // Get the page type before navigation so you can prevent duplicate
            // entries in the backstack.
            var currentPageType = ContentFrame.CurrentSourcePageType;
            if (page != currentPageType)
            {
                ContentFrame.Navigate(page, null, transitionInfo);
            }
        }

        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            var pageItem = pages.FirstOrDefault(p => p.Page == e.SourcePageType);

            if (e.SourcePageType == typeof(SettingsPage))
            {
                // Set name for the settings item
                var settingsMenuItem = (Microsoft.UI.Xaml.Controls.NavigationViewItem)NavView.SettingsItem;
                settingsMenuItem.Content = ResourceLoader.GetForCurrentView().GetString("NavViewSettingsContent");
                NavView.SelectedItem = settingsMenuItem;
            }
            else
            {
                NavView.SelectedItem = NavView.MenuItems
                    .OfType<Microsoft.UI.Xaml.Controls.NavigationViewItem>()
                    .FirstOrDefault(n => n.Tag.Equals(pageItem.Tag));
            }

            var headerText = ((Microsoft.UI.Xaml.Controls.NavigationViewItem)NavView.SelectedItem)?.Content?.ToString();
            NavViewHeaderText.Text = headerText ?? string.Empty;
        }

        private void Titlebar_BackRequested(object sender, EventArgs e)
        {
            if (ContentFrame.CanGoBack)
            {
                ContentFrame.GoBack();
            }
        }
    }
}

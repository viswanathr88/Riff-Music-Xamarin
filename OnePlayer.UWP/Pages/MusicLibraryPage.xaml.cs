using OnePlayer.UWP.ViewModel;
using System;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace OnePlayer.UWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MusicLibraryPage : NavViewPageBase, ISupportViewModel<MusicLibraryViewModel>
    {
        public MusicLibraryViewModel ViewModel => (App.Current.Resources["VMLocator"] as Locator).MusicLibrary;

        public MusicLibraryPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            Loaded += MusicLibraryPage_Loaded;
            HeaderText = ResourceLoader.GetForCurrentView().GetString("MusicLibraryPageHeader");
        }

        public LibraryPageBase ChildPage => (LibraryContentFrame.Content as LibraryPageBase);

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (!ViewModel.IsLoaded)
            {
                await ViewModel.LoadAsync(VoidType.Empty);
            }
        }

        private void MusicLibraryPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Simulate the first entry in the filter being selected
            RadioMenuFlyoutItem_Click(ToolbarFilterFlyout.Items.First(), new RoutedEventArgs());

            // Simulate the first entry in display mode being selected;
            DisplayModeFlyoutItem_Click(ToolbarDisplayModeFlyout.Items.First(), new RoutedEventArgs());
        }

        private void RadioMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as Microsoft.UI.Xaml.Controls.RadioMenuFlyoutItem;
            item.IsChecked = true;
            if (item != null)
            {
                var fontIcon = item.Icon as FontIcon;
                ToolbarFilter.Icon = new FontIcon() { Glyph = fontIcon.Glyph };
                ToolbarFilter.Label = item.Text;
                ToolbarText.Text = item.Text;
            }

            Type pageType;
            switch (item.Tag.ToString())
            {
                case "albums":
                    pageType = typeof(AlbumsPage);
                    break;
                case "artists":
                    pageType = typeof(ArtistsPage);
                    break;
                case "genres":
                    pageType = typeof(GenresPage);
                    break;
                case "tracks":
                    pageType = typeof(TracksPage);
                    break;
                default:
                    pageType = null;
                    break;
            }

            if (pageType != null && LibraryContentFrame.Content?.GetType() != pageType)
            {
                LibraryContentFrame.Navigate(pageType, null, new EntranceNavigationTransitionInfo());
            }
        }

        private void LibraryContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DisplayModeFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Microsoft.UI.Xaml.Controls.RadioMenuFlyoutItem);
            if (item != null)
            {
                item.IsChecked = true;
                var fontIcon = item.Icon as FontIcon;
                ToolbarDisplayMode.Icon = new FontIcon() { Glyph = fontIcon.Glyph };
                ToolbarDisplayMode.Label = item.Text;
            }
        }
    }
}

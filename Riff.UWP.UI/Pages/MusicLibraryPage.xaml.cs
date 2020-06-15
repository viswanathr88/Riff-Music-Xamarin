using Riff.Data;
using Riff.UWP.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Riff.UWP.Pages
{
    public class MusicLibraryPageBase : NavViewPageBase<MusicLibraryViewModel>
    {
    }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MusicLibraryPage : MusicLibraryPageBase
    {
        public MusicLibraryPage()
        {
            this.InitializeComponent();
            PreferViewUpdateBeforeLoad = true;
            NavigationCacheMode = NavigationCacheMode.Enabled;
            Loaded += MusicLibraryPage_Loaded;
            HeaderText = ResourceLoader.GetForCurrentView().GetString("MusicLibraryPageHeader");
        }

        private void MusicLibraryPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!ToolbarFilterAlbums.IsChecked && !ToolbarFilterArtists.IsChecked && !ToolbarFilterTracks.IsChecked)
            {
                // Simulate the first entry in the filter being selected
                RadioMenuFlyoutItem_Click(ToolbarFilterFlyout.Items.First(), new RoutedEventArgs());
            }
        }

        private void RadioMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as Microsoft.UI.Xaml.Controls.RadioMenuFlyoutItem;
            item.IsChecked = true;
            if (item != null)
            {
                ToolbarFilter.Content = item.Text;
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
                case "tracks":
                    pageType = typeof(TracksPage);
                    break;
                default:
                    throw new Exception("Unknown library page");
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
    }
}

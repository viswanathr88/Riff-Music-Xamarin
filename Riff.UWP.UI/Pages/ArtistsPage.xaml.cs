using Riff.UWP.ViewModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Riff.UWP.Pages
{
    public class ArtistsPageBase : LibraryPageBase<ArtistsViewModel>
    {

    }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ArtistsPage : ArtistsPageBase
    {
        public ArtistsPage()
        {
            this.InitializeComponent();
            RegisterForChanges = true;
        }

        protected async override void HandleViewModelPropertyChanged(string propertyName)
        {
            base.HandleViewModelPropertyChanged(propertyName);

            if (propertyName == nameof(ViewModel.IsLoaded) && !ViewModel.IsLoaded)
            {
                await ViewModel.LoadAsync();
            }
        }

        public static string FormatAlbumsCount(int albumsCount)
        {
            return string.Format(albumsCount == 1 ? Strings.Resources.ArtistsPageOneAlbumFormat : Strings.Resources.ArtistsPageMultipleAlbumFormat, albumsCount);
        }

        private void ArtistList_ItemClick(object sender, Windows.UI.Xaml.Controls.ItemClickEventArgs e)
        {
            if (e.ClickedItem != null)
            {
                var artistVM = e.ClickedItem as ArtistItemViewModel;
                var parentFrame = VisualTreeHelperExtensions.FindParent<Frame>(Frame, "ContentFrame");
                parentFrame.Navigate(typeof(ArtistPage), artistVM.Item, new EntranceNavigationTransitionInfo());
            }
        }

        private void Grid_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is ArtistItemViewModel itemVM)
            {
                itemVM.IsPointerOver = true;
            }
        }

        private void Grid_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is ArtistItemViewModel itemVM)
            {
                itemVM.IsPointerOver = false;
            }
        }

        private void ArtistItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count > 0)
            {
                foreach (var item in e.RemovedItems)
                {
                    if (item is ArtistItemViewModel itemVM)
                    {
                        itemVM.IsSelected = false;
                    }
                }
            }

            if (e.AddedItems.Count > 0)
            {
                foreach (var item in e.AddedItems)
                {
                    if (item is ArtistItemViewModel itemVM)
                    {
                        itemVM.IsSelected = true;
                    }
                }
            }
        }
    }
}

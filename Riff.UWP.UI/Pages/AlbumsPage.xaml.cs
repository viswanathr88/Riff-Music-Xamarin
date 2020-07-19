using Riff.UWP.ViewModel;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Riff.UWP.Pages
{
    public class AlbumsPageBase : LibraryPageBase<AlbumsViewModel>
    {

    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AlbumsPage : AlbumsPageBase
    {
        private AlbumItemViewModel _storedItem = null;
        public AlbumsPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            RegisterForChanges = true;
            PreferViewUpdateBeforeLoad = true;
        }

        protected async override void HandleViewModelPropertyChanged(string propertyName)
        {
            base.HandleViewModelPropertyChanged(propertyName);

            if (propertyName == nameof(ViewModel.IsLoaded) && !ViewModel.IsLoaded)
            {
                await ViewModel.LoadAsync();
            }
        }

        private async void AlbumItems_Loaded(object sender, RoutedEventArgs e)
        {
            if (_storedItem != null)
            {
                AlbumItems.ScrollIntoView(_storedItem, ScrollIntoViewAlignment.Default);
                AlbumItems.UpdateLayout();

                var animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("backAnimation");
                if (animation != null)
                {
                    await AlbumItems.TryStartConnectedAnimationAsync(animation, _storedItem, "AlbumArt");
                }

                _storedItem.IsSelected = false;
                _storedItem = null;
            }
        }      

        private async void SortFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.ReloadAsync();
        }

        private async void SortOrderFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.ReloadAsync();
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem != null)
            {
                _storedItem = e.ClickedItem as AlbumItemViewModel;
                _ = AlbumItems.PrepareConnectedAnimation("ca1", _storedItem, "AlbumArt");

                var parentFrame = VisualTreeHelperExtensions.FindParent<Frame>(Frame, "ContentFrame");
                parentFrame.Navigate(typeof(AlbumPage), _storedItem.Item, new SuppressNavigationTransitionInfo());
            }
        }

        private void AlbumItem_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is AlbumItemViewModel itemVM)
            {
                itemVM.IsPointerOver = true;
            }
        }

        private void AlbumItem_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is AlbumItemViewModel itemVM)
            {
                itemVM.IsPointerOver = false;
            }
        }

        private void AlbumItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count > 0)
            {
                foreach (var item in e.RemovedItems)
                {
                    if (item is AlbumItemViewModel itemVM)
                    {
                        itemVM.IsSelected = false;
                    }
                }
            }

            if (e.AddedItems.Count > 0)
            {
                foreach (var item in e.AddedItems)
                {
                    if (item is AlbumItemViewModel itemVM)
                    {
                        itemVM.IsSelected = true;
                    }
                }
            }
        }

        private void BrowseArtistContextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AlbumItemViewModel itemVM = null;
            if (sender is MenuFlyoutItem flyoutItem && flyoutItem.CommandParameter != null)
            {
                itemVM = flyoutItem.CommandParameter as AlbumItemViewModel;
            }
            else if (sender is FrameworkElement element && element.DataContext != null)
            {
                itemVM = element.DataContext as AlbumItemViewModel;
            }

            var parentFrame = VisualTreeHelperExtensions.FindParent<Frame>(Frame, "ContentFrame");
            parentFrame.Navigate(typeof(ArtistPage), itemVM.Item.Artist, new EntranceNavigationTransitionInfo());
        }
    }
}

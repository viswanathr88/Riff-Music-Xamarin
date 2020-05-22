using Microsoft.UI.Xaml.Controls;
using OnePlayer.Data;
using OnePlayer.Data.Access;
using OnePlayer.UWP.ViewModel;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace OnePlayer.UWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AlbumsPage : LibraryPageBase, ISupportViewModel<AlbumsViewModel>
    {
        private Album _storedItem = null;
        public AlbumsPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private MusicLibrary Library => (Application.Current.Resources["VMLocator"] as Locator).Library;

        public AlbumsViewModel ViewModel => (Application.Current.Resources["VMLocator"] as Locator).MusicLibrary.Albums;

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (!ViewModel.IsLoaded)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async() => await ViewModel.LoadAsync(VoidType.Empty));
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
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

                _storedItem = null;
            }
        }

        private void GridView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.InRecycleQueue)
            {
                var templateRoot = args.ItemContainer.ContentTemplateRoot as FrameworkElement;
                var image = (Image)templateRoot.FindName("AlbumArt");

                image.Source = null;
            }

            if (args.Phase == 0)
            {
                args.RegisterUpdateCallback(2, LoadImageAsync);
                args.Handled = true;
            }
        }

        private async void LoadImageAsync(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.Phase == 2)
            {
                var templateRoot = args.ItemContainer.ContentTemplateRoot as FrameworkElement;
                var image = (Image)templateRoot.FindName("AlbumArt");

                image.Opacity = 1;

                var item = ViewModel.Items[args.ItemIndex] as Album;
                if (item != null)
                {
                    await LoadArtAsync(image, item);
                }
            }
        }

        private async Task<bool> LoadArtAsync(Image image, Album album)
        {
            bool loaded = false;
            ThumbnailSize artSize = ThumbnailSize.Medium;
            if (Library.AlbumArts.Exists(album.Id.Value, artSize))
            {
                using (var stream = Library.AlbumArts.Get(album.Id.Value, artSize))
                {
                    using (var rtStream = stream.AsRandomAccessStream())
                    {
                        var bm = new BitmapImage();
                        await bm.SetSourceAsync(rtStream);
                        image.Source = bm;
                        loaded = true;
                    }
                }
            }

            return loaded;
        }

        private async void SortFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadAsync(VoidType.Empty);
        }

        private async void SortOrderFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadAsync(VoidType.Empty);
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem != null)
            {
                _storedItem = e.ClickedItem as Album;
                var animation = AlbumItems.PrepareConnectedAnimation("ca1", _storedItem, "AlbumArt");

                var parentFrame = VisualTreeHelperExtensions.FindParent<Frame>(Frame, "ContentFrame");
                parentFrame.Navigate(typeof(AlbumPage), _storedItem, new SuppressNavigationTransitionInfo());
            }
        }

        private void AlbumGridItem_Loaded(object sender, RoutedEventArgs e)
        {
            var root = (UIElement)sender;
            InitializeAnimation(VisualTreeHelperExtensions.FindVisualChild<Border>(root));
        }

        private void InitializeAnimation(UIElement root)
        {
            var rootVisual = ElementCompositionPreview.GetElementVisual(root);
            var compositor = rootVisual.Compositor;

            // Create animation to scale up the rectangle
            var pointerEnteredAnimation = compositor.CreateVector3KeyFrameAnimation();
            pointerEnteredAnimation.InsertKeyFrame(1.0f, new System.Numerics.Vector3(1.05f));

            // Create animation to scale the rectangle back down
            var pointerExitedAnimation = compositor.CreateVector3KeyFrameAnimation();
            pointerExitedAnimation.InsertKeyFrame(1.0f, new System.Numerics.Vector3(1.0f));

            // Play animations on pointer enter and exit
            root.PointerEntered += (sender, args) =>
            {
                rootVisual.CenterPoint = new System.Numerics.Vector3(rootVisual.Size / 2, 0);
                rootVisual.StartAnimation("Scale", pointerEnteredAnimation);
            };

            root.PointerExited += (sender, args) => rootVisual.StartAnimation("Scale", pointerExitedAnimation);
        }
    }
}

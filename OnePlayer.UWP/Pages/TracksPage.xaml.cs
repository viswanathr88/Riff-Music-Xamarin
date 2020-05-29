using OnePlayer.Data;
using OnePlayer.UWP.ViewModel;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace OnePlayer.UWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TracksPage : LibraryPageBase, ISupportViewModel<TracksViewModel>
    {
        public TracksPage()
        {
            this.InitializeComponent();
        }

        public TracksViewModel ViewModel => (Application.Current.Resources["VMLocator"] as Locator).MusicLibrary.Tracks;
        public MusicLibrary Library => (Application.Current.Resources["VMLocator"] as Locator).Library;

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;

            if (!ViewModel.IsLoaded)
            {
                await ViewModel.LoadAsync();
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        private async void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.IsLoaded) && !ViewModel.IsLoaded)
            {
                await ViewModel.LoadAsync();
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

        private void TracksList_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void TracksList_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.InRecycleQueue)
            {
                var templateRoot = args.ItemContainer.ContentTemplateRoot as FrameworkElement;
                var image = (Image)templateRoot.FindName("TrackArt");

                if (image != null)
                {
                    image.Source = null;
                }
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
                var image = (Image)templateRoot.FindName("TrackArt");

                if (image != null)
                {
                    image.Opacity = 1;

                    var item = ViewModel.Tracks[args.ItemIndex] as Track;
                    if (item != null)
                    {
                        await LoadArtAsync(image, item);
                    }
                }
            }
        }

        private async Task<bool> LoadArtAsync(Image image, Track track)
        {
            bool loaded = false;
            if (Library.AlbumArts.Exists(track.Album.Id.Value))
            {
                using (var stream = Library.AlbumArts.Get(track.Album.Id.Value))
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
    }
}

using OnePlayer.Data;
using OnePlayer.UWP.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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

            if (!ViewModel.IsLoaded)
            {
                await ViewModel.LoadAsync(VoidType.Empty);
            }
        }

        private async void SortFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadAsync(VoidType.Empty);
        }

        private async void SortOrderFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.LoadAsync(VoidType.Empty);
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
            ThumbnailSize artSize = ThumbnailSize.Medium;
            if (Library.TrackArts.Exists(track.Id.Value, artSize))
            {
                using (var stream = Library.TrackArts.Get(track.Id.Value, artSize))
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

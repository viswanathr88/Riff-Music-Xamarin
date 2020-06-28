using CommonServiceLocator;
using Riff.Data;
using Riff.UWP.ViewModel;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Riff.UWP.Pages
{
    public class TracksPageBase : LibraryPageBase<TracksViewModel>
    {
    }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TracksPage : TracksPageBase, ISupportPlaying
    {
        public TracksPage()
        {
            this.InitializeComponent();
            RegisterForChanges = true;
        }

        public IMusicLibrary Library => ServiceLocator.Current.GetInstance<IMusicLibrary>();

        public PlayerViewModel Player => ServiceLocator.Current.GetInstance<PlayerViewModel>();

        protected async override void HandleViewModelPropertyChanged(string propertyName)
        {
            base.HandleViewModelPropertyChanged(propertyName);
            if (propertyName == nameof(ViewModel.IsLoaded) && !ViewModel.IsLoaded)
            {
                await ViewModel.LoadAsync();
            }
            else if (propertyName == nameof(ViewModel.CurrentIndex))
            {
                await Player.PlayAsync(ViewModel.Tracks, Convert.ToUInt32(ViewModel.CurrentIndex));
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

        private async void Track_Selected(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                await Player.PlayAsync(ViewModel.Tracks, Convert.ToUInt32(ViewModel.CurrentIndex));
            }
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

                    if (ViewModel.Tracks[args.ItemIndex] is DriveItem item)
                    {
                        await LoadArtAsync(image, item.Track);
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

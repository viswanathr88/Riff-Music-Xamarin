using OnePlayer.Data;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace OnePlayer.UWP.ViewModel
{
    public sealed class SearchItemViewModel : ViewModelBase
    {
        private readonly MusicLibrary library;
        private readonly SearchItem item;
        private BitmapImage image = null;

        public SearchItemViewModel(MusicLibrary library, SearchItem item)
        {
            this.library = library;
            this.item = item;
        }

        public string Name => item.Name;

        public string Description => item.Description;

        public SearchItemType Type => item.Type;

        public ImageSource Art => image;

        public bool HasNoArt => !GetHasArt();

        private bool GetHasArt()
        {
            bool hasArt = false;
            if (item.Type == SearchItemType.Album || item.Type == SearchItemType.Track || item.Type == SearchItemType.TrackArtist)
            {
                var cache = item.Type == SearchItemType.Album ? library.AlbumArts : library.TrackArts;
                hasArt = cache.Exists(item.Id, ThumbnailSize.Small);
            }

            return hasArt;
        }

        public async Task LoadArtAsync()
        {
            if (item.Type == SearchItemType.Album || item.Type == SearchItemType.Track || item.Type == SearchItemType.TrackArtist)
            {
                var cache = item.Type == SearchItemType.Album ? library.AlbumArts : library.TrackArts;
                if (cache.Exists(item.Id, ThumbnailSize.Small))
                {
                    using (var stream = cache.Get(item.Id, ThumbnailSize.Small))
                    {
                        using (var rtStream = stream.AsRandomAccessStream())
                        {
                            if (this.image == null)
                            {
                                this.image = new BitmapImage();
                            }
                            await this.image.SetSourceAsync(rtStream);
                        }
                    }
                }
            }
        }
    }
}

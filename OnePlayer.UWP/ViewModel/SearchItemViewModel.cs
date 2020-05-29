using OnePlayer.Data;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace OnePlayer.UWP.ViewModel
{
    public sealed class SearchItemViewModel : ViewModelBase
    {
        private readonly MusicLibrary library;
        private readonly SearchItem item;
        private BitmapImage image = null;
        private ThumbnailSize size = ThumbnailSize.Medium;
        public SearchItemViewModel(MusicLibrary library, SearchItem item)
        {
            this.library = library;
            this.item = item;
        }

        public long Id => item.Id;

        public string Name => item.Name;

        public string Description => item.Description;

        public SearchItemType Type => item.Type;

        public ImageSource Art => image;

        public bool HasNoArt => !GetHasArt();

        private bool GetHasArt()
        {
            bool hasArt = false;
            long? id = (item.Type == SearchItemType.Album) ? item.Id : item.ParentId;
            if (id.HasValue && item.Type == SearchItemType.Album)
            {
                hasArt = library.AlbumArts.Exists(id.Value, size);
            }

            return hasArt;
        }

        public async Task LoadArtAsync()
        {
            long? id = (item.Type == SearchItemType.Album) ? item.Id : item.ParentId;
            if (id.HasValue)
            {
                if (library.AlbumArts.Exists(id.Value, size))
                {
                    using (var stream = library.AlbumArts.Get(id.Value, size))
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

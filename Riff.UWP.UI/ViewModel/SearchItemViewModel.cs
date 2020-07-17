using Mirage.ViewModel;
using Riff.Data;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Riff.UWP.ViewModel
{
    public sealed class SearchItemViewModel : ViewModelBase
    {
        private readonly IMusicLibrary library;
        private readonly SearchItem item;
        private BitmapImage image = null;
        public SearchItemViewModel(IMusicLibrary library, SearchItem item)
        {
            this.library = library;
            this.item = item;
        }

        public long Id => item.Id;

        public string Name => item.Name;

        public string Description => item.Description;

        public SearchItemType Type => item.Type;

        public long? ParentId => item.ParentId;

        public ImageSource Art => image;

        public bool HasNoArt => !GetHasArt();

        private bool GetHasArt()
        {
            bool hasArt = false;
            long? id = (item.Type == SearchItemType.Album) ? item.Id : item.ParentId;
            if (id.HasValue)
            {
                hasArt = library.AlbumArts.Exists(id.Value);
            }

            return hasArt;
        }

        public async Task LoadArtAsync()
        {
            long? id = (item.Type == SearchItemType.Album) ? item.Id : item.ParentId;
            if (id.HasValue && library.AlbumArts.Exists(id.Value))
            {
                using (var stream = library.AlbumArts.Get(id.Value))
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

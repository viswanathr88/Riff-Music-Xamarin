using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Media.Playback;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Riff.UWP.ViewModel
{
    public sealed class MediaListItem : ViewModelBase
    {
        private readonly MediaPlaybackItem item;
        private ImageSource image = null;
        private static readonly string AlbumArtProperty = "AlbumArtPath";
        private static readonly string YearProperty = "Year";

        public MediaListItem(MediaPlaybackItem item)
        {
            this.item = item;
        }

        public string Title => item.GetDisplayProperties().MusicProperties.Title;

        public string Artist => item.GetDisplayProperties().MusicProperties.Artist;

        public string Album => item.GetDisplayProperties().MusicProperties.AlbumTitle;

        public string AlbumArtist => item.GetDisplayProperties().MusicProperties.AlbumArtist;

        public int ReleaseYear => item.Source.CustomProperties.ContainsKey(YearProperty) ? (int)item.Source.CustomProperties[YearProperty] : 0;

        public ImageSource Art
        {
            get => image;
            private set => SetProperty(ref this.image, value);
        }

        public async Task LoadArtAsync()
        {
            var path = item.Source.CustomProperties.ContainsKey(AlbumArtProperty) ? (string)item.Source.CustomProperties[AlbumArtProperty] : string.Empty;
            if (!string.IsNullOrEmpty(path))
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    using (var rStream = stream.AsRandomAccessStream())
                    {
                        var bitmapImage = new BitmapImage()
                        {
                            DecodePixelHeight = 128,
                            DecodePixelWidth = 128,
                            DecodePixelType = DecodePixelType.Logical
                        };

                        await bitmapImage.SetSourceAsync(rStream);
                        Art = bitmapImage;
                    }
                }
            }
        }
    }
}

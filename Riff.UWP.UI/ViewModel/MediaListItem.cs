using Mirage.ViewModel;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Media.Playback;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Riff.UWP.ViewModel
{
    public sealed class MediaListItem : ViewModelBase, IMediaListItem
    {
        private readonly MediaPlaybackItem item;
        private ImageSource image = null;
        private static readonly string AlbumArtProperty = "AlbumArtPath";
        private static readonly string YearProperty = "Year";
        private static readonly string TrackIdProperty = "TrackId";
        private static readonly string ItemIdProperty = "ItemId";


        public MediaListItem(MediaPlaybackItem item)
        {
            this.item = item;
        }

        public string ItemId => item.Source.CustomProperties.ContainsKey(ItemIdProperty) ? (string)item.Source.CustomProperties[ItemIdProperty] : null;

        public long? TrackId => item.Source.CustomProperties.ContainsKey(ItemIdProperty) ? (long?)item.Source.CustomProperties[TrackIdProperty] : null;

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

using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace Riff.UWP.Converters
{
    public sealed class TaglessItemsConverter : IValueConverter
    {
        public TaglessItemsConverter()
        {

        }
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string str = (string)value;
            if (!string.IsNullOrEmpty(str))
            {
                return str;
            }

            string type = (string)parameter;
            string key;
            switch (type)
            {
                case "album":
                    key = "UnknownAlbumText";
                    break;
                case "artist":
                    key = "UnknownArtistText";
                    break;
                case "genre":
                    key = "UnknownGenreText";
                    break;
                case "track":
                    key = "UnknownTitleText";
                    break;
                default:
                    key = "UnknownText";
                    break;
            }

            return ResourceLoader.GetForCurrentView().GetString(key);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

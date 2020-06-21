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
            string result;
            switch (type)
            {
                case "album":
                    result = Strings.Resources.UnknownAlbumText;
                    break;
                case "artist":
                    result = Strings.Resources.UnknownArtistText;
                    break;
                case "genre":
                    result = Strings.Resources.UnknownGenreText;
                    break;
                case "track":
                    result = Strings.Resources.UnknownTitleText;
                    break;
                default:
                    result = Strings.Resources.UnknownText;
                    break;
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

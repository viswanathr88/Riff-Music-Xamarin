using System;
using System.IO;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace OnePlayer.UWP.Converters
{
    class ImageStreamConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            BitmapImage image = new BitmapImage()
            {
                // DecodePixelType = DecodePixelType.Logical,
                DecodePixelHeight = 32,
                DecodePixelWidth = 32
            };

            if (value != null && value is Stream)
            {
                using (Stream stream = value as Stream)
                {
                    using (var raStream = stream.AsRandomAccessStream())
                    {
                        image.SetSource(raStream);
                    }
                }
                    
            }

            return image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

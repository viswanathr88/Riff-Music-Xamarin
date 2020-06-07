using Riff.Data.Access;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Riff.UWP.Converters
{
    public sealed class AlbumSortTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string tag = parameter.ToString();
            AlbumSortType type = (AlbumSortType)value;
            if (tag == "sortByName" && type == AlbumSortType.Name)
            {
                return true;
            }
            else if (tag == "sortByReleaseYear" && type == AlbumSortType.ReleaseYear)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            string tag = parameter.ToString();
            bool isChecked = (bool)value;

            if (tag == "sortByName")
            {
                return isChecked ? AlbumSortType.Name : AlbumSortType.ReleaseYear;
            }
            else if (tag == "sortByReleaseYear")
            {
                return isChecked ? AlbumSortType.ReleaseYear : AlbumSortType.Name;
            }
            else
            {
                throw new Exception("Unexected album sort type tag");
            }
        }
    }
}

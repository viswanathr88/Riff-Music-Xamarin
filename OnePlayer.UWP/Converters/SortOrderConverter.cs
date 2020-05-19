using OnePlayer.Data;
using System;
using Windows.UI.Xaml.Data;

namespace OnePlayer.UWP.Converters
{
    public sealed class SortOrderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string tag = parameter.ToString();
            SortOrder order = (SortOrder)value;
            if (order == SortOrder.Ascending && tag == "sortOrderIncreasing")
            {
                return true;
            }
            else if (order == SortOrder.Descending && tag == "sortOrderDecreasing")
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

            if (tag == "sortOrderIncreasing")
            {
                return isChecked ? SortOrder.Ascending : SortOrder.Descending;
            }
            else if (tag == "sortOrderDecreasing")
            {
                return isChecked ? SortOrder.Descending : SortOrder.Ascending;
            }
            else
            {
                throw new Exception("Unknown Sort Order detected");
            }
        }
    }
}

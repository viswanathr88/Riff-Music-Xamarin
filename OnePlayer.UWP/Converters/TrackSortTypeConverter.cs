using OnePlayer.Data.Access;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace OnePlayer.UWP.Converters
{
    public sealed class TrackSortTypeConverter : IValueConverter
    {
        private TrackSortType lastCheckedValue;
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string tag = parameter.ToString();
            TrackSortType type = (TrackSortType)value;
            if (tag == "sortByTitle" && type == TrackSortType.Title)
            {
                return true;
            }
            else if (tag == "sortByTrackArtist" && type == TrackSortType.TrackArtist)
            {
                return true;
            }
            else if (tag == "sortByDuration" && type == TrackSortType.Duration)
            {
                return true;
            }
            else if (tag == "sortByReleaseYear" && type == TrackSortType.ReleaseYear)
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

            if (tag == "sortByTitle" && isChecked)
            {
                lastCheckedValue = TrackSortType.Title;
                return TrackSortType.Title;
            }
            else if (tag == "sortByTrackArtist" && isChecked)
            {
                lastCheckedValue = TrackSortType.TrackArtist;
                return TrackSortType.TrackArtist;
            }
            else if (tag == "sortByDuration" && isChecked)
            {
                lastCheckedValue = TrackSortType.Duration;
                return TrackSortType.Duration;
            }
            else if (tag == "sortByReleaseYear" && isChecked)
            {
                lastCheckedValue = TrackSortType.ReleaseYear;
                return TrackSortType.ReleaseYear;
            }
            else
            {
                return lastCheckedValue;
            }
        }
    }
}

using Riff.Sync;
using System;
using Windows.UI.Xaml.Data;

namespace Riff.UWP.Converters
{
    public sealed class SyncStatusIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string glyph = "";
            if (value is SyncState state)
            {
                switch (state)
                {
                    case SyncState.NotStarted:
                        glyph = "\uEA6A";
                        break;
                    case SyncState.Started:
                        glyph = "\uE895";
                        break;
                    case SyncState.Syncing:
                        glyph = "\uE895";
                        break;
                    case SyncState.NotSyncing:
                        glyph = "\uEA6A";
                        break;
                    case SyncState.Uptodate:
                        glyph = "\uE930";
                        break;
                    case SyncState.Stopped:
                        glyph = "\uE769";
                        break;
                    default:
                        glyph = "\uE895";
                        break;
                }
            }

            return glyph;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

using System.ComponentModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace Riff.UWP.ViewModel
{
    public interface IMediaListItem : INotifyPropertyChanged
    {
        string Album { get; }
        string AlbumArtist { get; }
        ImageSource Art { get; }
        string Artist { get; }
        string ItemId { get; }
        int ReleaseYear { get; }
        string Title { get; }
        long? TrackId { get; }
        Task LoadArtAsync();
    }
}

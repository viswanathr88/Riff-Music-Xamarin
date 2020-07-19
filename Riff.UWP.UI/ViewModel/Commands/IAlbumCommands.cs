using System.Windows.Input;

namespace Riff.UWP.ViewModel.Commands
{
    public interface IAlbumCommands
    {
        ICommand PlayAlbum { get; }

        ICommand AddToNowPlaying { get; }

        ICommand AddToPlaylist { get; }
    }
}

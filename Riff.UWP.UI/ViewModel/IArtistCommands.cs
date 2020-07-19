using Mirage.ViewModel.Commands;

namespace Riff.UWP.ViewModel
{
    public interface IArtistCommands
    {
        IAsyncCommand<ArtistItemViewModel> AddToPlaylist { get; }
        IAsyncCommand<ArtistItemViewModel> PlayArtist { get; }
        IAsyncCommand<ArtistItemViewModel> PlayArtistNext { get; }
    }
}
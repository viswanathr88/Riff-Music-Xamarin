using Mirage.ViewModel.Commands;

namespace Riff.UWP.ViewModel
{
    public interface IAlbumCommands
    {
        IAsyncCommand<AlbumItemViewModel> AddToPlaylistCommand { get; }
        IAsyncCommand<AlbumItemViewModel> PlayAlbumItem { get; }
        IAsyncCommand<AlbumItemViewModel> AddToNowPlayingCommand { get; }
    }
}
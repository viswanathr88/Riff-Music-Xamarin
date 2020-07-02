using System.Windows.Input;

namespace Riff.UWP.ViewModel.Commands
{
    public interface IPlaylistNameCommand : ICommand
    {
        string PlaylistName { get; set; }
    }
}

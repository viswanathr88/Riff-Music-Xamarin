using Mirage.ViewModel.Commands;
using Riff.Data;
using System.Windows.Input;

namespace Riff.UWP.ViewModel.Commands
{
    public interface IPlaylistNameCommand : ICommand
    {
        string PlaylistName { get; set; }
    }
}

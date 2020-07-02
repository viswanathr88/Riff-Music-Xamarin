using Riff.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Riff.UWP.ViewModel.Commands
{
    public sealed class DeletePlaylistCommand : ICommand
    {
        private readonly IPlaylistManager playlistManager;

        public DeletePlaylistCommand(IPlaylistManager playlistManager)
        {
            this.playlistManager = playlistManager;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (parameter != null && parameter is IList<object> items)
            {
                foreach (var item in items)
                {
                    playlistManager.DeletePlaylist((item as Playlist).Name);
                }
            }
            else if (parameter != null && parameter is Playlist playlist)
            {
                playlistManager.DeletePlaylist(playlist.Name);
            }
        }
    }
}

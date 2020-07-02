using Riff.Data;
using Riff.UWP.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Riff.UWP.ViewModel.Commands
{
    public sealed class RenamePlaylistCommand : ICommand
    {
        private readonly IPlaylistManager playlistManager;
        private bool canExecute = true;

        public RenamePlaylistCommand(IPlaylistManager playlistManager)
        {
            this.playlistManager = playlistManager;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return parameter != null && parameter is Playlist && CanExecuteCommand;
        }

        public string PlaylistName
        {
            get;
            set;
        }

        public void Execute(object parameter)
        {

            CanExecuteCommand = false;
            try
            {
                if (parameter != null && parameter is Playlist playlist && playlist.Name != PlaylistName)
                {
                    playlistManager.RenamePlaylist(playlist.Name, PlaylistName);
                }
            }
            catch (Exception)
            { }
            finally
            {
                PlaylistName = string.Empty;
                CanExecuteCommand = true;
            }
        }

        private bool CanExecuteCommand
        {
            get => canExecute;
            set
            {
                if (this.canExecute != value)
                {
                    canExecute = value;
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}

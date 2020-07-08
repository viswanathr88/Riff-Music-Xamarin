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
        private readonly IMusicLibrary musicLibrary;
        private bool canExecute = true;

        public RenamePlaylistCommand(IMusicLibrary musicLibrary)
        {
            this.musicLibrary = musicLibrary;
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
            using (var session = musicLibrary.Edit())
            {
                try
                {
                    if (parameter != null && parameter is Playlist playlist && playlist.Name != PlaylistName)
                    {
                        var clonedPlaylist = playlist.Clone();
                        clonedPlaylist.Name = PlaylistName;
                        session.Playlists.Update(clonedPlaylist);
                        session.Save();
                    }
                }
                catch (Exception)
                {
                    session.Revert();
                }
                finally
                {
                    PlaylistName = string.Empty;
                    CanExecuteCommand = true;
                }
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

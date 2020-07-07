using Riff.Data;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Riff.UWP.ViewModel.Commands
{
    public sealed class DeletePlaylistCommand : ICommand
    {
        private readonly IMusicLibrary musicLibrary;
        private bool canExecute = true;

        public DeletePlaylistCommand(IMusicLibrary musicLibrary)
        {
            this.musicLibrary = musicLibrary;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return CanExecuteCommand;
        }

        public void Execute(object parameter)
        {
            CanExecuteCommand = false;
            using (var session = musicLibrary.Edit())
            {
                if (parameter != null && parameter is IList<object> items)
                {
                    foreach (var item in items)
                    {
                        var playlist = item as Playlist2;
                        session.PlaylistItems.Delete(playlist);
                        session.Playlists.Delete(playlist.Id.Value);
                    }
                }
                else if (parameter != null && parameter is Playlist2 playlist)
                {
                    session.PlaylistItems.Delete(playlist);
                    session.Playlists.Delete(playlist.Id.Value);
                }

                session.Save();
            }
            CanExecuteCommand = true;
        }

        private bool CanExecuteCommand
        {
            get => canExecute;
            set
            {
                if (canExecute != value)
                {
                    this.canExecute = value;
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}

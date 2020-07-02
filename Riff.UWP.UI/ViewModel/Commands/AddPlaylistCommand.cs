using Riff.Data;
using System;
using System.Windows.Input;

namespace Riff.UWP.ViewModel.Commands
{
    public sealed class AddPlaylistCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private IPlaylistManager playlistManager;
        private bool canExecute = false;

        public AddPlaylistCommand(IPlaylistManager playlistManager)
        {
            this.playlistManager = playlistManager;
        }

        public bool CanExecute(object parameter)
        {
            var val = parameter is string content && !string.IsNullOrEmpty(content);
            if (val != canExecute)
            {
                canExecute = val;
                CanExecuteChanged.Invoke(this, EventArgs.Empty);
            }

            return val;
        }

        public void Execute(object parameter)
        {
            if (parameter is string content)
            {
                playlistManager.CreatePlaylist(content);
            }
        }
    }
}

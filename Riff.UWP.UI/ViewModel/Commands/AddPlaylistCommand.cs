using Riff.Data;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace Riff.UWP.ViewModel.Commands
{
    public sealed class AddPlaylistCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private IMusicLibrary musicLibrary;
        private bool canExecute = true;

        public AddPlaylistCommand(IMusicLibrary musicLibrary)
        {
            this.musicLibrary = musicLibrary;
        }

        public Exception Error
        {
            get;
            private set;
        }

        public string PlaylistName
        {
            get;
            set;
        }

        public bool CanExecuteCommand
        {
            get => canExecute;
            private set
            {
                if (this.canExecute != value)
                {
                    this.canExecute = value;
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteCommand;
        }

        public void Execute(object parameter)
        {
            Error = null;
            CanExecuteCommand = false;
            var playlist = new Playlist2()
            {
                Name = PlaylistName,
                LastModified = DateTime.Now
            };

            using (var editor = musicLibrary.Edit())
            {
                try
                {
                    editor.Playlists.Add(playlist);
                    editor.Save();
                }
                catch (Exception ex)
                {
                    Error = ex;
                    editor.Revert();
                }
                finally
                {
                    CanExecuteCommand = true;
                    PlaylistName = string.Empty;
                }
            }
        }
    }
}

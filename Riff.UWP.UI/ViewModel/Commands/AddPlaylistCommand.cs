using Mirage.ViewModel.Commands;
using Riff.Data;
using System;

namespace Riff.UWP.ViewModel.Commands
{
    public sealed class AddPlaylistCommand : Command
    {
        private IMusicLibrary musicLibrary;
        private string playlistName;

        public AddPlaylistCommand(IMusicLibrary musicLibrary)
        {
            this.musicLibrary = musicLibrary;
        }

        public string PlaylistName
        {
            get => playlistName;
            set
            {
                if (playlistName != value)
                {
                    playlistName = value;
                    EvaluateCanExecute();
                }
            }
        }

        protected override bool CanExecute()
        {
            return !string.IsNullOrEmpty(PlaylistName);
        }

        protected override void Run()
        {
            var playlist = new Playlist()
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
                catch (Exception)
                {
                    editor.Revert();
                    throw;
                }
                finally
                {
                    PlaylistName = string.Empty;
                }
            }
        }
    }
}

using Mirage.ViewModel.Commands;
using Riff.Data;
using System;

namespace Riff.UWP.ViewModel.Commands
{
    public sealed class RenamePlaylistCommand : Command<Playlist>
    {
        private readonly IMusicLibrary musicLibrary;

        public RenamePlaylistCommand(IMusicLibrary musicLibrary)
        {
            this.musicLibrary = musicLibrary;
        }

        public string PlaylistName
        {
            get;
            set;
        }

        public override bool CanExecute(Playlist playlist)
        {
            return playlist != null && playlist.Id.HasValue && !string.IsNullOrEmpty(PlaylistName);
        }

        protected override void Run(Playlist playlist)
        {
            using (var session = musicLibrary.Edit())
            {
                try
                {
                    var clonedPlaylist = playlist.Clone();
                    clonedPlaylist.Name = PlaylistName;
                    session.Playlists.Update(clonedPlaylist);
                    session.Save();
                }
                catch (Exception)
                {
                    session.Revert();
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

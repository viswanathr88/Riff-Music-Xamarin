using Mirage.ViewModel.Commands;
using Riff.Data;
using System;

namespace Riff.UWP.ViewModel.Commands
{
    public sealed class DeletePlaylistCommand : Command<Playlist>
    {
        private readonly IMusicLibrary musicLibrary;

        public DeletePlaylistCommand(IMusicLibrary musicLibrary)
        {
            this.musicLibrary = musicLibrary;
        }

        public override bool CanExecute(Playlist param)
        {
            return param != null && param.Id.HasValue;
        }

        protected override void Run(Playlist playlist)
        {
            using (var session = musicLibrary.Edit())
            {
                try
                {
                    session.PlaylistItems.Delete(playlist);
                    session.Playlists.Delete(playlist.Id.Value);
                    session.Save();
                }
                catch (Exception)
                {
                    session.Revert();
                    throw;
                }
            }
        }
    }
}

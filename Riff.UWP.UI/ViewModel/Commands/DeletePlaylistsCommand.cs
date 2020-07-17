using Mirage.ViewModel.Commands;
using Riff.Data;
using System;
using System.Collections.Generic;

namespace Riff.UWP.ViewModel.Commands
{
    public sealed class DeletePlaylistsCommand : Command<IList<object>>
    {
        private readonly IMusicLibrary musicLibrary;

        public DeletePlaylistsCommand(IMusicLibrary musicLibrary)
        {
            this.musicLibrary = musicLibrary;
        }

        public override bool CanExecute(IList<object> param)
        {
            return param != null;
        }

        protected override void Run(IList<object> items)
        {
            using (var session = musicLibrary.Edit())
            {
                try
                {
                    foreach (var item in items)
                    {
                        var playlist = item as Playlist;
                        session.PlaylistItems.Delete(playlist);
                        session.Playlists.Delete(playlist.Id.Value);
                    }
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

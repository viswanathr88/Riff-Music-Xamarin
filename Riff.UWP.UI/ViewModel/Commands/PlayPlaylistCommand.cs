using Mirage.ViewModel.Commands;
using Riff.Data;
using Riff.Data.Sqlite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Riff.UWP.ViewModel.Commands
{
    public sealed class PlayPlaylistCommand : AsyncCommand<Playlist>
    {
        private readonly IPlayer player;
        private readonly IMusicLibrary musicLibrary;

        public PlayPlaylistCommand(IPlayer player, IMusicLibrary musicLibrary)
        {
            this.musicLibrary = musicLibrary;
            this.player = player;
        }

        public bool AddToNowPlayingList
        {
            get;
            set;
        }

        public override bool CanExecute(Playlist playlist)
        {
            return playlist != null && playlist.Id.HasValue;
        }

        protected override async Task RunAsync(Playlist playlist)
        {
            IList<DriveItem> tracks = new List<DriveItem>();
            ExtractDriveItems(tracks, playlist);

            if (tracks.Count > 0)
            {
                await player.PlayAsync(tracks, 0, autoplay: !AddToNowPlayingList);
            }
        }

        private void ExtractDriveItems(IList<DriveItem> tracks, Playlist playlist)
        {
            var options = new PlaylistItemAccessOptions()
            {
                PlaylistFilter = playlist.Id,
                IncludeDriveItem = true
            };
            var playlistItems = musicLibrary.PlaylistItems.Get(options);
            foreach (var playlistItem in playlistItems)
            {
                tracks.Add(playlistItem.DriveItem);
            }
        }
    }
}

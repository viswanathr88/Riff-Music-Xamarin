using Mirage.ViewModel.Commands;
using Riff.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Riff.UWP.ViewModel.Commands
{
    public sealed class PlayPlaylistsCommand : AsyncCommand<IList<object>>
    {
        private readonly IPlayer player;
        private readonly IMusicLibrary musicLibrary;

        public PlayPlaylistsCommand(IPlayer player, IMusicLibrary musicLibrary)
        {
            this.musicLibrary = musicLibrary;
            this.player = player;
        }

        public bool AddToNowPlayingList
        {
            get;
            set;
        }

        public override bool CanExecute(IList<object> playlists)
        {
            return true;
        }

        protected override async Task RunAsync(IList<object> playlists)
        {
            int index = 0;
            foreach (var item in playlists)
            {
                if (item is Playlist playlist)
                {
                    bool autoplay = AddToNowPlayingList ? false : index++ == 0;
                    await player.PlayAsync(playlist, autoplay);
                }
            }
        }
    }
}

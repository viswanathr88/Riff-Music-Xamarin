using Mirage.ViewModel.Commands;
using Riff.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Riff.UWP.ViewModel.Commands
{
    public sealed class PlayTracksCommand : AsyncCommand<uint>
    {
        private readonly IPlayer player;
        private readonly bool addToNowPlaying = false;

        public PlayTracksCommand(IPlayer player, bool addToNowPlaying)
        {
            this.player = player;
            this.addToNowPlaying = addToNowPlaying;
        }

        public IList<DriveItem> Tracks { get; set; }

        public override bool CanExecute(uint index)
        {
            return true;
        }

        protected override async Task RunAsync(uint index)
        {
            await player.PlayAsync(Tracks, index, autoplay: !addToNowPlaying);
        }
    }
}

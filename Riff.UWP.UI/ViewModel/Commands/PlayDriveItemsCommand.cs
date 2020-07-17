using Mirage.ViewModel.Commands;
using Riff.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Riff.UWP.ViewModel.Commands
{
    public sealed class PlayDriveItemsCommand : AsyncCommand<IList<DriveItem>>
    {
        private readonly IPlayer player;

        public PlayDriveItemsCommand(IPlayer player)
        {
            this.player = player;
        }

        public bool AddToNowPlayingList { get; set; } = false;

        public override bool CanExecute(IList<DriveItem> items)
        {
            return items != null && items.Count > 0;
        }

        protected override async Task RunAsync(IList<DriveItem> items)
        {
            await player.PlayAsync(items, 0, autoplay: !AddToNowPlayingList);
        }
    }
}

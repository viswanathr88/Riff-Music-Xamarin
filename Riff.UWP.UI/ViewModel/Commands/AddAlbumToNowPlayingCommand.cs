using Mirage.ViewModel.Commands;
using System.Threading.Tasks;

namespace Riff.UWP.ViewModel.Commands
{
    public sealed class AddAlbumToNowPlayingCommand : AsyncCommand<AlbumItemViewModel>
    {
        private readonly IPlayer player;

        public AddAlbumToNowPlayingCommand(IPlayer player)
        {
            this.player = player;
        }

        public override bool CanExecute(AlbumItemViewModel album)
        {
            return album.Item.Id.HasValue;
        }

        protected override async Task RunAsync(AlbumItemViewModel album)
        {
            await player.PlayAsync(album.Item, autoplay: false);
        }
    }
}

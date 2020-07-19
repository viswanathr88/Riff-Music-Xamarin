using Mirage.ViewModel.Commands;
using System.Threading.Tasks;

namespace Riff.UWP.ViewModel.Commands
{
    public sealed class PlayAlbumItemCommand : AsyncCommand<AlbumItemViewModel>
    {
        private readonly IPlayer player;

        public PlayAlbumItemCommand(IPlayer player)
        {
            this.player = player;
        }

        public override bool CanExecute(AlbumItemViewModel param)
        {
            return param != null;
        }

        protected override async Task RunAsync(AlbumItemViewModel album)
        {
            await player.PlayAsync(album.Item, autoplay: true);
        }
    }
}

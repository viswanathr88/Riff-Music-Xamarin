using Mirage.ViewModel.Commands;
using Riff.Data;
using System.Threading.Tasks;

namespace Riff.UWP.ViewModel.Commands
{
    public sealed class PlayAlbumCommand : AsyncCommand<Album>
    {
        private readonly IPlayer player;

        public PlayAlbumCommand(IPlayer player)
        {
            this.player = player;
        }

        public override bool CanExecute(Album param)
        {
            return param != null;
        }

        protected override async Task RunAsync(Album album)
        {
            await player.PlayAsync(album, autoplay: true);
        }
    }
}

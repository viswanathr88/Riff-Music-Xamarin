using Mirage.ViewModel.Commands;
using System.Threading.Tasks;

namespace Riff.UWP.ViewModel.Commands
{
    public sealed class PlayArtistCommand : AsyncCommand<ArtistItemViewModel>
    {
        private readonly IPlayer player;

        public PlayArtistCommand(IPlayer player)
        {
            this.player = player;
        }

        public override bool CanExecute(ArtistItemViewModel artist)
        {
            return artist.Item.Id.HasValue;
        }

        protected override async Task RunAsync(ArtistItemViewModel artist)
        {
            await player.PlayAsync(artist.Item, autoplay: true);
        }
    }
}

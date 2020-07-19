using Mirage.ViewModel.Commands;
using Riff.Data;
using System.Linq;

namespace Riff.UWP.ViewModel
{
    public sealed class ArtistItemViewModel : ItemWithOverlayViewModel<Artist>
    {
        private readonly IGrouping<Artist, Album> group;
        private readonly IArtistCommands artistCommands;

        public ArtistItemViewModel(IGrouping<Artist, Album> group, IThumbnailReadOnlyCache cache, IArtistCommands commands) : base(group.Key)
        {
            this.artistCommands = commands;
            this.group = group;

            // Populate album arts
            foreach (var album in group)
            {
                if (cache.Exists(album.Id.Value))
                {
                    AlbumArt = cache.GetPath(album.Id.Value);
                    break;
                }
            }
        }

        public string Name => group.Key.Name;

        public int AlbumsCount => group.Count();

        public string AlbumArt { get; } = " ";

        public IAsyncCommand<ArtistItemViewModel> Play => artistCommands.PlayArtist;

        public IAsyncCommand<ArtistItemViewModel> PlayNext => artistCommands.PlayArtistNext;

        public IAsyncCommand<ArtistItemViewModel> AddToPlaylist => artistCommands.AddToPlaylist;
    }
}

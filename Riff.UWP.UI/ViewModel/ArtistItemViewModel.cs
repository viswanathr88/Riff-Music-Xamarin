using Mirage.ViewModel;
using Riff.Data;
using System.Linq;

namespace Riff.UWP.ViewModel
{
    public sealed class ArtistItemViewModel : ViewModelBase
    {
        private readonly IGrouping<Artist, Album> group;

        public ArtistItemViewModel(IGrouping<Artist, Album> group, IThumbnailReadOnlyCache cache)
        {
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

        public Artist Model => group.Key;

        public string Name => group.Key.Name;

        public int AlbumsCount => group.Count();

        public string AlbumArt { get; } = " ";

    }
}

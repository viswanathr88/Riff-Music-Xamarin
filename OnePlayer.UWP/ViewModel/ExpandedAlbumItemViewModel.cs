using OnePlayer.Data;
using System.Collections.ObjectModel;
using System.Linq;

namespace OnePlayer.UWP.ViewModel
{
    public sealed class ExpandedAlbumItemViewModel : ViewModelBase
    {
        private ObservableCollection<Track> tracks;
        private readonly IGrouping<Album, Track> group;
        private readonly ThumbnailCache cache;

        public ExpandedAlbumItemViewModel(ThumbnailCache cache, IGrouping<Album, Track> group)
        {
            this.cache = cache;
            this.group = group;

            Tracks = new ObservableCollection<Track>(group);
        }

        public Album Album => group.Key;

        public ObservableCollection<Track> Tracks { get; }

        public string Art => cache.Exists(Album.Id.Value) ? cache.GetPath(Album.Id.Value) : " ";

    }
}

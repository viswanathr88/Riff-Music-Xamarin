using OnePlayer.Data;
using OnePlayer.Data.Access;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlayer.UWP.ViewModel
{
    sealed class AlbumComparer : IEqualityComparer<Album>
    {
        public bool Equals(Album x, Album y)
        {
            return x.Id.Equals(y.Id);
        }

        public int GetHashCode(Album obj)
        {
            return obj.Id.GetHashCode();
        }
    }

    public sealed class ExpandedAlbumItem : IGrouping<Album, Track>
    {
        private IList<Track> tracks;
        private ThumbnailCache cache;

        public ExpandedAlbumItem(ThumbnailCache cache, Album key, IEnumerable<Track> tracks)
        {
            this.cache = cache;
            Key = key;
            this.tracks = new List<Track>(tracks);
        }

        public Album Key { get; }

        public IEnumerator<Track> GetEnumerator() => tracks.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => tracks.GetEnumerator();

        public string AlbumArt => cache.Exists(Key.Id.Value) ? cache.GetPath(Key.Id.Value) : " ";
    }

    public sealed class ArtistViewModel : DataViewModel<Artist>
    {
        private ObservableCollection<ExpandedAlbumItem> albumTracks;
        private readonly MusicLibrary library;

        public ArtistViewModel(MusicLibrary library)
        {
            this.library = library ?? throw new ArgumentNullException(nameof(library));
        }

        public ObservableCollection<ExpandedAlbumItem> AlbumTracks
        {
            get => albumTracks;
            set => SetProperty(ref this.albumTracks, value);
        }

        public async override Task LoadAsync(Artist artist)
        {
            Parameter = artist;

            var options = new TrackAccessOptions()
            {
                IncludeAlbum = true,
                IncludeGenre = true,
                AlbumArtistFilter = artist.Id,
                SortType = TrackSortType.ReleaseYear,
                SortOrder = SortOrder.Descending
            };

            var groups = await Task.Run(() =>
            {
                var tracks = library.Metadata.Tracks.Get(options);
                return tracks.GroupBy(track => track.Album, (key, list) => new ExpandedAlbumItem(library.AlbumArts, key, list), new AlbumComparer());
            });

            AlbumTracks = new ObservableCollection<ExpandedAlbumItem>(groups);
            IsLoaded = true;
        }
    }
}

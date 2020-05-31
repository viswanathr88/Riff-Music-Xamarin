using OnePlayer.Data;
using OnePlayer.Data.Access;
using System;
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

    public sealed class ArtistViewModel : DataViewModel<Artist>
    {
        private ObservableCollection<ExpandedAlbumItemViewModel> albumTracks;
        private readonly MusicLibrary library;

        public ArtistViewModel(MusicLibrary library)
        {
            this.library = library ?? throw new ArgumentNullException(nameof(library));
        }

        public ObservableCollection<ExpandedAlbumItemViewModel> AlbumTracks
        {
            get => albumTracks;
            set => SetProperty(ref this.albumTracks, value);
        }

        public async override Task LoadAsync(Artist artist)
        {
            var options = new TrackAccessOptions()
            {
                IncludeAlbum = true,
                IncludeGenre = true,
                AlbumArtistFilter = artist.Id
            };

            var groups = await Task.Run(() =>
            {
                var tracks = library.Metadata.Tracks.Get(options);
                return tracks.GroupBy(track => track.Album, new AlbumComparer());
            });

            AlbumTracks = new ObservableCollection<ExpandedAlbumItemViewModel>();
            foreach (var group in groups)
            {
                AlbumTracks.Add(new ExpandedAlbumItemViewModel(library.AlbumArts, group));
            }

            IsLoaded = true;
        }
    }
}

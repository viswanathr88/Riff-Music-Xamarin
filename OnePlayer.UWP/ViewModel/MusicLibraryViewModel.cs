using OnePlayer.Data;
using System;
using System.Threading.Tasks;

namespace OnePlayer.UWP.ViewModel
{
    public sealed class MusicLibraryViewModel : DataViewModel
    {
        private readonly MusicLibrary library;
        private readonly Lazy<AlbumsViewModel> albums;
        private readonly Lazy<ArtistsViewModel> artists;
        private readonly Lazy<TracksViewModel> tracks;

        public MusicLibraryViewModel(MusicLibrary library)
        {
            this.library = library ?? throw new ArgumentNullException(nameof(library));
            albums = new Lazy<AlbumsViewModel>(() => new AlbumsViewModel(this.library));
            artists = new Lazy<ArtistsViewModel>(() => new ArtistsViewModel(this.library));
            tracks = new Lazy<TracksViewModel>(() => new TracksViewModel(this.library));
        }

        public AlbumsViewModel Albums => albums.Value;

        public ArtistsViewModel Artists => artists.Value;

        public TracksViewModel Tracks => tracks.Value;

        public override Task LoadAsync()
        {
            IsLoaded = true;
            return Task.CompletedTask;
        }
    }
}

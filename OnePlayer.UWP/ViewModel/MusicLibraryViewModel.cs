using OnePlayer.Data;
using System;
using System.Threading.Tasks;

namespace OnePlayer.UWP.ViewModel
{
    public sealed class MusicLibraryViewModel : DataViewModel<VoidType>
    {
        private readonly MusicLibrary library;
        private readonly Lazy<AlbumsViewModel> albums;
        private readonly Lazy<ArtistsViewModel> artists;

        public MusicLibraryViewModel(MusicLibrary library)
        {
            this.library = library ?? throw new ArgumentNullException(nameof(library));
            albums = new Lazy<AlbumsViewModel>(() => new AlbumsViewModel(this.library));
            artists = new Lazy<ArtistsViewModel>(() => new ArtistsViewModel(this.library));
        }

        public AlbumsViewModel Albums => albums.Value;

        public ArtistsViewModel Artists => artists.Value;

        public override Task LoadAsync(VoidType parameter)
        {
            IsLoaded = true;
            return Task.CompletedTask;
        }
    }
}

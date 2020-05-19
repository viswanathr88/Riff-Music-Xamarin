using OnePlayer.Data;
using System;
using System.Threading.Tasks;

namespace OnePlayer.UWP.ViewModel
{
    public sealed class MusicLibraryViewModel : DataViewModel<VoidType>
    {
        private readonly MusicLibrary library;
        private readonly Lazy<AlbumsViewModel> albums;

        public MusicLibraryViewModel(MusicLibrary library)
        {
            this.library = library ?? throw new ArgumentNullException(nameof(library));
            albums = new Lazy<AlbumsViewModel>(() => new AlbumsViewModel(this.library));
        }

        public AlbumsViewModel Albums => albums.Value;

        public override Task LoadAsync(VoidType parameter)
        {
            IsLoaded = true;
            return Task.CompletedTask;
        }
    }
}

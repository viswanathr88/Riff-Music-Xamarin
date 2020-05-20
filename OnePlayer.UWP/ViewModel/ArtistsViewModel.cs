using OnePlayer.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace OnePlayer.UWP.ViewModel
{
    public sealed class ArtistsViewModel : DataViewModel
    {
        private ObservableCollection<Artist> items;
        private readonly MusicLibrary library;

        public ArtistsViewModel(MusicLibrary library)
        {
            this.library = library ?? throw new ArgumentNullException(nameof(library));
        }

        public ObservableCollection<Artist> Items
        {
            get => items;
            set => SetProperty(ref this.items, value);
        }

        public override async Task LoadAsync(VoidType parameter)
        {
            var results = await FetchArtistsAsync();
            Items = new ObservableCollection<Artist>(results);
            IsLoaded = true;
        }

        private async Task<IList<Artist>> FetchArtistsAsync()
        {
            return await Task.Run(() => library.Metadata.Artists.GetAll());
        }
    }
}

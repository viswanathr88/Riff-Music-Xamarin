using ListDiff;
using OnePlayer.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace OnePlayer.UWP.ViewModel
{
    public sealed class ArtistsViewModel : DataViewModel
    {
        private ObservableCollection<Artist> items = new ObservableCollection<Artist>();
        private readonly MusicLibrary library;

        public ArtistsViewModel(MusicLibrary library)
        {
            this.library = library ?? throw new ArgumentNullException(nameof(library));
            this.library.Metadata.Refreshed += Metadata_Refreshed;
        }

        public ObservableCollection<Artist> Items
        {
            get => items;
            set => SetProperty(ref this.items, value);
        }

        public override async Task LoadAsync()
        {
            var results = await FetchArtistsAsync();
            Items.MergeInto(results, (x, y) => x.Id == y.Id);
            IsLoaded = true;
        }

        private async Task<IList<Artist>> FetchArtistsAsync()
        {
            return await Task.Run(() => library.Metadata.Artists.GetAll());
        }
        private async void Metadata_Refreshed(object sender, EventArgs e)
        {
            await RunUISafe(() => IsLoaded = false);
        }
    }
}

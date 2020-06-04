using ListDiff;
using OnePlayer.Data;
using OnePlayer.Data.Access;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace OnePlayer.UWP.ViewModel
{
    public sealed class AlbumsViewModel : DataViewModel
    {
        private ObservableCollection<Album> items = new ObservableCollection<Album>();
        private readonly MusicLibrary library;
        private AlbumSortType sortType = AlbumSortType.ReleaseYear;
        private SortOrder sortOrder = SortOrder.Descending;

        public AlbumsViewModel(MusicLibrary library)
        {
            this.library = library ?? throw new ArgumentNullException(nameof(library));
            this.library.Metadata.Refreshed += Metadata_Refreshed;
        }

        public ObservableCollection<Album> Items
        {
            get => items;
            set => SetProperty(ref this.items, value);
        }

        public AlbumSortType SortType
        {
            get => sortType;
            set => SetProperty(ref this.sortType, value);
        }

        public SortOrder SortOrder
        {
            get => sortOrder;
            set => SetProperty(ref this.sortOrder, value);
        }

        public override async Task LoadAsync()
        {
            var results = await FetchAlbumsAsync();
            Items.MergeInto(results, (x, y) => x.Id == y.Id);
            IsLoaded = true;
        }

        public async Task ReloadAsync()
        {
            Items = new ObservableCollection<Album>();
            await LoadAsync();
        }

        private async void Metadata_Refreshed(object sender, EventArgs e)
        {
            await RunUISafe(() => IsLoaded = false);
        }

        private async Task<IList<Album>> FetchAlbumsAsync()
        {
            var options = new AlbumAccessOptions()
            {
                IncludeArtist = true,
                SortType = SortType,
                SortOrder = SortOrder
            };

            return await Task.Run(() => library.Metadata.Albums.Get(options));
        }
    }
}

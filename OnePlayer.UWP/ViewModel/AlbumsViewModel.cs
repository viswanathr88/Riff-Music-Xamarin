using OnePlayer.Data;
using OnePlayer.Data.Access;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace OnePlayer.UWP.ViewModel
{
    public sealed class AlbumsViewModel : DataViewModel
    {
        private ObservableCollection<Album> items;
        private readonly MusicLibrary library;
        private AlbumSortType sortType = AlbumSortType.ReleaseYear;
        private SortOrder sortOrder = SortOrder.Descending;

        public AlbumsViewModel(MusicLibrary library)
        {
            this.library = library ?? throw new ArgumentNullException(nameof(library));
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

        public override async Task LoadAsync(VoidType parameter)
        {
            //var ds = new VirtualDataSource<Album>(FetchAlbums);
            //await ds.UpdateCountAsync(GetCount);
            var results = await FetchAlbumsAsync();
            Items = new ObservableCollection<Album>(results);
            IsLoaded = true;
        }

        private Task<Album[]> FetchAlbums(ItemIndexRange range, CancellationToken ct)
        {
            var options = new AlbumAccessOptions()
            {
                IncludeArtist = true,
                IncludeGenre = true,
                StartPosition = range.FirstIndex,
                Count = range.LastIndex - range.FirstIndex + 1,
                SortType = SortType,
                SortOrder = SortOrder
            };

            return Task.FromResult(library.Metadata.Albums.Get(options).ToArray());

            /*return await Task.Run(() => {
                ct.ThrowIfCancellationRequested();
                var items = library.Metadata.Albums.Get(options);
                ct.ThrowIfCancellationRequested();
                return items.ToArray();
            });*/
        }

        private async Task<IList<Album>> FetchAlbumsAsync()
        {
            var options = new AlbumAccessOptions()
            {
                IncludeArtist = true,
                IncludeGenre = true,
                SortType = SortType,
                SortOrder = SortOrder
            };

            return await Task.Run(() => library.Metadata.Albums.Get(options));
        }

        private int GetCount()
        {
            return (int)library.Metadata.Albums.GetCount();
        }
    }
}

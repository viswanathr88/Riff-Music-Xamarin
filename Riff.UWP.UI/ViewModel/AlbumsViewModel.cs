using Riff.Data;
using Riff.Data.Access;
using Riff.UWP.UI.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Riff.UWP.ViewModel
{
    class AlbumEqualityComparer : IEqualityComparer<Album>
    {
        public bool Equals(Album x, Album y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(Album obj)
        {
            return obj.GetHashCode();
        }
    }

    public sealed class AlbumsViewModel : DataViewModel
    {
        private ObservableCollection<Album> items = new ObservableCollection<Album>();
        private readonly IMusicMetadata metadata;
        private AlbumSortType sortType = AlbumSortType.ReleaseYear;
        private SortOrder sortOrder = SortOrder.Descending;

        public AlbumsViewModel(IMusicMetadata metadata)
        {
            this.metadata = metadata;
            metadata.Refreshed += Metadata_Refreshed;
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
            var options = new AlbumAccessOptions()
            {
                IncludeArtist = true,
                SortType = SortType,
                SortOrder = SortOrder
            };

            if (Items.Count == 0)
            {
                Items = new ObservableCollection<Album>(await Task.Run(() => metadata.Albums.Get(options)));
            }
            else
            {
                var diffList = await Task.Run(() => {
                    var albums = metadata.Albums.Get(options);
                    return Diff.Compare(Items, albums, new AlbumEqualityComparer());
                });

                Items.ApplyDiff(diffList);
            }

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
    }
}

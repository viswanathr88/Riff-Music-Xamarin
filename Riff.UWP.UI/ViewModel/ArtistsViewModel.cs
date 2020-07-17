using Mirage.ViewModel;
using Riff.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Riff.UWP.ViewModel
{
    sealed class ArtistComparer : IEqualityComparer<Artist>
    {
        public bool Equals(Artist x, Artist y)
        {
            return x.Id == y.Id && x.Name == y.Name;
        }

        public int GetHashCode(Artist obj)
        {
            return obj.Id.GetHashCode();
        }
    }

    public sealed class ArtistsViewModel : DataViewModel
    {
        private ObservableCollection<ArtistItemViewModel> items = new ObservableCollection<ArtistItemViewModel>();
        private readonly IMusicLibrary library;
        private bool isCollectionEmpty = false;

        public ArtistsViewModel(IMusicLibrary library)
        {
            this.library = library ?? throw new ArgumentNullException(nameof(library));
            this.library.Refreshed += Metadata_Refreshed;
        }

        public ObservableCollection<ArtistItemViewModel> Items
        {
            get => items;
            set => SetProperty(ref this.items, value);
        }

        public bool IsCollectionEmpty
        {
            get => isCollectionEmpty;
            set => SetProperty(ref this.isCollectionEmpty, value);
        }

        public override async Task LoadAsync()
        {
            var groups = await Task.Run(() => GetArtists());
            foreach (var group in groups)
            {
                Items.Add(new ArtistItemViewModel(group, library.AlbumArts));
            }

            IsCollectionEmpty = (Items.Count == 0);

            IsLoaded = true;
        }

        private async void Metadata_Refreshed(object sender, EventArgs e)
        {
            await UIHelper.RunUISafe(() => IsLoaded = false);
        }

        private IEnumerable<IGrouping<Artist, Album>> GetArtists()
        {
            var options = new AlbumAccessOptions()
            {
                IncludeArtist = true,
                SortType = AlbumSortType.ReleaseYear,
                SortOrder = SortOrder.Descending
            };

            var albums = library.Albums.Get(options);
            return albums.GroupBy(album => album.Artist, new ArtistComparer()).OrderBy(group => group.Key.Name);
        }
    }
}

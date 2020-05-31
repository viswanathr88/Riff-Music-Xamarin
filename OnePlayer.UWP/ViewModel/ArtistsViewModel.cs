using ListDiff;
using OnePlayer.Data;
using OnePlayer.Data.Access;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace OnePlayer.UWP.ViewModel
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
        private readonly MusicLibrary library;

        public ArtistsViewModel(MusicLibrary library)
        {
            this.library = library ?? throw new ArgumentNullException(nameof(library));
            this.library.Metadata.Refreshed += Metadata_Refreshed;
        }

        public ObservableCollection<ArtistItemViewModel> Items
        {
            get => items;
            set => SetProperty(ref this.items, value);
        }

        public override async Task LoadAsync()
        {
            /*var results = await FetchArtistsAsync();
            Items.MergeInto(results, (x, y) => x.Id == y.Id);
            IsLoaded = true;*/

            var groups = await Task.Run(() => GetArtists());
            foreach (var group in groups)
            {
                Items.Add(new ArtistItemViewModel(group, library.AlbumArts));
            }

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

        private IEnumerable<IGrouping<Artist, Album>> GetArtists()
        {
            var options = new AlbumAccessOptions()
            {
                IncludeArtist = true
            };

            var albums = library.Metadata.Albums.Get(options);
            return albums.GroupBy(album => album.Artist, new ArtistComparer()).OrderBy(group => group.Key.Name);
        }
    }
}

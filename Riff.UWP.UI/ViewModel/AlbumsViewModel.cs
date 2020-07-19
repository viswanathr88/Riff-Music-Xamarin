using Mirage.ViewModel;
using Mirage.ViewModel.Commands;
using Riff.Data;
using Riff.UWP.UI.Extensions;
using Riff.UWP.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

    public sealed class AlbumsViewModel : DataViewModel, IAlbumCommands
    {
        private readonly IMusicLibrary library;
        private ObservableCollection<AlbumItemViewModel> items = new ObservableCollection<AlbumItemViewModel>();
        private AlbumSortType sortType = AlbumSortType.ReleaseYear;
        private SortOrder sortOrder = SortOrder.Descending;
        private bool isCollectionEmpty = false;

        public AlbumsViewModel(IMusicLibrary library, IPlayer player, PlaylistsViewModel playlistsVM)
        {
            this.library = library ?? throw new ArgumentNullException(nameof(library));
            library.Refreshed += Library_Refreshed;
            PlayAlbumItem = new PlayAlbumItemCommand(player);
            AddToPlaylistCommand = new AddAlbumToPlaylistCommand(library, playlistsVM);
            AddToNowPlayingCommand = new AddAlbumToNowPlayingCommand(player);
        }

        public ObservableCollection<AlbumItemViewModel> Items
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

        public IAsyncCommand<AlbumItemViewModel> PlayAlbumItem { get; }

        public IAsyncCommand<AlbumItemViewModel> AddToPlaylistCommand { get; }

        public IAsyncCommand<AlbumItemViewModel> AddToNowPlayingCommand { get; }

        public bool IsCollectionEmpty
        {
            get => isCollectionEmpty;
            set => SetProperty(ref this.isCollectionEmpty, value);
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
                Items = new ObservableCollection<AlbumItemViewModel>();

                foreach (var album in await Task.Run(() => library.Albums.Get(options)))
                {
                    Items.Add(new AlbumItemViewModel(album, library, this));
                }
            }
            else
            {
                var diffList = await Task.Run(() =>
                {
                    var albums = library.Albums.Get(options);
                    return Diff.Compare(Items.Select(itemVM => itemVM.Item).ToList(), albums, new AlbumEqualityComparer());
                });

                Items.ApplyDiff(diffList, (album) => new AlbumItemViewModel(album, library, this));
            }

            IsCollectionEmpty = (Items.Count == 0);

            IsLoaded = true;
        }

        public async Task ReloadAsync()
        {
            Items = new ObservableCollection<AlbumItemViewModel>();
            await LoadAsync();
        }

        private async void Library_Refreshed(object sender, EventArgs e)
        {
            await UIHelper.RunUISafe(() => IsLoaded = false);
        }
    }
}

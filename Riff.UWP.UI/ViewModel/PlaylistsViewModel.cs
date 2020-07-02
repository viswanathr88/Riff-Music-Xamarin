using Riff.Data;
using Riff.UWP.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Riff.UWP.ViewModel
{
    public class PlaylistsViewModel : DataViewModel
    {
        private readonly IMusicLibrary musicLibrary;

        private ObservableCollection<Playlist> playlists;
        private string playlistName;
        private bool isEmpty;
        private bool isSelectionMode;

        public PlaylistsViewModel(IPlayer player, IMusicLibrary library)
        {
            this.musicLibrary = library;
            this.musicLibrary.Playlists.StateChanged += PlaylistManager_StateChanged;
            this.Add = new AddPlaylistCommand(musicLibrary.Playlists);
            Delete = new DeletePlaylistCommand(musicLibrary.Playlists);
            Play = new PlayPlaylistsCommand(player);
            PlayNext = new PlayPlaylistsCommand(player) { AddToNowPlayingList = true };
            Rename = new RenamePlaylistCommand(musicLibrary.Playlists);
        }

        public ObservableCollection<Playlist> Playlists
        {
            get => playlists;
            private set => SetProperty(ref this.playlists, value);
        }

        public bool IsEmpty
        {
            get => isEmpty;
            private set => SetProperty(ref this.isEmpty, value);
        }

        public string PlaylistName
        {
            get => playlistName;
            set => SetProperty(ref this.playlistName, value);
        }

        public bool IsSelectionMode
        {
            get => isSelectionMode;
            set => SetProperty(ref this.isSelectionMode, value);
        }

        public ICommand Add { get; }

        public ICommand Delete { get; }

        public ICommand Play { get; }

        public ICommand PlayNext { get; }

        public RenamePlaylistCommand Rename { get; }

        public async override Task LoadAsync()
        {
            var playlists = await Task.Run(() => musicLibrary.Playlists.GetPlaylists());
            Playlists = new ObservableCollection<Playlist>(playlists);
            IsEmpty = (Playlists.Count == 0);
        }

        private async void PlaylistManager_StateChanged(object sender, System.EventArgs e)
        {
            await LoadAsync();
        }

        public async Task AddToPlaylist(Album album, Playlist playlist)
        {
            var options = new DriveItemAccessOptions()
            {
                IncludeTrack = true,
                IncludeTrackAlbum = true,
                IncludeTrackGenre = true,
                AlbumFilter = album.Id,
                SortType = TrackSortType.ReleaseYear,
                SortOrder = SortOrder.Descending
            };

            await AddToPlaylist(options, playlist);
        }

        public async Task AddToPlaylist(Artist artist, Playlist playlist)
        {
            var options = new DriveItemAccessOptions()
            {
                IncludeTrack = true,
                IncludeTrackAlbum = true,
                IncludeTrackGenre = true,
                AlbumArtistFilter = artist.Id,
                SortType = TrackSortType.ReleaseYear,
                SortOrder = SortOrder.Descending
            };

            await AddToPlaylist(options, playlist);
        }

        public async Task AddToPlaylist(IList<DriveItem> items, Playlist playlist)
        {
            await AddToPlaylist(() => items, playlist);
        }

        private async Task AddToPlaylist(DriveItemAccessOptions options, Playlist playlist)
        {
            await AddToPlaylist(() => musicLibrary.DriveItems.Get(options), playlist);
        }

        private async Task AddToPlaylist(Func<IList<DriveItem>> itemFetcher, Playlist playlist)
        {
            await Task.Run(async () =>
            {
                var items = itemFetcher();
                foreach (var item in items)
                {
                    playlist.Items.Add(item);
                }

                await playlist.SaveAsync();
            });
        }
    }
}

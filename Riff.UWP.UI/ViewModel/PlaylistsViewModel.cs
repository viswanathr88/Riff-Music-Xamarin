using Riff.Data;
using Riff.UWP.UI.Extensions;
using Riff.UWP.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Riff.UWP.ViewModel
{
    class PlaylistComparer : IEqualityComparer<Playlist2>
    {
        public bool Equals(Playlist2 x, Playlist2 y)
        {
            return x.Id == y.Id &&
                x.Name == y.Name &&
                x.LastModified == y.LastModified;
        }

        public int GetHashCode(Playlist2 obj)
        {
            return obj.GetHashCode();
        }
    }

    public class PlaylistsViewModel : DataViewModel
    {
        private readonly IMusicLibrary musicLibrary;

        private ObservableCollection<Playlist2> playlists = new ObservableCollection<Playlist2>();
        private bool isEmpty;
        private bool isSelectionMode;

        public PlaylistsViewModel(IPlayer player, IMusicLibrary library)
        {
            this.musicLibrary = library;
            this.Add = new AddPlaylistCommand(musicLibrary);
            Delete = new DeletePlaylistCommand(musicLibrary);
            Play = new PlayPlaylistsCommand(player, musicLibrary);
            PlayNext = new PlayPlaylistsCommand(player, musicLibrary) { AddToNowPlayingList = true };
            Rename = new RenamePlaylistCommand(musicLibrary);
        }

        public ObservableCollection<Playlist2> Playlists
        {
            get => playlists;
            private set => SetProperty(ref this.playlists, value);
        }

        public bool IsEmpty
        {
            get => isEmpty;
            private set => SetProperty(ref this.isEmpty, value);
        }

        public bool IsSelectionMode
        {
            get => isSelectionMode;
            set => SetProperty(ref this.isSelectionMode, value);
        }

        public AddPlaylistCommand Add { get; }

        public ICommand Delete { get; }

        public ICommand Play { get; }

        public ICommand PlayNext { get; }

        public RenamePlaylistCommand Rename { get; }

        public async override Task LoadAsync()
        {
            var diffList = await Task.Run(() => {
                var playlists = musicLibrary.Playlists2.Get(new PlaylistAccessOptions());
                return Diff.Compare(Playlists, playlists, new PlaylistComparer());
            });
            Playlists.ApplyDiff(diffList);
            IsEmpty = (Playlists.Count == 0);
        }

        private async void PlaylistManager_StateChanged(object sender, System.EventArgs e)
        {
            await LoadAsync();
        }

        public async Task AddToPlaylist(Album album, Playlist2 playlist)
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

        public async Task AddToPlaylist(Artist artist, Playlist2 playlist)
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

        public async Task AddToPlaylist(IList<DriveItem> items, Playlist2 playlist)
        {
            await AddToPlaylist(() => items, playlist);
        }

        private async Task AddToPlaylist(DriveItemAccessOptions options, Playlist2 playlist)
        {
            await AddToPlaylist(() => musicLibrary.DriveItems.Get(options), playlist);
        }

        private async Task AddToPlaylist(Func<IList<DriveItem>> itemFetcher, Playlist2 playlist)
        {
            await Task.Run(() =>
            {
                var items = itemFetcher();
                using (var session = musicLibrary.Edit())
                {
                    IList<PlaylistItem> playlistItems = new List<PlaylistItem>();
                    foreach (var driveItem in items)
                    {
                        playlistItems.Add(new PlaylistItem()
                        {
                            DriveItem = driveItem,
                            PlaylistId = playlist.Id.Value
                        });
                    }

                    session.PlaylistItems.Add(playlist, playlistItems);
                }
            });
        }
    }
}

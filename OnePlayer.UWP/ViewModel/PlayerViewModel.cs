using OnePlayer.Data;
using OnePlayer.Data.Access;
using OnePlayer.Sync;
using System.Threading.Tasks;
using Windows.Media.Playback;

namespace OnePlayer.UWP.ViewModel
{
    public sealed class PlayerViewModel : DataViewModel
    {
        private bool isPlayerVisible;
        private readonly MusicLibrary library;
        private readonly SyncEngine syncEngine;
        private MediaList list;

        public PlayerViewModel(MusicLibrary library, SyncEngine engine)
        {
            this.library = library;
            this.syncEngine = engine;
            this.MediaPlayer = new Windows.Media.Playback.MediaPlayer();
            MediaPlayer.AudioCategory = Windows.Media.Playback.MediaPlayerAudioCategory.Media;
        }

        public Windows.Media.Playback.MediaPlayer MediaPlayer { get; }

        public MediaList PlaybackList
        {
            get => list;
            private set
            {
                SetProperty(ref this.list, value);
                IsPlayerVisible = value != null;
            }
        }

        public bool IsPlayerVisible
        {
            get => isPlayerVisible;
            private set => SetProperty(ref this.isPlayerVisible, value);
        }

        public async Task PlayAsync(Album album, uint startIndex = 0)
        {
            var options = new DriveItemAccessOptions()
            {
                IncludeTrack = true,
                IncludeTrackAlbum = true,
                AlbumFilter = album.Id,
                SortType = TrackSortType.ReleaseYear,
                SortOrder = SortOrder.Descending
            };

            await PlayAsync(options, startIndex);
        }

        public async Task PlayAsync(Artist artist, uint startIndex = 0)
        {
            var options = new DriveItemAccessOptions()
            {
                IncludeTrack = true,
                IncludeTrackAlbum = true,
                AlbumArtistFilter = artist.Id,
                SortType = TrackSortType.ReleaseYear,
                SortOrder = SortOrder.Descending
            };

            await PlayAsync(options, startIndex);
        }

        public async Task PlayAsync(TrackSortType type, SortOrder order, uint startIndex = 0)
        {
            var options = new DriveItemAccessOptions()
            {
                IncludeTrack = true,
                IncludeTrackAlbum = true,
                SortType = type,
                SortOrder = order
            };

            await PlayAsync(options, startIndex);
        }

        private async Task PlayAsync(DriveItemAccessOptions options, uint startIndex)
        {
            PlaybackList = new MediaList(library, syncEngine);
            MediaPlayer.Source = PlaybackList.InnerList;

            await Task.Run(async() =>
            {
                var items = library.Metadata.DriveItems.Get(options);
                await PlaybackList.SetItems(items, startIndex, MediaPlayer);
            });
        }

        public override Task LoadAsync()
        {
            return Task.CompletedTask;
        }
    }
}

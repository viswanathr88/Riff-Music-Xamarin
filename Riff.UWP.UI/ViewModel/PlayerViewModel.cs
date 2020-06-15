using Riff.Data;
using Riff.Data.Access;
using Riff.Sync;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media.Playback;

namespace Riff.UWP.ViewModel
{
    public sealed class PlayerViewModel : DataViewModel
    {
        private bool isPlayerVisible;
        private readonly MusicLibrary library;
        private readonly ITrackUrlDownloader urlDownloader;
        private MediaList list;

        public PlayerViewModel(MusicLibrary library, ITrackUrlDownloader urlDownloader)
        {
            this.library = library;
            this.urlDownloader = urlDownloader;
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
            var items = await Task.Run(() => library.Metadata.DriveItems.Get(options));
            await PlayAsync(items, startIndex);
        }

        public async Task PlayAsync(IList<DriveItem> items, uint startIndex)
        {
            PlaybackList = new MediaList(library, urlDownloader);
            MediaPlayer.Source = PlaybackList.InnerList;
            MediaPlayer.AutoPlay = true;

            await Task.Run(async () =>
            {
                await PlaybackList.SetItems(items, startIndex);
            });
        }

        public override Task LoadAsync()
        {
            return Task.CompletedTask;
        }
    }
}

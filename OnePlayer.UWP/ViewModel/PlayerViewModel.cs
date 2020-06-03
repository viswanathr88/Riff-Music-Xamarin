using Microsoft.VisualBasic.CompilerServices;
using OnePlayer.Data;
using OnePlayer.Data.Access;
using OnePlayer.Sync;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Media.Playback;
using Windows.Storage.Streams;

namespace OnePlayer.UWP.ViewModel
{
    public sealed class PlayerViewModel : DataViewModel
    {
        private bool isPlayerVisible;
        private readonly MusicLibrary library;
        private readonly SyncEngine syncEngine;

        public PlayerViewModel(MusicLibrary library, SyncEngine engine)
        {
            this.library = library;
            this.syncEngine = engine;
            this.MediaPlayer = new Windows.Media.Playback.MediaPlayer();
            MediaPlayer.AudioCategory = Windows.Media.Playback.MediaPlayerAudioCategory.Media;
        }

        public Windows.Media.Playback.MediaPlayer MediaPlayer { get; }

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
            Windows.Media.Playback.MediaPlaybackList list = new Windows.Media.Playback.MediaPlaybackList();
            MediaPlayer.Source = list;

            await Task.Run(() =>
            {
                var items = library.Metadata.DriveItems.Get(options);

                uint currentIndex = 0;
                foreach (var item in items)
                {
                    var playbackItem = AddToPlaybackList(list, item);
                    if (currentIndex == startIndex)
                    {
                        list.StartingItem = playbackItem;
                        list.MoveTo(startIndex);
                        MediaPlayer.Play();
                    }
                    currentIndex++;
                }
            });
        }
        
        private MediaPlaybackItem AddToPlaybackList(MediaPlaybackList list, DriveItem item)
        {
            var binder = new Windows.Media.Core.MediaBinder();
            binder.Token = item.Id;
            binder.Binding += Binder_Binding;
            var playbackSource = Windows.Media.Core.MediaSource.CreateFromMediaBinder(binder);
            var playbackItem = new Windows.Media.Playback.MediaPlaybackItem(playbackSource);
            var props = playbackItem.GetDisplayProperties();
            props.Type = Windows.Media.MediaPlaybackType.Music;
            props.MusicProperties.Title = item.Track.Title;
            props.MusicProperties.Artist = item.Track.Artist;
            props.MusicProperties.AlbumTitle = item.Track.Album.Name;
            props.MusicProperties.TrackNumber = Convert.ToUInt32(item.Track.Number);
            // props.MusicProperties.AlbumArtist = item.Track.Album.Artist.Name;
            // props.MusicProperties.Genres.Add(item.Track.Genre.Name);
            playbackItem.ApplyDisplayProperties(props);
            list.Items.Add(playbackItem);
            return playbackItem;
        }

        private async void Binder_Binding(Windows.Media.Core.MediaBinder sender, Windows.Media.Core.MediaBindingEventArgs args)
        {
            var deferral = args.GetDeferral();
            var uri = await this.syncEngine.GetDownloadUrlAsync(sender.Token);
            args.SetUri(uri);
            deferral.Complete();
        }

        public override Task LoadAsync()
        {
            return Task.CompletedTask;
        }
    }
}

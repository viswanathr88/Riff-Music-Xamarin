using Riff.Data;
using Riff.Sync;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Media.Playback;

namespace Riff.UWP.ViewModel
{
    public sealed class PlayerViewModel : DataViewModel, IPlayer
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

        public event EventHandler<EventArgs> CurrentTrackChanged;

        public Windows.Media.Playback.MediaPlayer MediaPlayer { get; }

        public IMediaList PlaybackList
        {
            get => list;
            private set
            {
                if (list != value)
                {
                    if (list != null && list is INotifyPropertyChanged changed)
                    {
                        changed.PropertyChanged -= PlaybackList_PropertyChanged;
                    }
                    SetProperty(ref this.list, (MediaList)value);

                    if (value != null && value is INotifyPropertyChanged newChanged)
                    {
                        newChanged.PropertyChanged += PlaybackList_PropertyChanged;
                    }
                }
                IsPlayerVisible = value != null;
            }
        }

        public bool IsPlayerVisible
        {
            get => isPlayerVisible;
            private set => SetProperty(ref this.isPlayerVisible, value);
        }

        public async Task PlayAsync(IList<DriveItem> items, uint startIndex)
        {
            PlaybackList = new MediaList(library, urlDownloader);
            MediaPlayer.Source = list.InnerList;
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

        private void PlaybackList_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MediaList.CurrentIndex))
            {
                CurrentTrackChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}

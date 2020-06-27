using Riff.Data;
using Riff.Sync;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage.Streams;

namespace Riff.UWP.ViewModel
{
    public sealed class MediaList : ObservableCollection<IMediaListItem>, INotifyPropertyChanged, IMediaList
    {
        private readonly ITrackUrlDownloader trackUrlDownloader;
        private readonly MusicLibrary library;
        private IMediaListItem currentItem;
        private int currentIndex = -1;
        private static int MediaListMaxSize = 1000;

        public MediaList(MusicLibrary library, ITrackUrlDownloader urlDownloader)
        {
            this.library = library;
            this.trackUrlDownloader = urlDownloader;
            InnerList = new MediaPlaybackList();
            InnerList.Items.VectorChanged += Items_VectorChanged;
            InnerList.CurrentItemChanged += List_CurrentItemChanged;
        }

        public MediaPlaybackList InnerList { get; private set; }

        public IMediaListItem CurrentItem
        {
            get => currentItem;
            private set
            {
                if (this.currentItem != value)
                {
                    this.currentItem = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(CurrentItem)));
                }
            }
        }

        public int CurrentIndex
        {
            get => currentIndex;
            set
            {
                if (this.currentIndex != value)
                {
                    this.currentIndex = value;
                    InnerList.MoveTo((uint)this.currentIndex);
                    this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(CurrentIndex)));
                }
            }
        }

        public async Task AddItems(IList<DriveItem> items, uint startIndex)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (startIndex >= items.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }

            int itemsCount = Math.Min(MediaListMaxSize - InnerList.Items.Count, items.Count);
            int index = Convert.ToInt32(startIndex);

            while (itemsCount-- > 0)
            {
                int indexToCopy = index % items.Count;
                var item = items[indexToCopy];

                var binder = new Windows.Media.Core.MediaBinder
                {
                    Token = item.Id
                };
                binder.Binding += Binder_Binding;
                var playbackSource = Windows.Media.Core.MediaSource.CreateFromMediaBinder(binder);
                var playbackItem = new Windows.Media.Playback.MediaPlaybackItem(playbackSource);

                // It's ok if we fail to set metadata for the track
                try
                {
                    var albumId = item.Track.Album.Id.Value;
                    var props = playbackItem.GetDisplayProperties();
                    props.Type = Windows.Media.MediaPlaybackType.Music;

                    props.MusicProperties.Title = item.Track.Title ?? "Unknown Title";
                    props.MusicProperties.Artist = item.Track.Artist ?? "Unknown Artist";
                    props.MusicProperties.AlbumTitle = item.Track.Album.Name ?? "Unknown Album";
                    props.MusicProperties.TrackNumber = Convert.ToUInt32(item.Track.Number);
                    props.Thumbnail = library.AlbumArts.Exists(albumId) ? RandomAccessStreamReference.CreateFromFile(await library.AlbumArts.GetStorageFile(albumId)) : null;
                    playbackSource.CustomProperties.Add("TrackId", item.Track.Id);
                    playbackSource.CustomProperties.Add("ItemId", item.Id);
                    playbackSource.CustomProperties.Add("AlbumId", albumId);
                    playbackSource.CustomProperties.Add("Year", item.Track.Album.ReleaseYear);
                    playbackSource.CustomProperties.Add("AlbumArtPath", library.AlbumArts.Exists(albumId) ? library.AlbumArts.GetPath(albumId) : "");
                    playbackItem.ApplyDisplayProperties(props);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }

                InnerList.Items.Add(playbackItem);
                index++;
            }
        }

        private async void Binder_Binding(MediaBinder sender, MediaBindingEventArgs args)
        {
            var deferral = args.GetDeferral();
            var uri = await this.trackUrlDownloader.GetDownloadUrlAsync(sender.Token);
            args.SetUri(uri);
            deferral.Complete();
        }

        private async void List_CurrentItemChanged(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args)
        {
            await ViewModelBase.RunUISafe(async () =>
            {
                var index = InnerList.CurrentItemIndex;
                if (index < Items.Count && index >= 0)
                {
                    int _index = Convert.ToInt32(index);
                    CurrentItem = Items[Convert.ToInt32(_index)];
                    CurrentIndex = _index;
                    await CurrentItem.LoadArtAsync();
                }
            });
        }

        private async void Items_VectorChanged(Windows.Foundation.Collections.IObservableVector<MediaPlaybackItem> sender, Windows.Foundation.Collections.IVectorChangedEventArgs args)
        {
            await ViewModelBase.RunUISafe(() =>
            {
                int index = Convert.ToInt32(args.Index);

                if (args.CollectionChange == Windows.Foundation.Collections.CollectionChange.ItemInserted)
                {
                    InsertItem(index, new MediaListItem(InnerList.Items[index]));
                }
                else if (args.CollectionChange == Windows.Foundation.Collections.CollectionChange.ItemRemoved)
                {
                    RemoveAt(index);
                }
                else if (args.CollectionChange == Windows.Foundation.Collections.CollectionChange.ItemChanged)
                {
                    Items[index] = new MediaListItem(InnerList.Items[index]);
                }
                else
                {
                    Items.Clear();
                    throw new Exception("Trying to understand this event");
                }
            });

        }
    }
}

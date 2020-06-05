using OnePlayer.Data;
using OnePlayer.Sync;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage.Streams;

namespace OnePlayer.UWP.ViewModel
{
    public sealed class MediaList : ObservableCollection<MediaListItem>
    {
        private readonly MediaPlaybackList list;
        private readonly SyncEngine syncEngine;
        private readonly MusicLibrary library;
        private MediaListItem currentItem;
        private int currentIndex = -1;

        public MediaList(MusicLibrary library, SyncEngine syncEngine)
        {
            this.library = library;
            this.syncEngine = syncEngine;
            list = new MediaPlaybackList();
            list.Items.VectorChanged += Items_VectorChanged;
            list.CurrentItemChanged += List_CurrentItemChanged;
        }

        public MediaPlaybackList InnerList => list;

        public MediaListItem CurrentItem
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

        public void SetItems(IEnumerable<DriveItem> items, uint startIndex, MediaPlayer player)
        {
            uint currentIndex = 0;
            foreach (var item in items)
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
                props.Thumbnail = library.AlbumArts.Exists(item.Track.Album.Id.Value) ? RandomAccessStreamReference.CreateFromUri(new Uri(library.AlbumArts.GetPath(item.Track.Album.Id.Value), UriKind.Absolute)) : null;
                playbackSource.CustomProperties.Add("AlbumId", item.Track.Album.Id.Value);
                playbackSource.CustomProperties.Add("Year", item.Track.Album.ReleaseYear);
                playbackSource.CustomProperties.Add("AlbumArtPath", library.AlbumArts.Exists(item.Track.Album.Id.Value) ? library.AlbumArts.GetPath(item.Track.Album.Id.Value) : "");
                playbackItem.ApplyDisplayProperties(props);
                list.Items.Add(playbackItem);

                if (currentIndex == startIndex)
                {
                    list.StartingItem = list.Items[(int)currentIndex];
                    list.MoveTo(currentIndex);
                    player.Play();
                }

                currentIndex++;
            }
        }

        private async void Binder_Binding(MediaBinder sender, MediaBindingEventArgs args)
        {
            var deferral = args.GetDeferral();
            var uri = await this.syncEngine.GetDownloadUrlAsync(sender.Token);
            args.SetUri(uri);
            deferral.Complete();
        }

        private async void List_CurrentItemChanged(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args)
        {
            await ViewModelBase.RunUISafe(async () =>
            {
                var index = list.CurrentItemIndex;
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
                    InsertItem(index, new MediaListItem(list.Items[index]));
                }
                else if (args.CollectionChange == Windows.Foundation.Collections.CollectionChange.ItemRemoved)
                {
                    RemoveAt(index);
                }
                else if (args.CollectionChange == Windows.Foundation.Collections.CollectionChange.ItemChanged)
                {
                    Items[index] = new MediaListItem(list.Items[index]);
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

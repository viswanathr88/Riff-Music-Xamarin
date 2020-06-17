using Riff.Data;
using Riff.Data.Access;
using Riff.UWP.UI.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls.Primitives;

namespace Riff.UWP.ViewModel
{
    class TracksEqualityComparer : IEqualityComparer<DriveItem>
    {
        public bool Equals(DriveItem x, DriveItem y)
        {
            return x.Track.Id == y.Track.Id;
        }

        public int GetHashCode(DriveItem obj)
        {
            return obj.Track.GetHashCode();
        }
    }

    public sealed class TracksViewModel : DataViewModel
    {
        private readonly MusicLibrary library;
        private TrackSortType sortType = TrackSortType.ReleaseYear;
        private SortOrder sortOrder = SortOrder.Descending;
        private ObservableCollection<DriveItem> tracks = new ObservableCollection<DriveItem>();
        private int currentIndex = -1;

        public TracksViewModel(MusicLibrary library)
        {
            this.library = library ?? throw new ArgumentNullException(nameof(library));
            this.library.Metadata.Refreshed += Metadata_Refreshed;
        }

        public int CurrentIndex
        {
            get => currentIndex;
            set => SetProperty(ref this.currentIndex, value);
        }

        public ObservableCollection<DriveItem> Tracks
        {
            get => tracks;
            private set => SetProperty(ref this.tracks, value);
        }

        public TrackSortType SortType
        {
            get => sortType;
            set => SetProperty(ref this.sortType, value);
        }

        public SortOrder SortOrder
        {
            get => sortOrder;
            set => SetProperty(ref this.sortOrder, value);
        }

        public async override Task LoadAsync()
        {
            var options = new DriveItemAccessOptions()
            {
                IncludeTrack = true,
                IncludeTrackAlbum = true,
                IncludeTrackGenre = true,
                SortType = SortType,
                SortOrder = SortOrder
            };

            if (Tracks.Count == 0)
            {
                Tracks = new ObservableCollection<DriveItem>(await Task.Run(() => library.Metadata.DriveItems.Get(options)));
            }
            else
            {
                var diffList = await Task.Run(() =>
                {
                    var results = library.Metadata.DriveItems.Get(options);
                    return Diff.Compare(Tracks, results, new TracksEqualityComparer());
                });

                Tracks.ApplyDiff(diffList);
            }
            IsLoaded = true;
        }

        public async Task ReloadAsync()
        {
            Tracks = new ObservableCollection<DriveItem>();
            await LoadAsync();
        }

        private async void Metadata_Refreshed(object sender, EventArgs e)
        {
            await RunUISafe(() => IsLoaded = false);
        }
    }
}

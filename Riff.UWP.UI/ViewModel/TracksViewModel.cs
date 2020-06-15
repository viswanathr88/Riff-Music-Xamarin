using ListDiff;
using Riff.Data;
using Riff.Data.Access;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Riff.UWP.ViewModel
{
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
            var results = await FetchTracksAsync();
            Tracks.MergeInto(results, (x, y) => x.Id == y.Id);
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

        private async Task<IList<DriveItem>> FetchTracksAsync()
        {
            var options = new DriveItemAccessOptions()
            {
                IncludeTrack = true,
                IncludeTrackAlbum = true,
                SortType = SortType,
                SortOrder = SortOrder
            };

            return await Task.Run(() => library.Metadata.DriveItems.Get(options));
        }
    }
}

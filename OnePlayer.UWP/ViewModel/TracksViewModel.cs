using ListDiff;
using OnePlayer.Data;
using OnePlayer.Data.Access;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace OnePlayer.UWP.ViewModel
{
    public sealed class TracksViewModel : DataViewModel
    {
        private readonly MusicLibrary library;
        private TrackSortType sortType = TrackSortType.ReleaseYear;
        private SortOrder sortOrder = SortOrder.Descending;
        private ObservableCollection<Track> tracks = new ObservableCollection<Track>();

        public TracksViewModel(MusicLibrary library)
        {
            this.library = library ?? throw new ArgumentNullException(nameof(library));
            this.library.Metadata.Refreshed += Metadata_Refreshed;
        }

        public ObservableCollection<Track> Tracks
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
            Tracks = new ObservableCollection<Track>();
            await LoadAsync();
        }

        private async void Metadata_Refreshed(object sender, EventArgs e)
        {
            await RunUISafe(() => IsLoaded = false);
        }

        private async Task<IList<Track>> FetchTracksAsync()
        {
            var options = new TrackAccessOptions()
            {
                IncludeAlbum = true,
                IncludeGenre = true,
                SortType = SortType,
                SortOrder = SortOrder
            };

            return await Task.Run(() => library.Metadata.Tracks.Get(options));
        }
    }
}

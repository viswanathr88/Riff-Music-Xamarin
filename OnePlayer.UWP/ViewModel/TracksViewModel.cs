using OnePlayer.Data;
using OnePlayer.Data.Access;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlayer.UWP.ViewModel
{
    public sealed class TracksViewModel : DataViewModel
    {
        private readonly MusicLibrary library;
        private TrackSortType sortType = TrackSortType.ReleaseYear;
        private SortOrder sortOrder = SortOrder.Descending;
        private ObservableCollection<Track> tracks;

        public TracksViewModel(MusicLibrary library)
        {
            this.library = library ?? throw new ArgumentNullException(nameof(library));
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

        public async override Task LoadAsync(VoidType parameter)
        {
            var options = new TrackAccessOptions()
            {
                IncludeAlbum = true,
                IncludeGenre = true,
                SortType = SortType,
                SortOrder = SortOrder
            };

            var tracks = await Task.Run(() => library.Metadata.Tracks.Get(options));
            Tracks = new ObservableCollection<Track>(tracks);

            IsLoaded = true;
        }
    }
}

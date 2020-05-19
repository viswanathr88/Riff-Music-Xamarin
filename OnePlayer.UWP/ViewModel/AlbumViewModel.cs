using OnePlayer.Data;
using OnePlayer.Data.Access;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace OnePlayer.UWP.ViewModel
{
    public sealed class AlbumViewModel : DataViewModel<Album>
    {
        private Album album;
        private readonly IMusicMetadata metadata;
        private ObservableCollection<Track> tracks;

        public AlbumViewModel(IMusicMetadata metadata)
        {
            this.metadata = metadata;
        }

        public Album AlbumInfo
        {
            get => album;
            private set => SetProperty(ref this.album, value);
        }

        public ObservableCollection<Track> Tracks
        {
            get => tracks;
            private set => SetProperty(ref this.tracks, value);
        }

        public override async Task LoadAsync(Album album)
        {
            AlbumInfo = album ?? throw new ArgumentNullException(nameof(album));

            var tracks = await Task.Run(() =>
            {
                var options = new TrackAccessOptions()
                {
                    SortType = TrackSortType.Number,
                    SortOrder = SortOrder.Ascending,
                    AlbumFilter = album.Id.Value
                };

                return this.metadata.Tracks.Get(options);
            });

            Tracks = new ObservableCollection<Track>(tracks);
            IsLoaded = true;
        }
    }
}

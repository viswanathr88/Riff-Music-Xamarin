using Mirage.ViewModel;
using Riff.Data;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Riff.UWP.ViewModel
{
    public sealed class AlbumViewModel : DataViewModel<Album>
    {
        private Album album;
        private readonly IMusicLibrary library;
        private ObservableCollection<DriveItem> tracks;

        public AlbumViewModel(IMusicLibrary library)
        {
            this.library = library;
        }

        public Album AlbumInfo
        {
            get => album;
            private set => SetProperty(ref this.album, value);
        }

        public ObservableCollection<DriveItem> Tracks
        {
            get => tracks;
            private set => SetProperty(ref this.tracks, value);
        }

        public override async Task LoadAsync(Album album)
        {
            AlbumInfo = album ?? throw new ArgumentNullException(nameof(album));

            var tracks = await Task.Run(async() =>
            {
                var albumOptions = new AlbumAccessOptions()
                {
                    IncludeArtist = true,
                    IncludeGenre = true,
                    AlbumFilter = album.Id
                };

                var a = this.library.Albums.Get(albumOptions).First();
                await UIHelper.RunUISafe(() => AlbumInfo = a);

                // Get tracks for album
                var options = new DriveItemAccessOptions()
                {
                    SortType = TrackSortType.Number,
                    SortOrder = SortOrder.Ascending,
                    AlbumFilter = album.Id.Value,
                    IncludeTrack = true
                };

                return this.library.DriveItems.Get(options);
            });

            Tracks = new ObservableCollection<DriveItem>(tracks);
            IsLoaded = true;
        }
    }
}

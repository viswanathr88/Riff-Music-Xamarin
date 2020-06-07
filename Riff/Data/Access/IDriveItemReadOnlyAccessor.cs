using System.Collections;
using System.Collections.Generic;

namespace Riff.Data.Access
{
    public sealed class DriveItemAccessOptions
    {
        public string DriveItemFilter { get; set; }
        public long? TrackFilter { get; set; }
        public long? AlbumFilter { get; set; }
        public long? AlbumArtistFilter { get; set; }
        public TrackSortType? SortType { get; set; }
        public SortOrder? SortOrder { get; set; }
        public long? StartPosition { get; set; }
        public long? Count { get; set; }
        public bool IncludeTrack { get; set; }
        public bool IncludeTrackAlbum { get; set; }
    }

    public interface IDriveItemReadOnlyAccessor
    {
        DriveItem Get(string id);

        IList<DriveItem> Get(DriveItemAccessOptions options);
    }
}

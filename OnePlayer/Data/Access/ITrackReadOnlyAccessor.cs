using System;
using System.Collections.Generic;
using System.Text;

namespace OnePlayer.Data.Access
{
    public enum TrackSortType
    {
        Number,
        Title,
        TrackArtist,
        Duration
    };

    public sealed class TrackAccessOptions
    {
        public TrackSortType? SortType { get; set; }
        public SortOrder SortOrder { get; set; } = SortOrder.Descending;
        public long? TrackFilter { get; set; }
        public long? AlbumFilter { get; set; }
        public long? GenreFilter { get; set; }
        public long? StartPosition { get; set; }
        public long? Count { get; set; }
        public bool IncludeAlbum { get; set; } = false;
        public bool IncludeGenre { get; set; } = false;
    }

    public interface ITrackReadOnlyAccessor
    {
        Track Get(long id);

        IList<Track> Get(TrackAccessOptions options);

        long GetCount();

        long GetCount(TrackAccessOptions options);
    }
}

using System.Collections.Generic;

namespace OnePlayer.Data.Access
{
    public enum AlbumSortType
    {
        ReleaseYear,
        Name
    };

    public sealed class AlbumAccessOptions
    {
        public AlbumSortType? SortType { get; set; }
        public SortOrder SortOrder { get; set; } = Data.SortOrder.Descending;
        public long? ArtistFilter { get; set; }
        public long? GenreFilter { get; set; }
        public long? AlbumFilter { get; set; }
        public string AlbumNameFilter { get; set; } = string.Empty;
        public long? StartPosition { get; set; }
        public long? Count { get; set; }
        public bool IncludeArtist { get; set; } = false;
        public bool IncludeGenre { get; set; } = false;
    }

    public interface IAlbumReadOnlyAccessor
    {
        long GetCount();

        long GetCount(AlbumAccessOptions options);

        Album Get(long id);

        IList<Album> Get();

        IList<Album> Get(AlbumSortType type, SortOrder order);

        IList<Album> Get(AlbumAccessOptions options);

        Album FindByArtist(long artistId, string albumName);
    }
}

using System.Collections.Generic;

namespace Riff.Data
{
    public enum PlaylistSortType
    {
        Name,
        LastModified
    };

    public sealed class PlaylistAccessOptions
    {
        public long? PlaylistFilter { get; set; }
        public PlaylistSortType SortType { get; set; } = PlaylistSortType.LastModified;
        public SortOrder SortOrder { get; set; } = SortOrder.Descending;
    }

    public interface IPlaylistReadOnlyAccessor
    {
        Playlist2 Get(long id);
        IList<Playlist2> Get(PlaylistAccessOptions options);
    }

   public interface IPlaylistAccessor : IPlaylistReadOnlyAccessor
    {
        Playlist2 Add(Playlist2 playlist);
        Playlist2 Update(Playlist2 playlist);
        void Delete(long id);
    }
}

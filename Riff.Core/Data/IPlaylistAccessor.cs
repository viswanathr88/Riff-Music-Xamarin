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
        Playlist Get(long id);
        IList<Playlist> Get(PlaylistAccessOptions options);
    }

   public interface IPlaylistAccessor : IPlaylistReadOnlyAccessor
    {
        Playlist Add(Playlist playlist);
        Playlist Update(Playlist playlist);
        void Delete(long id);
    }
}

using System.Collections.Generic;

namespace Riff.Data.Sqlite
{
    public sealed class PlaylistItemAccessOptions
    {
        public long? PlaylistItemFilter { get; set; }
        public long? PlaylistFilter { get; set; }
        public bool IncludeDriveItem { get; set; }
        public bool GetLastItemOnly { get; set; }
    }

    public interface IPlaylistItemReadOnlyAccessor
    {
        PlaylistItem Get(long id);
        IList<PlaylistItem> Get(PlaylistItemAccessOptions options);
    }

    public interface IPlaylistItemAccessor : IPlaylistItemReadOnlyAccessor
    {
        IList<PlaylistItem> Add(Playlist2 playlist, IList<PlaylistItem> items);
        PlaylistItem Add(Playlist2 playlist, PlaylistItem item);
        void Delete(Playlist2 playlist);
        void Delete(long id);
        void Update(PlaylistItem item);
        void Reorder(Playlist2 playlist, int newIndex, int oldIndex, int count);
        void Reorder(IList<PlaylistItem> sourceItems, int newIndex, int oldIndex, int count);
    }
}
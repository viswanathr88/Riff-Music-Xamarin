using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Riff.Data
{
    public interface IEditSession : IDisposable
    {
        IAlbumAccessor Albums { get; }

        IArtistAccessor Artists { get; }

        IGenreAccessor Genres { get; }

        ITrackAccessor Tracks { get; }

        IDriveItemAccessor DriveItems { get; }

        IIndexAccessor Index { get; }

        IThumbnailInfoAccessor Thumbnails { get; }

        IThumbnailCache AlbumArts { get; }

        void Save();

        void Revert();
    }

    public interface IMusicLibrary : IDisposable
    {
        IAlbumReadOnlyAccessor Albums { get; }

        IArtistReadOnlyAccessor Artists { get; }

        IGenreReadOnlyAccessor Genres { get; }

        ITrackReadOnlyAccessor Tracks { get; }

        IDriveItemReadOnlyAccessor DriveItems { get; }

        IIndexReadOnlyAccessor Index { get; }

        IThumbnailInfoReadOnlyAccessor Thumbnails { get; }

        IThumbnailReadOnlyCache AlbumArts { get; }

        IEditSession Edit();

        event EventHandler<EventArgs> Refreshed;

        void Search(SearchQuery query, List<SearchItem> results);

        IList<SearchItem> Search(SearchQuery query);

        IPlaylistManager Playlists { get; }
    }
}

using OnePlayer.Data.Access;
using System;

namespace OnePlayer.Data
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

        void Save();

        void Revert();
    }

    public interface IMusicMetadata : IDisposable
    {
        IAlbumReadOnlyAccessor Albums { get; }

        IArtistReadOnlyAccessor Artists { get; }

        IGenreReadOnlyAccessor Genres { get; }

        ITrackReadOnlyAccessor Tracks { get; }

        IDriveItemReadOnlyAccessor DriveItems { get; }

        IIndexReadOnlyAccessor Index { get; }

        IThumbnailInfoReadOnlyAccessor Thumbnails { get; }

        IEditSession Edit();

        event EventHandler<EventArgs> Refreshed;
    }
}

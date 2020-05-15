using System;

namespace OnePlayer.Data
{
    public interface IMusicMetadata : IDisposable
    {
        IAlbumAccessor Albums { get; }

        IArtistAccessor Artists { get; }

        IGenreAccessor Genres { get; }

        ITrackAccessor Tracks { get; }

        IDriveItemAccessor DriveItems { get; }

        IIndexAccessor Index { get; }

        IThumbnailInfoAccessor Thumbnails { get; }

        void Save();

        void Rollback();
    }
}

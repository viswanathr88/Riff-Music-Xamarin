using System;

namespace OnePlayer.Data
{
    public interface IMusicDataContext : IDisposable
    {
        IAlbumAccessor Albums { get; }

        IArtistAccessor Artists { get; }

        IGenreAccessor Genres { get; }

        ITrackAccessor Tracks { get; }

        IDriveItemAccessor DriveItems { get; }

        IIndexAccessor Index { get; }

        IThumbnailInfoAccessor Thumbnails { get; }

        void Migrate();

        void Save(bool beginNewTransaction = true);

        void Rollback();

        void RemoveOrphans();
    }
}

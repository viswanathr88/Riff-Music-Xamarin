using Microsoft.Data.Sqlite;
using System;

namespace Riff.Data.Sqlite
{
    internal sealed class MusicMetadataEditSession : IEditSession
    {
        private readonly SqliteConnection connection;
        private readonly IEditSessionHandler handler;
        private SqliteTransaction transaction;

        public MusicMetadataEditSession(SqliteConnection connection, IEditSessionHandler handler)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
            this.handler = handler;
            this.transaction = this.connection.BeginTransaction();
        }

        public IAlbumAccessor Albums { get; set; }

        public IArtistAccessor Artists { get; set; }

        public IGenreAccessor Genres { get; set; }

        public ITrackAccessor Tracks { get; set; }

        public IDriveItemAccessor DriveItems { get; set; }

        public IIndexAccessor Index { get; set; }

        public IThumbnailInfoAccessor Thumbnails { get; set; }

        public IThumbnailCache AlbumArts { get; set; }

        public IPlaylistAccessor Playlists { get; set; }

        public IPlaylistItemAccessor PlaylistItems { get; set; }

        public void Revert()
        {
            transaction.Rollback();
            transaction.Dispose();
            transaction = this.connection.BeginTransaction();
        }

        public void Save()
        {
            SaveInternal(true);
            handler?.HandleSessionSaved();
        }

        public void Dispose()
        {
            RemoveOrphans();
            SaveInternal(false);
            handler?.HandleSessionDisposed();
        }

        private void SaveInternal(bool newTransaction)
        {
            transaction.Commit();
            transaction.Dispose();
            if (newTransaction)
            {
                transaction = this.connection.BeginTransaction();
            }
        }

        private void RemoveOrphans()
        {
            Query("DELETE FROM PlaylistItem WHERE PlaylistId NOT IN (SELECT DISTINCT Id FROM Playlist)");
            Query("DELETE FROM PlaylistItem WHERE DriveItemId NOT IN (SELECT DISTINCT Id FROM DriveItem)");
            Query("DELETE FROM Track WHERE Id NOT IN (SELECT DISTINCT TrackId FROM DriveItem)");
            Query("DELETE FROM IndexedTrack WHERE docid NOT IN (SELECT DISTINCT TrackId FROM DriveItem)");
            Query("DELETE FROM Genre WHERE Id NOT IN (SELECT DISTINCT GenreId FROM Track)");
            Query("DELETE FROM Album WHERE Id NOT IN (SELECT DISTINCT AlbumId FROM Track)");
            Query("DELETE FROM ThumbnailInfo WHERE Id NOT IN (SELECT DISTINCT Id FROM Album)");
            Query("DELETE FROM Artist WHERE Id NOT IN (SELECT DISTINCT ArtistId FROM Album)");
        }

        private void Query(string query)
        {
            using (var command = transaction.Connection.CreateCommand())
            {
                command.CommandText = query;
                command.ExecuteNonQuery();
            }
        }
    }
}

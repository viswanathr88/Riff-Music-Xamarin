using SQLite;
using System;

namespace OnePlayer.Data.Sqlite
{
    sealed class MusicDatabaseContext : IMusicDataContext
    {
        private readonly SQLiteConnection connection;
        private readonly string dbPath;

        public MusicDatabaseContext(string dbPath)
        {
            if (string.IsNullOrEmpty(dbPath))
            {
                throw new ArgumentNullException(nameof(dbPath));
            }

            this.dbPath = dbPath;
            connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex);
            connection.BeginTransaction();

            // Create the table objects
            Albums = new AlbumTable(connection);
            Artists = new ArtistTable(connection);
            Genres = new GenreTable(connection);
            Tracks = new TrackTable(connection);
            DriveItems = new DriveItemTable(connection);
            Index = new IndexedTrackTable(connection);
            Thumbnails = new ThumbnailInfoTable(connection);
        }

        public IAlbumAccessor Albums { get; }

        public IArtistAccessor Artists { get; }

        public IGenreAccessor Genres { get; }

        public ITrackAccessor Tracks { get; }

        public IDriveItemAccessor DriveItems { get; }

        public IIndexAccessor Index { get; }

        public IThumbnailInfoAccessor Thumbnails { get; }

        public void Migrate()
        {
            Genres.EnsureCreated();
            Artists.EnsureCreated();
            Albums.EnsureCreated();
            Tracks.EnsureCreated();
            DriveItems.EnsureCreated();
            Index.EnsureCreated();
            Thumbnails.EnsureCreated();
            Save();
        }

        public void Rollback()
        {
            connection.Rollback();
            connection.BeginTransaction();
        }

        public void Dispose()
        {
            Save(false);
            connection?.Dispose();
        }

        public void Save(bool beginNewTransaction = true)
        {
            connection.Commit();
            if (beginNewTransaction)
            {
                connection.BeginTransaction();
            }
        }

        public void RemoveOrphans()
        {
            connection.Query<int>($"DELETE FROM {nameof(Track)} WHERE {nameof(Track.Id)} NOT IN (SELECT DISTINCT {nameof(DriveItem.TrackId)} FROM {nameof(DriveItem)})");
            connection.Query<int>($"DELETE FROM {nameof(IndexedTrack)} WHERE docid NOT IN (SELECT DISTINCT {nameof(DriveItem.TrackId)} FROM {nameof(DriveItem)})");
            connection.Query<int>($"DELETE FROM {nameof(ThumbnailInfo)} WHERE {nameof(ThumbnailInfo.Id)} NOT IN (SELECT DISTINCT {nameof(DriveItem.TrackId)} FROM {nameof(DriveItem)})");
            connection.Query<int>($"DELETE FROM {nameof(Genre)} WHERE {nameof(Genre.Id)} NOT IN (SELECT DISTINCT {nameof(Track.GenreId)} FROM {nameof(Track)})");
            connection.Query<int>($"DELETE FROM {nameof(Album)} WHERE {nameof(Album.Id)} NOT IN (SELECT DISTINCT {nameof(Track.AlbumId)} FROM {nameof(Track)})");
            connection.Query<int>($"DELETE FROM {nameof(Artist)} WHERE {nameof(Artist.Id)} NOT IN (SELECT DISTINCT {nameof(Album.ArtistId)} FROM {nameof(Album)})");
        }
    }
}

using OnePlayer.Data;
using SQLite;
using System;

namespace OnePlayer.Database
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
        }

        public IAlbumAccessor Albums { get; }

        public IArtistAccessor Artists { get; }

        public IGenreAccessor Genres { get; }

        public ITrackAccessor Tracks { get; }

        public IDriveItemAccessor DriveItems { get; }

        public void Migrate()
        {
            using (var conn = new SQLiteConnection(dbPath, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex))
            {
                connection.CreateTable<Genre>();
                connection.CreateTable<Artist>();
                connection.CreateTable<Album>();
                connection.CreateTable<Track>();
                connection.CreateTable<DriveItem>();
            }
        }

        public void Rollback()
        {
            connection.Rollback();
            connection.BeginTransaction();
        }

        public void Dispose()
        {
            connection.Commit();
            connection.Close();
        }

        public void Save()
        {
            connection.Commit();
            connection.BeginTransaction();
        }
    }
}

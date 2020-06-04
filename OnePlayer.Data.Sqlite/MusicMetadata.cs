using Microsoft.Data.Sqlite;
using OnePlayer.Data.Access;
using System;
using System.Linq;

namespace OnePlayer.Data.Sqlite
{
    public sealed class MusicMetadata : IMusicMetadata, IEditSessionHandler
    {
        private static readonly Version LatestVersion = Version.AddIndexes;
        private readonly SqliteConnection connection;
        private readonly DataExtractor extractor = new DataExtractor();

        private readonly ArtistTable artistTable;
        private readonly GenreTable genreTable;
        private readonly AlbumTable albumTable;
        private readonly TrackTable tracksTable;
        private readonly DriveItemTable driveItemTable;
        private readonly IndexedTrackTable indexedTrackTable;
        private readonly ThumbnailInfoTable thumbnailInfoTable;

        public event EventHandler<EventArgs> Refreshed;

        public MusicMetadata(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            var builder = new SqliteConnectionStringBuilder
            {
                Mode = SqliteOpenMode.ReadWriteCreate,
                DataSource = path,
                Cache = SqliteCacheMode.Shared
            };

            this.connection = new SqliteConnection(builder.ToString());
            this.connection.Open();

            // Enable object cache
            extractor.DisableCache = false;

            artistTable = new ArtistTable(connection, extractor);
            genreTable = new GenreTable(connection, extractor);
            albumTable = new AlbumTable(connection, extractor);
            tracksTable = new TrackTable(connection, extractor);
            driveItemTable = new DriveItemTable(connection, extractor);
            indexedTrackTable = new IndexedTrackTable(connection);
            thumbnailInfoTable = new ThumbnailInfoTable(connection);

            Version version = GetVersion();

            if (version != LatestVersion)
            {
                var versions = Enum.GetValues(typeof(Version)).Cast<Version>().ToList();
                var currentVersionIndex = versions.IndexOf(version);
                var versionRange = versions.GetRange(currentVersionIndex + 1, versions.Count - currentVersionIndex - 1);

                foreach (var ver in versionRange)
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            artistTable.HandleUpgrade(ver);
                            genreTable.HandleUpgrade(ver);
                            albumTable.HandleUpgrade(ver);
                            tracksTable.HandleUpgrade(ver);
                            driveItemTable.HandleUpgrade(ver);
                            indexedTrackTable.HandleUpgrade(ver);
                            thumbnailInfoTable.HandleUpgrade(ver);
                            
                            SetVersion(ver);
                            transaction.Commit();
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
        }

        public IAlbumReadOnlyAccessor Albums => albumTable;

        public IArtistReadOnlyAccessor Artists => artistTable;

        public IGenreReadOnlyAccessor Genres => genreTable;

        public ITrackReadOnlyAccessor Tracks => tracksTable;

        public IDriveItemReadOnlyAccessor DriveItems => driveItemTable;

        public IIndexReadOnlyAccessor Index => indexedTrackTable;

        public IThumbnailInfoReadOnlyAccessor Thumbnails => thumbnailInfoTable;

        public void Dispose()
        {
            connection?.Dispose();
        }

        public Version GetVersion()
        {
            Version version = Version.None;
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "PRAGMA user_version";
                var user_version_obj = command.ExecuteScalar();

                if (user_version_obj != null)
                {
                    version = (Version)Convert.ToInt32(user_version_obj);
                }
            }

            return version;
        }

        private void SetVersion(Version version)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"PRAGMA user_version = {(int)version}";
                command.ExecuteNonQuery();
            }
        }

        public IEditSession Edit()
        {
            return new MusicMetadataEditSession(connection, this)
            {
                Albums = albumTable,
                Artists = artistTable,
                Genres = genreTable,
                Tracks = tracksTable,
                DriveItems = driveItemTable,
                Index = indexedTrackTable,
                Thumbnails = thumbnailInfoTable
            };
        }

        public void HandleSessionSaved()
        {
            Refreshed?.Invoke(this, EventArgs.Empty);
        }

        public void HandleSessionDisposed()
        {
            
        }
    }
}

using Microsoft.Data.Sqlite;
using Riff.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Version = Riff.Data.Sqlite.Version;

namespace Riff.Data
{
    public sealed class MusicLibrary : IMusicLibrary, IEditSessionHandler
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
        private readonly ThumbnailCache thumbnailCache;

        public event EventHandler<EventArgs> Refreshed;

        public MusicLibrary(string path, string name)
        {
            var fullPath = Path.Combine(path, name);
            var builder = new SqliteConnectionStringBuilder
            {
                Mode = SqliteOpenMode.ReadWriteCreate,
                DataSource = fullPath,
                Cache = SqliteCacheMode.Shared
            };

            this.connection = new SqliteConnection(builder.ToString());
            this.connection.Open();

            // Enable object cache
            extractor.DisableCache = true;

            artistTable = new ArtistTable(connection, extractor);
            genreTable = new GenreTable(connection, extractor);
            albumTable = new AlbumTable(connection, extractor);
            tracksTable = new TrackTable(connection, extractor);
            driveItemTable = new DriveItemTable(connection, extractor);
            indexedTrackTable = new IndexedTrackTable(connection, extractor);
            thumbnailInfoTable = new ThumbnailInfoTable(connection);
            thumbnailCache = new ThumbnailCache(Path.Combine(path, "Thumbnails", "Albums"));
            Playlists = new PlaylistManager(Path.Combine(path, "Playlists"));

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

        public IThumbnailReadOnlyCache AlbumArts => thumbnailCache;

        public IPlaylistManager Playlists { get; }

        public void Dispose()
        {
            connection?.Dispose();
        }

        internal Version GetVersion()
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
                Thumbnails = thumbnailInfoTable,
                AlbumArts = thumbnailCache
            };
        }

        public void HandleSessionSaved()
        {
            Refreshed?.Invoke(this, EventArgs.Empty);
        }

        public void HandleSessionDisposed()
        {
            
        }

        public void Search(SearchQuery query, List<SearchItem> results)
        {
            foreach (var artist in Index.FindMatchingArtists(query.Term, query.MaxArtistCount))
            {
                results.Add(new SearchItem { Id = artist.Id, Type = SearchItemType.Artist, Name = artist.ArtistName, Description = artist.TrackCount.ToString(), Rank = artist.Rank });
            }

            foreach (var genre in Index.FindMatchingGenres(query.Term, query.MaxGenreCount))
            {
                results.Add(new SearchItem { Id = genre.Id, Type = SearchItemType.Genre, Name = genre.GenreName, Description = genre.TrackCount.ToString(), Rank = genre.Rank });
            }

            foreach (var album in Index.FindMatchingAlbums(query.Term, query.MaxAlbumCount))
            {
                results.Add(new SearchItem() { Id = album.Id, Type = SearchItemType.Album, Name = album.AlbumName, Description = album.ArtistName, Rank = album.Rank });
            }

            var tracks = Index.FindMatchingTracks(query.Term, query.MaxTrackCount);
            foreach (var track in Index.FindMatchingTracks(query.Term, query.MaxTrackCount))
            {
                results.Add(new SearchItem() { Id = track.Id, Type = SearchItemType.Track, Name = track.TrackName, Description = track.TrackArtist, Rank = track.Rank, ParentId = track.AlbumId });
            }

            if (query.MaxTrackCount > tracks.Count)
            {
                int diff = query.MaxTrackCount - tracks.Count;
                foreach (var track in Index.FindMatchingTracksWithArtists(query.Term, diff))
                {
                    results.Add(new SearchItem() { Id = track.Id, Type = SearchItemType.TrackArtist, Name = track.TrackName, Description = track.TrackArtist, Rank = track.Rank, ParentId = track.AlbumId });
                }
            }

            results.Sort(new SearchItemComparer());
        }

        public IList<SearchItem> Search(SearchQuery query)
        {
            List<SearchItem> searchItems = new List<SearchItem>();
            Search(query, searchItems);
            return searchItems;
        }
    }
}

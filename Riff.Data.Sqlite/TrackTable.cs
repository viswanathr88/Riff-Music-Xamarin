using Microsoft.Data.Sqlite;
using Riff.Data.Access;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Riff.Data.Sqlite
{
    public sealed class TrackTable : ITrackAccessor, ITable
    {
        private readonly SqliteConnection connection;
        private readonly DataExtractor extractor;
        private const string tableName = "Track";

        public TrackTable(SqliteConnection connection, DataExtractor extractor)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
            this.extractor = extractor;
        }

        public string Name => tableName;

        public void HandleUpgrade(Version version)
        {
            if (version == Version.Initial)
            {
                using (var command = connection.CreateCommand())
                {
                    var builder = new StringBuilder();
                    builder.AppendLine($"CREATE TABLE {tableName}");
                    builder.AppendLine("(");
                    builder.AppendLine("Id INTEGER PRIMARY KEY AUTOINCREMENT,");
                    builder.AppendLine("Title VARCHAR,");
                    builder.AppendLine("Number INTEGER,");
                    builder.AppendLine("Artist VARCHAR,");
                    builder.AppendLine("Bitrate INTEGER,");
                    builder.AppendLine("Duration INTEGER,");
                    builder.AppendLine("Composers VARCHAR,");
                    builder.AppendLine("ReleaseYear INTEGER,");
                    builder.AppendLine("AlbumId INTEGER,");
                    builder.AppendLine("GenreId INTEGER,");
                    builder.AppendLine("FOREIGN KEY(AlbumId) REFERENCES Album(Id),");
                    builder.AppendLine("FOREIGN KEY(GenreId) REFERENCES Genre(Id)");
                    builder.AppendLine(")");

                    command.CommandText = builder.ToString();
                    command.ExecuteNonQuery();
                }
            }

            if (version == Version.AddIndexes)
            {
                using (var command = connection.CreateCommand())
                {
                    var builder = new StringBuilder();
                    builder.AppendLine($"CREATE INDEX Idx_{Name}_Title ON {Name}(Title);");
                    builder.AppendLine($"CREATE INDEX Idx_{Name}_ReleaseYear ON {Name}(ReleaseYear);");
                    builder.AppendLine($"CREATE INDEX Idx_{Name}_Duration ON {Name}(Duration);");
                    builder.AppendLine($"CREATE INDEX Idx_{Name}_Artist ON {Name}(Artist)");

                    command.CommandText = builder.ToString();
                    command.ExecuteNonQuery();
                }
            }
        }

        public Track Add(Track track)
        {
            if (track == null)
            {
                throw new ArgumentNullException(nameof(track));
            }

            if (track.Id.HasValue)
            {
                throw new ArgumentException(nameof(track.Id));
            }

            if (track.Album == null || !track.Album.Id.HasValue)
            {
                throw new ArgumentNullException(nameof(track.Album));
            }

            if (track.Genre == null || !track.Genre.Id.HasValue)
            {
                throw new ArgumentNullException(nameof(track.Genre));
            }

            using (var command = connection.CreateCommand())
            {
                var builder = new StringBuilder();
                builder.AppendLine($"INSERT INTO {tableName}");
                builder.AppendLine("(Title, Number, Artist, Bitrate, Duration, Composers, ReleaseYear, AlbumId, GenreId)");
                builder.AppendLine("VALUES(@Title, @Number, @Artist, @Bitrate, @Duration, @Composers, @ReleaseYear, @AlbumId, @GenreId);");
                builder.AppendLine("select last_insert_rowid()");

                command.CommandText = builder.ToString();
                AddParameters(track, command);

                track.Id = (long)command.ExecuteScalar();
            }

            return track;
        }

        public Track Get(long id)
        {
            return Get(new TrackAccessOptions() { TrackFilter = id }).FirstOrDefault();
        }

        public IList<Track> Get(TrackAccessOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.AlbumArtistFilter.HasValue)
            {
                options.IncludeAlbum = true;
            }

            IList<Track> tracks = new List<Track>();
            using (var command = connection.CreateCommand())
            {
                StringBuilder builder = new StringBuilder();
                ApplySelect(options, builder);
                ApplyFilters(options, command, builder);
                ApplySorting(options, builder);
                ApplyLimits(options, builder);

                command.CommandText = builder.ToString();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tracks.Add(extractor.ExtractTrack(reader));
                    }
                }
            }

            return tracks;
        }

        public Track Update(Track track)
        {
            if (track == null)
            {
                throw new ArgumentNullException(nameof(track));
            }

            if (!track.Id.HasValue)
            {
                throw new ArgumentNullException(nameof(track.Id));
            }

            if (track.Album == null || !track.Album.Id.HasValue)
            {
                throw new ArgumentNullException(nameof(track.Album));
            }

            if (track.Genre == null || !track.Genre.Id.HasValue)
            {
                throw new ArgumentNullException(nameof(track.Genre));
            }

            using (var command = connection.CreateCommand())
            {
                var builder = new StringBuilder();
                builder.AppendLine($"UPDATE {tableName}");
                builder.AppendLine("SET Title = @Title,");
                builder.AppendLine("Number = @Number,");
                builder.AppendLine("Artist = @Artist,");
                builder.AppendLine("Bitrate = @Bitrate,");
                builder.AppendLine("Duration = @Duration,");
                builder.AppendLine("Composers = @Composers,");
                builder.AppendLine("ReleaseYear = @ReleaseYear,");
                builder.AppendLine("AlbumId = @AlbumId,");
                builder.AppendLine("GenreId = @GenreId");

                command.CommandText = builder.ToString();
                AddParameters(track, command);

                command.ExecuteNonQuery();
            }

            return track;
        }

        public long GetCount()
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT COUNT(*) FROM {tableName}";

                return (long)command.ExecuteScalar();
            }
        }

        public long GetCount(TrackAccessOptions options)
        {
            using (var command = connection.CreateCommand())
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine($"SELECT COUNT(*) FROM {tableName}");
                ApplyFilters(options, command, builder);

                command.CommandText = builder.ToString();
                return (long)command.ExecuteScalar();
            }
        }

        private static void ApplySelect(TrackAccessOptions options, StringBuilder builder)
        {
            IList<string> fields = new List<string>() { "track.Id AS TrackId", "track.Title AS TrackTitle", "track.Number AS TrackNumber", "track.Artist AS TrackArtist", "track.Bitrate AS TrackBitrate", "track.Duration AS TrackDuration", "track.Composers AS TrackComposers", "track.ReleaseYear AS TrackReleaseYear" };
            IList<string> joins = new List<string>();

            if (options.IncludeAlbum)
            {
                fields.Add("album.Id AS AlbumId");
                fields.Add("album.Name AS AlbumName");
                fields.Add("album.ReleaseYear AS AlbumReleaseYear");
                fields.Add("album.ArtistId AS ArtistId"); 
                fields.Add("album.GenreId AS GenreId");
                joins.Add("INNER JOIN Album album ON track.AlbumId = album.Id");
            }
            else
            {
                fields.Add("track.AlbumId AS AlbumId");
            }

            if (options.IncludeGenre)
            {
                fields.Add("genre.Id AS TrackGenreId");
                fields.Add("genre.Name AS TrackGenreName");
                joins.Add("INNER JOIN Genre genre ON track.GenreId = genre.Id");
            }
            else
            {
                fields.Add("track.GenreId AS TrackGenreId");
            }

            builder.AppendLine($"SELECT {string.Join(",", fields)}");
            builder.AppendLine($"FROM {tableName} {tableName.ToLower()}");
            foreach (var joinStmt in joins)
            {
                builder.AppendLine(joinStmt);
            }
        }

        private static void ApplyLimits(TrackAccessOptions options, StringBuilder builder)
        {
            if (options.Count.HasValue)
            {
                builder.Append($"LIMIT {options.Count}");
                if (options.StartPosition.HasValue)
                {
                    builder.Append($" OFFSET {options.StartPosition.Value}");
                }
            }
        }

        private static void ApplySorting(TrackAccessOptions options, StringBuilder builder)
        {
            if (options.SortType.HasValue)
            {
                builder.Append("ORDER BY ");
                string sortOrderStr = $"{(options.SortOrder == SortOrder.Ascending ? "ASC" : "DESC")}";
                if (options.SortType == TrackSortType.ReleaseYear)
                {
                    if (options.IncludeAlbum)
                    {
                        builder.AppendLine($"AlbumReleaseYear {sortOrderStr}, AlbumName ASC, TrackNumber ASC, TrackTitle ASC");
                    }
                    else
                    {
                        builder.AppendLine($"TrackReleaseYear {sortOrderStr}, TrackTitle ASC");
                    }
                }
                else
                {
                    builder.Append($"{options.SortType.Value} {sortOrderStr}");
                }
            }
        }

        private static void ApplyFilters(TrackAccessOptions options, SqliteCommand command, StringBuilder builder)
        {
            var filters = BuildFilters(command, options);

            if (filters.Count > 0)
            {
                builder.AppendLine($"WHERE {string.Join(" AND ", filters)}");
            }
        }

        private static IList<string> BuildFilters(SqliteCommand command, TrackAccessOptions options)
        {
            IList<string> filters = new List<string>();
            if (options.AlbumFilter.HasValue)
            {
                filters.Add("track.AlbumId = @AlbumId");
                command.Parameters.AddWithValue("@AlbumId", options.AlbumFilter.Value);
            }

            if (options.TrackFilter.HasValue)
            {
                filters.Add("track.Id = @TrackId");
                command.Parameters.AddWithValue("@TrackId", options.TrackFilter);
            }

            if (options.GenreFilter.HasValue)
            {
                filters.Add($"track.GenreId = @GenreId");
                command.Parameters.AddWithValue("@GenreId", options.GenreFilter);
            }

            if (options.AlbumArtistFilter.HasValue)
            {
                filters.Add($"album.ArtistId = @AlbumArtistId");
                command.Parameters.AddWithValue("@AlbumArtistId", options.AlbumArtistFilter.Value);
            }

            return filters;
        }

        private static void AddParameters(Track track, SqliteCommand command)
        {
            command.Parameters.AddWithValue("@Title", track.Title);
            command.Parameters.AddWithValue("@Number", track.Number);
            command.Parameters.AddWithNullableValue("@Artist", track.Artist);
            command.Parameters.AddWithValue("@Bitrate", track.Bitrate);
            command.Parameters.AddWithValue("@Duration", track.Duration.TotalMilliseconds);
            command.Parameters.AddWithNullableValue("@Composers", track.Composers);
            command.Parameters.AddWithValue("@ReleaseYear", track.ReleaseYear);
            command.Parameters.AddWithValue("@AlbumId", track.Album.Id);
            command.Parameters.AddWithValue("@GenreId", track.Genre.Id);
        }
    }
}

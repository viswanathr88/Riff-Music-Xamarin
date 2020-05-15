using Microsoft.Data.Sqlite;
using OnePlayer.Data.Access;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnePlayer.Data.Sqlite
{
    public sealed class TrackTable : ITrackAccessor, ITable
    {
        private readonly SqliteConnection connection;
        private const string tableName = "Track";

        public TrackTable(SqliteConnection connection)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
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
                        tracks.Add(ExtractTrack(reader));
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
            IList<string> fields = new List<string>() { "track.Id AS Id", "track.Title AS Title", "track.Number AS Number", "track.Artist AS TrackArtist", "track.Bitrate AS Bitrate", "track.Duration AS Duration", "track.Composers AS Composers", "track.ReleaseYear AS ReleaseYear", "track.AlbumId AS AlbumId", "track.genreId AS GenreId" };
            IList<string> joins = new List<string>();

            if (options.IncludeAlbum)
            {
                fields.Add("album.Name AS AlbumName, album.ReleaseYear AS AlbumReleaseYear, album.ArtistId AS AlbumArtistId, album.GenreId AS AlbumGenreId");
                joins.Add("INNER JOIN Album album ON track.AlbumId = album.Id");
            }

            if (options.IncludeGenre)
            {
                fields.Add("genre.Name AS GenreName");
                joins.Add("INNER JOIN Genre genre ON track.GenreId = genre.Id");
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
                builder.AppendLine($"ORDER BY {options.SortType.Value} {(options.SortOrder == SortOrder.Ascending ? "ASC" : "DESC")}");
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

            return filters;
        }

        private static void AddParameters(Track track, SqliteCommand command)
        {
            command.Parameters.AddWithValue("@Title", track.Title);
            command.Parameters.AddWithValue("@Number", track.Number);
            command.Parameters.AddWithNullableValue("@Artist", track.Artist);
            command.Parameters.AddWithValue("@Bitrate", track.Bitrate);
            command.Parameters.AddWithValue("@Duration", track.Duration);
            command.Parameters.AddWithNullableValue("@Composers", track.Composers);
            command.Parameters.AddWithValue("@ReleaseYear", track.ReleaseYear);
            command.Parameters.AddWithValue("@AlbumId", track.Album.Id);
            command.Parameters.AddWithValue("@GenreId", track.Genre.Id);
        }

        private Track ExtractTrack(SqliteDataReader reader)
        {
            int index = 0;
            Track track = new Track()
            {
                Id = reader.GetInt64(index++),
                Title = reader.IsDBNull(index) ? null : reader.GetString(index++),
                Number = reader.GetInt32(index++),
                Artist = reader.IsDBNull(index++) ? null : reader.GetString(index-1),
                Bitrate = reader.GetInt32(index++),
                Duration = reader.GetInt32(index++),
                Composers = reader.IsDBNull(index++) ? null : reader.GetString(index-1),
                ReleaseYear = reader.GetInt32(index++),
                Album = new Album()
                {
                    Id = reader.GetInt64(index++)
                },
                Genre = new Genre()
                {
                    Id = reader.GetInt64(index++)
                }
            };

            while (index < reader.FieldCount)
            {
                string columnName = reader.GetName(index);
                if (columnName == "AlbumName")
                {
                    track.Album.Name = reader.GetString(index);
                }
                else if (columnName == "AlbumReleaseYear")
                {
                    track.Album.ReleaseYear = reader.GetInt32(index);
                }
                else if (columnName == "AlbumArtistId")
                {
                    track.Album.Artist = new Artist() { Id = reader.GetInt64(index) };
                }
                else if (columnName == "AlbumGenreId")
                {
                    track.Album.Genre = new Genre() { Id = reader.GetInt64(index) };
                }
                else if (columnName == "GenreName")
                {
                    track.Genre.Name = reader.GetString(index);
                }

                index++;
            }

            return track;
        }
    }
}

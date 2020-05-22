using Microsoft.Data.Sqlite;
using OnePlayer.Data.Access;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnePlayer.Data.Sqlite
{
    public sealed class AlbumTable : IAlbumAccessor, ITable
    {
        private readonly SqliteConnection connection;
        private const string tableName = "Album";

        public AlbumTable(SqliteConnection connection)
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
                    builder.AppendLine("Name VARCHAR,");
                    builder.AppendLine("ReleaseYear INTEGER,");
                    builder.AppendLine("ArtistId INTEGER,");
                    builder.AppendLine("GenreId INTEGER,");
                    builder.AppendLine("FOREIGN KEY(ArtistId) REFERENCES Artist(Id),");
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
                    builder.AppendLine($"CREATE INDEX Idx_{Name}_Name ON {Name}(Name);");
                    builder.AppendLine($"CREATE INDEX Idx_{Name}_ReleaseYear ON {Name}(ReleaseYear)");

                    command.CommandText = builder.ToString();
                    command.ExecuteNonQuery();
                }
            }
        }

        public Album Add(Album album)
        {
            if (album.Id.HasValue)
            {
                throw new ArgumentException(nameof(album.Id));
            }

            if (album.Artist == null || !album.Artist.Id.HasValue)
            {
                throw new ArgumentNullException(nameof(album.Artist));
            }

            if (album.Genre == null || !album.Genre.Id.HasValue)
            {
                throw new ArgumentNullException(nameof(album.Genre));
            }

            using (var command = connection.CreateCommand())
            {
                var builder = new StringBuilder();
                builder.AppendLine($"INSERT INTO {tableName}");
                builder.AppendLine("(Name, ReleaseYear, ArtistId, GenreId)");
                builder.AppendLine("VALUES(@Name, @ReleaseYear, @ArtistId, @GenreId);");
                builder.AppendLine("select last_insert_rowid()");

                command.CommandText = builder.ToString();
                AddParameters(album, command);

                album.Id = (long)command.ExecuteScalar();
            }

            return album;
        }

        public Album FindByArtist(long artistId, string albumName)
        {
            if (string.IsNullOrEmpty(albumName))
            {
                throw new ArgumentNullException(nameof(albumName));
            }

            return Get(new AlbumAccessOptions()
            {
                AlbumNameFilter = albumName,
                ArtistFilter = artistId
            }).FirstOrDefault();
        }

        public Album Get(long id)
        {
            return Get(new AlbumAccessOptions()
            {
                AlbumFilter = id
            }).FirstOrDefault();
        }

        public IList<Album> Get()
        {
            return Get(new AlbumAccessOptions());
        }

        public IList<Album> Get(AlbumSortType type, SortOrder order)
        {
            return Get(new AlbumAccessOptions() { SortType = type, SortOrder = order });
        }

        public IList<Album> Get(AlbumAccessOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            IList<Album> albums = new List<Album>();
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
                        albums.Add(ExtractAlbum(reader));
                    }
                }
            }

            return albums;
        }

        public long GetCount()
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT COUNT(*) FROM {tableName}";

                return (long)command.ExecuteScalar();
            }
        }

        public long GetCount(AlbumAccessOptions options)
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

        public Album Update(Album album)
        {
            if (!album.Id.HasValue)
            {
                throw new ArgumentException(nameof(album.Id));
            }

            if (album.Artist == null || !album.Artist.Id.HasValue)
            {
                throw new ArgumentNullException(nameof(album.Artist));
            }

            if (album.Genre == null || !album.Genre.Id.HasValue)
            {
                throw new ArgumentNullException(nameof(album.Genre));
            }

            using (var command = connection.CreateCommand())
            {
                var builder = new StringBuilder();
                builder.AppendLine($"UPDATE {tableName}");
                builder.AppendLine($"SET Name = @Name,");
                builder.AppendLine($"ReleaseYear = @ReleaseYear,");
                builder.AppendLine($"ArtistId = @ArtistId,");
                builder.AppendLine($"GenreId = @GenreId");
                builder.AppendLine($"WHERE Id = {album.Id}");

                command.CommandText = builder.ToString();
                AddParameters(album, command);
                command.ExecuteNonQuery();
            }

            return album;
        }

        private static void ApplySelect(AlbumAccessOptions options, StringBuilder builder)
        {
            IList<string> fields = new List<string>() { "album.Id AS Id", "album.Name AS Name", "album.ReleaseYear AS ReleaseYear", "album.ArtistId AS ArtistId", "album.GenreId AS GenreId" };
            IList<string> joins = new List<string>();

            if (options.IncludeArtist)
            {
                fields.Add("artist.Name AS ArtistName");
                joins.Add("INNER JOIN Artist artist ON artist.Id = album.ArtistId");
            }

            if (options.IncludeGenre)
            {
                fields.Add("genre.Name AS GenreName");
                joins.Add("INNER JOIN Genre genre ON album.GenreId = genre.Id");
            }

            builder.AppendLine($"SELECT {string.Join(",", fields)}");
            builder.AppendLine($"FROM {tableName} {tableName.ToLower()}");
            foreach (var joinStmt in joins)
            {
                builder.AppendLine(joinStmt);
            }
        }

        private static void ApplyLimits(AlbumAccessOptions options, StringBuilder builder)
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

        private static void ApplySorting(AlbumAccessOptions options, StringBuilder builder)
        {
            if (options.SortType.HasValue)
            {
                builder.Append("ORDER BY ");
                string sortOrderStr = $"{(options.SortOrder == SortOrder.Ascending ? "ASC" : "DESC")}";

                if (options.SortType == AlbumSortType.ReleaseYear)
                {
                    builder.AppendLine($"ReleaseYear {sortOrderStr}, Name ASC");
                }
                else
                {
                    builder.AppendLine($"{options.SortType.Value} {sortOrderStr}");
                }
            }
        }

        private static void ApplyFilters(AlbumAccessOptions options, SqliteCommand command, StringBuilder builder)
        {
            var filters = BuildFilters(command, options);

            if (filters.Count > 0)
            {
                builder.AppendLine($"WHERE {string.Join(" AND ", filters)}");
            }
        }

        private static IList<string> BuildFilters(SqliteCommand command, AlbumAccessOptions options)
        {
            IList<string> filters = new List<string>();
            if (options.AlbumFilter.HasValue)
            {
                filters.Add("album.Id = @AlbumId");
                command.Parameters.AddWithValue("@AlbumId", options.AlbumFilter.Value);
            }

            if (!string.IsNullOrEmpty(options.AlbumNameFilter))
            {
                filters.Add("album.Name = @AlbumName");
                command.Parameters.AddWithValue("@AlbumName", options.AlbumNameFilter);
            }

            if (options.ArtistFilter.HasValue)
            {
                filters.Add("album.ArtistId = @ArtistId");
                command.Parameters.AddWithValue("@ArtistId", options.ArtistFilter);
            }

            if (options.GenreFilter.HasValue)
            {
                filters.Add($"album.GenreId = @GenreId");
                command.Parameters.AddWithValue("@GenreId", options.GenreFilter);
            }

            return filters;
        }

        private Album ExtractAlbum(SqliteDataReader reader)
        {
            int index = 0;
            var album = new Album()
            {
                Id = reader.GetInt64(index++),
                Name = reader.IsDBNull(index++) ? null : reader.GetString(index - 1),
                ReleaseYear = reader.GetInt32(index++),
                Artist = new Artist()
                {
                    Id = reader.GetInt64(index++)
                },
                Genre = new Genre()
                {
                    Id = reader.GetInt64(index++),
                }
            };

            while (index < reader.FieldCount)
            {
                string columnName = reader.GetName(index);
                if (columnName == "ArtistName")
                {
                    album.Artist.Name = reader.IsDBNull(index) ? null : reader.GetString(index);
                }
                else if (columnName == "GenreName")
                {
                    album.Genre.Name = reader.IsDBNull(index) ? null : reader.GetString(index);
                }

                index++;
            }

            return album;
        }

        private static void AddParameters(Album album, SqliteCommand command)
        {
            command.Parameters.AddWithNullableValue("@Name", album.Name);
            command.Parameters.AddWithValue("@ReleaseYear", album.ReleaseYear);
            command.Parameters.AddWithValue("@ArtistId", album.Artist.Id.Value);
            command.Parameters.AddWithValue("@GenreId", album.Genre.Id.Value);
        }
    }
}

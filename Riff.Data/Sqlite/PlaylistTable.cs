using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Riff.Data.Sqlite
{
    sealed class PlaylistTable : IPlaylistAccessor, ITable
    {
        private readonly SqliteConnection connection;
        private readonly DataExtractor extractor;

        public PlaylistTable(SqliteConnection connection, DataExtractor extractor)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
            this.extractor = extractor;
        }

        public string Name { get; } = "Playlist";

        public void HandleUpgrade(Version version)
        {
            if (version == Version.AddPlaylists)
            {
                using (var command = connection.CreateCommand())
                {
                    var builder = new StringBuilder();
                    builder.AppendLine($"CREATE TABLE {Name}");
                    builder.AppendLine("(");
                    builder.AppendLine("Id INTEGER PRIMARY KEY AUTOINCREMENT,");
                    builder.AppendLine("Name VARCHAR UNIQUE NOT NULL,");
                    builder.AppendLine("LastModified INTEGER");
                    builder.AppendLine(");");
                    builder.AppendLine($"CREATE INDEX Idx_{Name}_Name ON {Name}(Name);");
                    builder.AppendLine($"CREATE INDEX Idx_{Name}_LastModified ON {Name}(LastModified);");

                    command.CommandText = builder.ToString();
                    command.ExecuteNonQuery();
                }
            }
        }

        public Playlist2 Add(Playlist2 playlist)
        {
            if (playlist.Id.HasValue)
            {
                throw new ArgumentException(nameof(playlist.Id));
            }

            if (string.IsNullOrEmpty(playlist.Name))
            {
                throw new ArgumentException(nameof(playlist.Name));
            }

            playlist.LastModified = DateTime.Now;

            using (var command = connection.CreateCommand())
            {
                var builder = new StringBuilder();
                builder.AppendLine($"INSERT INTO {Name}");
                builder.AppendLine("(Name,LastModified)");
                builder.AppendLine("VALUES(@Name, @LastModified);");
                builder.AppendLine("select last_insert_rowid()");

                command.CommandText = builder.ToString();
                AddParameters(playlist, command);

                playlist.Id = (long)command.ExecuteScalar();
            }

            return playlist;
        }

        public void Delete(long id)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"DELETE FROM {Name} WHERE Id = @Id;";
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
        }

        public Playlist2 Get(long id)
        {
            var options = new PlaylistAccessOptions()
            {
                PlaylistFilter = id,
            };

            return Get(options).FirstOrDefault();
        }

        public IList<Playlist2> Get(PlaylistAccessOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            IList<Playlist2> playlists = new List<Playlist2>();

            using (var command = connection.CreateCommand())
            {
                StringBuilder builder = new StringBuilder();
                ApplySelect(options, builder);
                ApplyFilters(options, command, builder);
                ApplySorting(options, builder);

                command.CommandText = builder.ToString();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        playlists.Add(extractor.ExtractPlaylist(reader));
                    }
                }
            }

            return playlists;
        }

        private void ApplySorting(PlaylistAccessOptions options, StringBuilder builder)
        {
            builder.AppendLine($"ORDER BY {options.SortType} {(options.SortOrder == SortOrder.Descending ? "DESC" : "ASC")}");
        }

        private void ApplyFilters(PlaylistAccessOptions options, SqliteCommand command, StringBuilder builder)
        {
            if (options.PlaylistFilter.HasValue)
            {
                builder.AppendLine("WHERE Id = @Id");
                command.Parameters.AddWithValue("@Id", options.PlaylistFilter);
            }
        }

        private void ApplySelect(PlaylistAccessOptions options, StringBuilder builder)
        {
            builder.AppendLine($"SELECT Id, Name, LastModified FROM {Name}");
        }

        public Playlist2 Update(Playlist2 playlist)
        {
            if (!playlist.Id.HasValue)
            {
                throw new ArgumentNullException(nameof(playlist.Id));
            }

            if (string.IsNullOrEmpty(playlist.Name))
            {
                throw new ArgumentNullException(nameof(playlist.Name));
            }

            using (var command = connection.CreateCommand())
            {
                var builder = new StringBuilder();
                builder.AppendLine($"UPDATE {Name}");
                builder.AppendLine("SET Name = @Name,");
                builder.AppendLine("LastModified = @LastModified");
                builder.AppendLine("WHERE Id = @Id");

                command.CommandText = builder.ToString();
                AddParameters(playlist, command);
                command.Parameters.AddWithValue("@Id", playlist.Id.Value);

                command.ExecuteNonQuery();
            }

            return playlist;
        }

        private void AddParameters(Playlist2 playlist, SqliteCommand command)
        {
            command.Parameters.AddWithNullableValue("@Name", playlist.Name);
            command.Parameters.AddWithValue("@LastModified", playlist.LastModified.Ticks);
        }
    }
}

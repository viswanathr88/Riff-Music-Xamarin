using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Riff.Data.Sqlite
{
    internal sealed class PlaylistItemAccessOptions
    {
        public long? PlaylistItemFilter { get; set; }
        public long? PlaylistFilter { get; set; }
        public bool IncludeDriveItem { get; set; }
    }

    internal sealed class PlaylistItemTable : ITable
    {
        private readonly SqliteConnection connection;
        private readonly DataExtractor extractor;

        public PlaylistItemTable(SqliteConnection connection, DataExtractor extractor)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
            this.extractor = extractor;
        }

        public string Name { get; } = "PlaylistItem";

        public void HandleUpgrade(Version version)
        {
            if (version == Version.AddPlaylists)
            {
                using (var command = connection.CreateCommand())
                {
                    var builder = new StringBuilder();
                    builder.AppendLine("CREATE TABLE PlaylistItem");
                    builder.AppendLine("(");
                    builder.AppendLine("Id INTEGER PRIMARY KEY AUTOINCREMENT,");
                    builder.AppendLine("PlaylistId INTEGER,");
                    builder.AppendLine("DriveItemId VARCHAR,");
                    builder.AppendLine("Previous INTEGER,");
                    builder.AppendLine("Next INTEGER,");
                    builder.AppendLine($"FOREIGN KEY(PlaylistId) REFERENCES Playlist(Id),");
                    builder.AppendLine("FOREIGN KEY(DriveItemId) REFERENCES DriveItem(Id)");
                    builder.AppendLine($"FOREIGN KEY(Previous) REFERENCES {Name}(Id)");
                    builder.AppendLine($"FOREIGN KEY(Next) REFERENCES {Name}(Id)");
                    builder.AppendLine(")");
                    builder.AppendLine($"CREATE INDEX Idx_PlaylistItem_SortOrder ON {Name}(SortField);");

                    command.CommandText = builder.ToString();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Add(PlaylistItem item)
        {
            Add(new List<PlaylistItem>() { item });
        }

        public void Add(IList<PlaylistItem> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var lastItem = GetLastItem();


            using (var command = connection.CreateCommand())
            {
                var builder = new StringBuilder();
                builder.AppendLine($"INSERT INTO {Name}");
                builder.AppendLine("(PlaylistId, DriveItemId, Previous, Next)");
                builder.AppendLine("VALUES(@PlaylistId, @DriveItemId, @Previous, @Next);");
                builder.AppendLine("select last_insert_rowid();");

                foreach (var item in items)
                {
                    // Set Previous 
                    item.Previous = lastItem?.Id;
                    
                    // Add parameters to command
                    AddParameters(item, command);
                    
                    // Execute command
                    item.Id = (long)command.ExecuteScalar();
                    command.Parameters.Clear();

                    // Set next on the last item and update
                    if (lastItem != null)
                    {
                        lastItem.Next = item.Id;
                        Update(lastItem);
                    }
                    
                    lastItem = item;
                }
            }
        }

        public IList<PlaylistItem> Get(PlaylistItemAccessOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            IList<PlaylistItem> items = new List<PlaylistItem>();
            using (var command = connection.CreateCommand())
            {
                StringBuilder builder = new StringBuilder();
                ApplySelect(options, builder);
                ApplyFilters(options, command, builder);

                command.CommandText = builder.ToString();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(extractor.ExtractPlaylistItem(reader));
                    }
                }
            }

            return items;
        }

        private void ApplyFilters(PlaylistItemAccessOptions options, SqliteCommand command, StringBuilder builder)
        {
            throw new NotImplementedException();
        }

        private void ApplySelect(PlaylistItemAccessOptions options, StringBuilder builder)
        {
            throw new NotImplementedException();
        }

        public void Delete(long id)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"DELETE FROM {Name} WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", id);

                command.ExecuteNonQuery();
            }
        }

        public void Update(PlaylistItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException();
            }

            if (!item.Id.HasValue || string.IsNullOrEmpty(item.DriveItem.Id))
            {
                throw new ArgumentNullException();
            }

            using (var command = connection.CreateCommand())
            {
                var builder = new StringBuilder();
                builder.AppendLine($"UPDATE {Name}");
                builder.AppendLine("SET PlaylistId = @PlaylistId,");
                builder.AppendLine("DriveItemId = @DriveItemId,");
                builder.AppendLine("Previous = @Previous,");
                builder.AppendLine("Next = @Next");
                builder.AppendLine("WHERE Id = @Id");

                command.CommandText = builder.ToString();
                AddParameters(item, command);
                command.Parameters.AddWithValue("@Id", item.Id);

                command.ExecuteNonQuery();
            }
        }

        private static void AddParameters(PlaylistItem item, SqliteCommand command)
        {
            command.Parameters.AddWithValue("@PlaylistId", item.PlaylistId);
            command.Parameters.AddWithValue("@DriveItemId", item.DriveItem.Id);
            command.Parameters.AddWithNullableValue("@Previous", item.Previous);
            command.Parameters.AddWithNullableValue("@Next", item.Next);
        }

        private PlaylistItem GetLastItem()
        {
            PlaylistItem item = null;
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT Id AS PlaylistItemId, PlaylistId AS PlaylistItemPlaylistId, DriveItemId, Previous AS PlaylistItemPrevious, Next AS PlaylistItemNext FROM {Name} WHERE Next IS NULL LIMIT 1";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        item = extractor.ExtractPlaylistItem(reader);
                    }
                }
            }

            return item;
        }
    }
}

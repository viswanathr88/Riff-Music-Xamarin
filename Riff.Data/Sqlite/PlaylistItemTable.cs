using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Riff.Extensions;
using System.Runtime.CompilerServices;

namespace Riff.Data.Sqlite
{
    internal sealed class PlaylistItemTable : ITable, IPlaylistItemAccessor
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
                    builder.AppendLine($"CREATE TABLE {Name}");
                    builder.AppendLine("(");
                    builder.AppendLine("Id INTEGER PRIMARY KEY AUTOINCREMENT,");
                    builder.AppendLine("PlaylistId INTEGER,");
                    builder.AppendLine("Previous INTEGER,");
                    builder.AppendLine("Next INTEGER,");
                    builder.AppendLine("DriveItemId VARCHAR,");
                    builder.AppendLine($"FOREIGN KEY(PlaylistId) REFERENCES Playlist(Id),");
                    builder.AppendLine($"FOREIGN KEY(Previous) REFERENCES {Name}(Id)");
                    builder.AppendLine($"FOREIGN KEY(Next) REFERENCES {Name}(Id)");
                    builder.AppendLine("FOREIGN KEY(DriveItemId) REFERENCES DriveItem(Id)");
                    builder.AppendLine(");");
                    builder.AppendLine($"CREATE INDEX Idx_{Name}_Previous ON {Name}(Previous);");
                    builder.AppendLine($"CREATE INDEX Idx_{Name}_Next ON {Name}(Next);");

                    command.CommandText = builder.ToString();
                    command.ExecuteNonQuery();
                }
            }
        }

        public PlaylistItem Add(Playlist playlist, PlaylistItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return Add(playlist, new List<PlaylistItem>() { item }).FirstOrDefault();
        }

        public IList<PlaylistItem> Add(Playlist playlist, IList<PlaylistItem> items)
        {
            if (playlist == null || !playlist.Id.HasValue)
            {
                throw new ArgumentNullException(nameof(playlist));
            }

            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var lastItem = GetLastItem(playlist.Id.Value);

            using (var command = connection.CreateCommand())
            {
                var builder = new StringBuilder();
                builder.AppendLine($"INSERT INTO {Name}");
                builder.AppendLine("(PlaylistId, Previous, Next, DriveItemId)");
                builder.AppendLine("VALUES(@PlaylistId, @Previous, @Next, @DriveItemId);");
                builder.AppendLine("select last_insert_rowid();");

                command.CommandText = builder.ToString();

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

            return items;
        }

        public PlaylistItem Get(long id)
        {
            var options = new PlaylistItemAccessOptions()
            {
                PlaylistItemFilter = id
            };

            return Get(options).FirstOrDefault();
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

                if (options.PlaylistFilter.HasValue && !options.PlaylistItemFilter.HasValue && !options.GetLastItemOnly)
                {
                    // These items belong to the same playlist. Sort them
                    items = Sort(items);
                }
            }

            return items;
        }

        public void Update(PlaylistItem item)
        {
            if (item == null || !item.Id.HasValue)
            {
                throw new ArgumentNullException();
            }

            if (item.DriveItem == null || string.IsNullOrEmpty(item.DriveItem.Id))
            {
                throw new ArgumentNullException();
            }

            using (var command = connection.CreateCommand())
            {
                var builder = new StringBuilder();
                builder.AppendLine($"UPDATE {Name}");
                builder.AppendLine("SET PlaylistId = @PlaylistId,");
                builder.AppendLine("Previous = @Previous,");
                builder.AppendLine("Next = @Next,");
                builder.AppendLine("DriveItemId = @DriveItemId");
                builder.AppendLine("WHERE Id = @Id");

                command.CommandText = builder.ToString();
                AddParameters(item, command);
                command.Parameters.AddWithValue("@Id", item.Id);

                command.ExecuteNonQuery();
            }
        }

        public void Delete(long id)
        {
            PlaylistItem currentItem = Get(id);
            if (currentItem != null)
            {
                PlaylistItem previousItem = currentItem.Previous.HasValue ? Get(currentItem.Previous.Value) : null;
                PlaylistItem nextItem = currentItem.Next.HasValue ? Get(currentItem.Next.Value) : null;

                if (previousItem != null && nextItem != null)
                {
                    previousItem.Next = nextItem.Id;
                    nextItem.Previous = previousItem.Id;
                }
                else if (previousItem == null && nextItem != null)
                {
                    nextItem.Previous = null;
                }
                else if (previousItem != null && nextItem == null)
                {
                    previousItem.Next = null;
                }

                if (previousItem != null)
                {
                    Update(previousItem);
                }

                if (nextItem != null)
                {
                    Update(nextItem);
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"DELETE FROM {Name} WHERE Id = @Id";
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete(Playlist playlist)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"DELETE FROM {Name} WHERE PlaylistId = @PlaylistId";
                command.Parameters.AddWithValue("@PlaylistId", playlist.Id);
                command.ExecuteNonQuery();
            }
        }

        public void Reorder(Playlist playlist, int newIndex, int oldIndex, int count)
        {
            if (playlist == null)
            {
                throw new ArgumentNullException(nameof(playlist));
            }

            IList<PlaylistItem> sourceItems = Get(new PlaylistItemAccessOptions() { PlaylistFilter = playlist.Id });

            if (sourceItems.Count > 0)
            {
                Reorder(sourceItems, newIndex, oldIndex, count);
            }
        }

        public void Reorder(IList<PlaylistItem> sourceItems, int newIndex, int oldIndex, int count)
        {
            if (sourceItems == null)
            {
                throw new ArgumentNullException(nameof(sourceItems));
            }

            if (newIndex < 0 || newIndex >= sourceItems.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(newIndex));
            }

            if (oldIndex < 0 || oldIndex >= sourceItems.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(oldIndex));
            }

            if (oldIndex + count > sourceItems.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            var firstItem = sourceItems[oldIndex];
            var lastItem = sourceItems[oldIndex + count - 1];
            var previousItem = (oldIndex - 1) < 0 ? null : sourceItems[oldIndex - 1];
            var nextItem = (oldIndex + 1) >= sourceItems.Count ? null : sourceItems[oldIndex + 1];

            var predecessorIndex = (oldIndex < newIndex) ? newIndex : newIndex - 1;
            var successorIndex = predecessorIndex + 1;
            var predecessor = (predecessorIndex < 0 ? null : sourceItems[predecessorIndex]);
            var successor = (successorIndex == sourceItems.Count ? null : sourceItems[successorIndex]);

            // First, patch the links to where the items were moved
            lastItem.Next = successor?.Id;
            firstItem.Previous = predecessor?.Id;

            // Finally, patch the links from where the items were removed
            _ = previousItem != null ? (previousItem.Next = nextItem?.Id) : null;
            _ = nextItem != null ? (nextItem.Previous = previousItem?.Id) : null;
            _ = predecessor != null ? (predecessor.Next = firstItem.Id) : null;
            _ = successor != null ? (successor.Previous = lastItem.Id) : null;

            UpdateInternal(firstItem);
            if (firstItem != lastItem)
            {
                UpdateInternal(lastItem);
            }
            UpdateInternal(previousItem);
            UpdateInternal(nextItem);
            UpdateInternal(predecessor);
            UpdateInternal(successor);
        }

        private void UpdateInternal(PlaylistItem item)
        {
            if (item != null)
            {
                Update(item);
            }
        }

        private static void AddParameters(PlaylistItem item, SqliteCommand command)
        {
            command.Parameters.AddWithValue("@PlaylistId", item.PlaylistId);
            command.Parameters.AddWithNullableValue("@Previous", item.Previous);
            command.Parameters.AddWithNullableValue("@Next", item.Next);
            command.Parameters.AddWithValue("@DriveItemId", item.DriveItem.Id);
        }

        private PlaylistItem GetLastItem(long playlistId)
        {
            var options = new PlaylistItemAccessOptions()
            {
                PlaylistFilter = playlistId,
                GetLastItemOnly = true
            };

            return Get(options).FirstOrDefault();
        }

        private void ApplyFilters(PlaylistItemAccessOptions options, SqliteCommand command, StringBuilder builder)
        {
            var filters = BuildFilters(command, options);

            if (filters.Count > 0)
            {
                builder.AppendLine($"WHERE {string.Join(" AND ", filters)}");
            }
        }

        private static IList<string> BuildFilters(SqliteCommand command, PlaylistItemAccessOptions options)
        {
            IList<string> filters = new List<string>();

            if (options.PlaylistItemFilter.HasValue)
            {
                filters.Add("playlistItem.Id = @Id");
                command.Parameters.AddWithValue("@Id", options.PlaylistItemFilter);
            }
            else if (options.PlaylistFilter.HasValue)
            {
                filters.Add("playlistItem.PlaylistId = @PlaylistId");
                if (options.GetLastItemOnly)
                {
                    filters.Add("playlistItem.Next IS NULL");
                }
                command.Parameters.AddWithValue("@PlaylistId", options.PlaylistFilter);
            }

            return filters;
        }

        private void ApplySelect(PlaylistItemAccessOptions options, StringBuilder builder)
        {
            IList<string> fields = new List<string>()
            {
                "playlistItem.Id AS PlaylistItemId",
                "playlistItem.PlaylistId AS PlaylistItemPlaylistId",
                "playlistItem.Previous AS PlaylistItemPrevious",
                "playlistItem.Next AS PlaylistItemNext",
                "playlistItem.DriveItemId AS DriveItemId"
            };

            IList<string> joins = new List<string>();

            if (options.IncludeDriveItem)
            {
                fields.Append(new List<string>()
                {
                    "driveItem.Name AS DriveItemName",
                    "driveItem.Description AS DriveItemDescription",
                    "driveItem.CTag AS DriveItemCTag",
                    "driveItem.ETag AS DriveItemETag",
                    "driveItem.AddedDate AS DriveItemAddedDate",
                    "driveItem.LastModified AS DriveItemLastModified",
                    "driveItem.DownloadUrl AS DriveItemDownloadUrl",
                    "driveItem.Size AS DriveItemSize",
                    "driveItem.Source AS DriveItemSource",
                    "track.Id AS TrackId",
                    "track.Title AS TrackTitle",
                    "track.Number AS TrackNumber",
                    "track.Artist AS TrackArtist",
                    "track.Bitrate AS TrackBitrate",
                    "track.Duration as TrackDuration",
                    "track.Composers AS TrackComposers",
                    "track.ReleaseYear AS TrackReleaseYear",
                    "track.AlbumId AS AlbumId",
                    "album.Name AS AlbumName",
                    "album.ReleaseYear AS AlbumReleaseYear",
                    "album.ArtistId as ArtistId",
                    "album.GenreId AS GenreId",
                    "track.GenreId AS TrackGenreId",
                    "genre.Name AS TrackGenreName"
                });

                joins.Append(new List<string>()
                {
                    "INNER JOIN DriveItem driveItem ON playlistItem.DriveItemId = driveItem.Id",
                    "INNER JOIN Track track ON track.Id = driveItem.TrackId",
                    "INNER JOIN Genre genre ON track.GenreId = genre.Id",
                    "INNER JOIN Album album ON track.AlbumId = album.Id",
                });
            }

            builder.AppendLine($"SELECT {string.Join(",", fields)}");
            builder.AppendLine($"FROM {Name} playlistItem");
            foreach (var joinStmt in joins)
            {
                builder.AppendLine(joinStmt);
            }
        }

        private IList<PlaylistItem> Sort(IList<PlaylistItem> items)
        {
            List<PlaylistItem> sortedItems = new List<PlaylistItem>();
            IDictionary<long, int> map = new Dictionary<long, int>();

            // Index the list
            long currentItemId = -1;
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                if (!item.Previous.HasValue)
                {
                    // This is the first item
                    sortedItems.Add(item);
                    currentItemId = item.Id.Value;
                }
                else
                {
                    map[item.Previous.Value] = i;
                }
            }

            // Sort using the index
            while (sortedItems.Count < items.Count)
            {
                var nextItem = items[map[currentItemId]];
                sortedItems.Add(nextItem);
                currentItemId = nextItem.Id.Value;
            }

            return sortedItems;
        }
    }
}

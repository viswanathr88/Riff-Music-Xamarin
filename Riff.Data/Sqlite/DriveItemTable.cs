using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Riff.Data.Sqlite
{
    internal sealed class DriveItemTable : IDriveItemAccessor, ITable
    {
        private readonly SqliteConnection connection;
        private readonly DataExtractor extractor;

        public DriveItemTable(SqliteConnection connection, DataExtractor extractor)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
            this.extractor = extractor;
        }

        public string Name { get; } = "DriveItem";

        public void HandleUpgrade(Version version)
        {
            if (version == Version.Initial)
            {
                using (var command = connection.CreateCommand())
                {
                    var builder = new StringBuilder();
                    builder.AppendLine($"CREATE TABLE {Name}");
                    builder.AppendLine("(");
                    builder.AppendLine("Id VARCHAR PRIMARY KEY,");
                    builder.AppendLine("Name VARCHAR,");
                    builder.AppendLine("Description VARCHAR,");
                    builder.AppendLine("CTag VARCHAR,");
                    builder.AppendLine("ETag VARCHAR,");
                    builder.AppendLine("AddedDate INTEGER,");
                    builder.AppendLine("LastModified INTEGER,");
                    builder.AppendLine("DownloadUrl VARCHAR,");
                    builder.AppendLine("Size INTEGER,");
                    builder.AppendLine("Source INTEGER,");
                    builder.AppendLine("TrackId INTEGER,");
                    builder.AppendLine("FOREIGN KEY(TrackId) REFERENCES Track(Id)");
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
                    builder.AppendLine($"CREATE INDEX Idx_{Name}_Source ON {Name}(Source);");
                    builder.AppendLine($"CREATE INDEX Idx_{Name}_TrackId ON {Name}(TrackId);");

                    command.CommandText = builder.ToString();
                    command.ExecuteNonQuery();
                }
            }
        }

        public DriveItem Add(DriveItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (string.IsNullOrEmpty(item.Id))
            {
                throw new ArgumentException(nameof(item.Id));
            }

            if (item.Track == null || !item.Track.Id.HasValue)
            {
                throw new ArgumentNullException(nameof(item.Track.Id));
            }

            using (var command = connection.CreateCommand())
            {
                var builder = new StringBuilder();
                builder.AppendLine($"INSERT INTO {Name}");
                builder.AppendLine("(Id, Name, Description, CTag, ETag, AddedDate, LastModified, DownloadUrl, Size, Source, TrackId)");
                builder.AppendLine("VALUES(@Id, @Name, @Description, @CTag, @ETag, @AddedDate, @LastModified, @DownloadUrl, @Size, @Source, @TrackId);");

                command.CommandText = builder.ToString();
                AddParameters(item, command, builder);

                command.ExecuteNonQuery();
            }

            return item;
        }

        public void Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"DELETE FROM {Name} WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", id);

                command.ExecuteNonQuery();
            }
        }

        public DriveItem Get(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var options = new DriveItemAccessOptions()
            {
                DriveItemFilter = id
            };
            return Get(options).FirstOrDefault();
        }

        public IList<DriveItem> Get(DriveItemAccessOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.AlbumFilter.HasValue || options.AlbumArtistFilter.HasValue)
            {
                options.IncludeTrack = true;
                options.IncludeTrackAlbum = true;
            }

            IList<DriveItem> items = new List<DriveItem>();
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
                        items.Add(extractor.ExtractDriveItem(reader));
                    }
                }
            }

            return items;
        }

        public DriveItem Update(DriveItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (string.IsNullOrEmpty(item.Id))
            {
                throw new ArgumentException(nameof(item.Id));
            }

            if (item.Track == null || !item.Track.Id.HasValue)
            {
                throw new ArgumentNullException(nameof(item.Track.Id));
            }

            if (string.IsNullOrEmpty(item.Name))
            {
                item.Name = null;
            }

            if (string.IsNullOrEmpty(item.Description))
            {
                item.Description = null;
            }

            using (var command = connection.CreateCommand())
            {
                var builder = new StringBuilder();
                builder.AppendLine($"UPDATE {Name}");
                builder.AppendLine("SET Name = @Name,");
                builder.AppendLine("Description= @Description,");
                builder.AppendLine("CTag = @CTag,");
                builder.AppendLine("ETag = @ETag,");
                builder.AppendLine("AddedDate = @AddedDate,");
                builder.AppendLine("LastModified = @LastModified,");
                builder.AppendLine("DownloadUrl = @DownloadUrl,");
                builder.AppendLine("Size = @Size,");
                builder.AppendLine("Source = @Source,");
                builder.AppendLine("TrackId = @TrackId");
                builder.AppendLine($"WHERE Id = @Id");
                AddParameters(item, command, builder);

                command.ExecuteNonQuery();
            }

            return item;
        }

        private static void ApplySelect(DriveItemAccessOptions options, StringBuilder builder)
        {
            IList<string> fields = new List<string>() { "item.Id AS DriveItemId", "item.Name AS DriveItemName", "item.Description AS DriveItemDescription", "item.CTag AS DriveItemCTag", "item.ETag AS DriveItemETag", "item.AddedDate AS DriveItemAddedDate", "item.LastModified AS DriveItemLastModified", "item.DownloadUrl AS DriveItemDownloadUrl", "item.Size AS DriveItemSize", "item.Source AS DriveItemSource" };
            IList<string> joins = new List<string>();

            if (options.IncludeTrack)
            {
                fields.Add("track.Id AS TrackId");
                fields.Add("track.Title AS TrackTitle");
                fields.Add("track.Number AS TrackNumber");
                fields.Add("track.Artist AS TrackArtist");
                fields.Add("track.Bitrate AS TrackBitrate");
                fields.Add("track.Duration as TrackDuration");
                fields.Add("track.Composers AS TrackComposers");
                fields.Add("track.ReleaseYear AS TrackReleaseYear");
                fields.Add("track.AlbumId AS AlbumId");

                if (options.IncludeTrackAlbum)
                {
                    fields.Add("album.Name AS AlbumName");
                    fields.Add("album.ReleaseYear AS AlbumReleaseYear");
                    fields.Add("album.ArtistId as ArtistId");
                    fields.Add("album.GenreId AS GenreId");
                    joins.Add("INNER JOIN Album album ON track.AlbumId = album.Id");
                }

                fields.Add("track.GenreId AS TrackGenreId");

                if (options.IncludeTrackGenre)
                {
                    fields.Add("genre.Name AS TrackGenreName");
                    joins.Add("INNER JOIN Genre genre ON track.GenreId = genre.Id");
                }

                joins.Add("INNER JOIN Track track ON track.Id = item.TrackId");
            }
            else
            {
                fields.Add("item.TrackId as TrackId");
            }

            builder.AppendLine($"SELECT {string.Join(",", fields)}");
            builder.AppendLine($"FROM DriveItem item");
            foreach (var joinStmt in joins)
            {
                builder.AppendLine(joinStmt);
            }
        }

        private static void ApplyLimits(DriveItemAccessOptions options, StringBuilder builder)
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

        private static void ApplySorting(DriveItemAccessOptions options, StringBuilder builder)
        {
            if (options.SortType.HasValue && options.IncludeTrack)
            {
                builder.Append("ORDER BY ");
                string sortOrderStr = $"{(options.SortOrder == SortOrder.Ascending ? "ASC" : "DESC")}";
                if (options.SortType == TrackSortType.ReleaseYear && options.IncludeTrackAlbum)
                {
                    builder.AppendLine($"AlbumReleaseYear {sortOrderStr}, AlbumName ASC, TrackNumber ASC, TrackTitle ASC");
                }
                else
                {
                    builder.Append($"{(!options.SortType.ToString().StartsWith("Track") ? "Track" : string.Empty)}{options.SortType.Value} {sortOrderStr}");
                }
            }
        }

        private static void ApplyFilters(DriveItemAccessOptions options, SqliteCommand command, StringBuilder builder)
        {
            var filters = BuildFilters(command, options);

            if (filters.Count > 0)
            {
                builder.AppendLine($"WHERE {string.Join(" AND ", filters)}");
            }
        }

        private static IList<string> BuildFilters(SqliteCommand command, DriveItemAccessOptions options)
        {
            IList<string> filters = new List<string>();
            
            if (!string.IsNullOrEmpty(options.DriveItemFilter))
            {
                filters.Add("Id = @Id");
                command.Parameters.AddWithValue("@Id", options.DriveItemFilter);
            }
            
            if (options.TrackFilter.HasValue)
            {
                filters.Add("TrackId = @TrackId");
                command.Parameters.AddWithValue("@TrackId", options.TrackFilter);
            }

            if (options.AlbumFilter.HasValue)
            {
                filters.Add("track.AlbumId = @AlbumId");
                command.Parameters.AddWithValue("@AlbumId", options.AlbumFilter.Value);
            }

            if (options.AlbumArtistFilter.HasValue)
            {
                filters.Add($"album.ArtistId = @AlbumArtistId");
                command.Parameters.AddWithValue("@AlbumArtistId", options.AlbumArtistFilter.Value);
            }

            return filters;
        }

        private static void AddParameters(DriveItem item, SqliteCommand command, StringBuilder builder)
        {
            command.CommandText = builder.ToString();
            command.Parameters.AddWithValue("@Id", item.Id);
            command.Parameters.AddWithNullableValue("@Name", item.Name);
            command.Parameters.AddWithNullableValue("@Description", item.Description);
            command.Parameters.AddWithValue("@CTag", item.CTag);
            command.Parameters.AddWithValue("@ETag", item.ETag);
            command.Parameters.AddWithValue("@AddedDate", item.AddedDate.Ticks);
            command.Parameters.AddWithValue("@LastModified", item.LastModified.Ticks);
            command.Parameters.AddWithValue("@DownloadUrl", item.DownloadUrl);
            command.Parameters.AddWithValue("@Size", item.Size);
            command.Parameters.AddWithValue("@Source", item.Source);
            command.Parameters.AddWithValue("@TrackId", item.Track.Id);
        }
    }
}

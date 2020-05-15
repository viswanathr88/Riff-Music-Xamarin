using Microsoft.Data.Sqlite;
using OnePlayer.Data.Access;
using System;
using System.Text;

namespace OnePlayer.Data.Sqlite
{
    public sealed class DriveItemTable : IDriveItemAccessor, ITable
    {
        private readonly SqliteConnection connection;

        public DriveItemTable(SqliteConnection connection)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
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
                builder.AppendLine("(Id, CTag, ETag, AddedDate, LastModified, DownloadUrl, Size, Source, TrackId)");
                builder.AppendLine("VALUES(@Id, @CTag, @ETag, @AddedDate, @LastModified, @DownloadUrl, @Size, @Source, @TrackId);");

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

            DriveItem item = null;

            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT * FROM {Name} WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", id);

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    item = ExtractDriveItem(reader);
                }
            }

            return item;
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

            using (var command = connection.CreateCommand())
            {
                var builder = new StringBuilder();
                builder.AppendLine($"UPDATE {Name}");
                builder.AppendLine("SET CTag = @CTag,");
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

        private static void AddParameters(DriveItem item, SqliteCommand command, StringBuilder builder)
        {
            command.CommandText = builder.ToString();
            command.Parameters.AddWithValue("@Id", item.Id);
            command.Parameters.AddWithValue("@CTag", item.CTag);
            command.Parameters.AddWithValue("@ETag", item.ETag);
            command.Parameters.AddWithValue("@AddedDate", item.AddedDate.Ticks);
            command.Parameters.AddWithValue("@LastModified", item.LastModified.Ticks);
            command.Parameters.AddWithValue("@DownloadUrl", item.DownloadUrl);
            command.Parameters.AddWithValue("@Size", item.Size);
            command.Parameters.AddWithValue("@Source", item.Source);
            command.Parameters.AddWithValue("@TrackId", item.Track.Id);
        }

        private DriveItem ExtractDriveItem(SqliteDataReader reader)
        {
            int index = 0;
            return new DriveItem()
            {
                Id = reader.GetString(index++),
                CTag = reader.GetString(index++),
                ETag = reader.GetString(index++),
                AddedDate = new DateTime(reader.GetInt64(index++)),
                LastModified = new DateTime(reader.GetInt64(index++)),
                DownloadUrl = reader.GetString(index++),
                Size = reader.GetInt32(index++),
                Source = (DriveItemSource)reader.GetInt32(index++),
                Track = new Track()
                {
                    Id = reader.GetInt64(index++)
                }
            };
        }
    }
}

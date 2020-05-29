using Microsoft.Data.Sqlite;
using OnePlayer.Data.Access;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnePlayer.Data.Sqlite
{
    public sealed class ThumbnailInfoTable : IThumbnailInfoAccessor, ITable
    {
        private readonly SqliteConnection connection;

        public ThumbnailInfoTable(SqliteConnection connection)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public string Name { get; } = "ThumbnailInfo";
        
        public ThumbnailInfo Add(ThumbnailInfo info)
        {
            if (!info.Id.HasValue)
            {
                throw new ArgumentException(nameof(info.Id));
            }

            using (var command = connection.CreateCommand())
            {
                var builder = new StringBuilder();
                builder.AppendLine($"INSERT INTO {Name} (Id, SmallUrl, MediumUrl, LargeUrl, Cached, AttemptCount)");
                builder.AppendLine("VALUES (@Id, @SmallUrl, @MediumUrl, @LargeUrl, @Cached, @AttemptCount);");
                builder.AppendLine("select last_insert_rowid();");

                command.CommandText = builder.ToString();
                AddParameters(info, command);

                info.Id = (long)command.ExecuteScalar();
            }

            return info;
        }

        public ThumbnailInfo Get(long id)
        {
            ThumbnailInfo info = null;

            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT * FROM {Name} WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", id);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        info = ExtractThumbnailInfo(reader);
                    }
                }
            }

            return info;
        }

        public IList<ThumbnailInfo> GetUncached()
        {
            IList<ThumbnailInfo> thumbnails = new List<ThumbnailInfo>();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT * FROM {Name} WHERE Cached = @Cached";
                command.Parameters.AddWithValue("@Cached", 0);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        thumbnails.Add(ExtractThumbnailInfo(reader));
                    }
                }
            }

            return thumbnails;
        }

        public void HandleUpgrade(Version version)
        {
            if (version == Version.Initial)
            {
                using (var command = connection.CreateCommand())
                {
                    var builder = new StringBuilder();
                    builder.AppendLine($"CREATE TABLE {Name}");
                    builder.AppendLine("(");
                    builder.AppendLine("Id INTEGER PRIMARY KEY,");
                    builder.AppendLine("SmallUrl VARCHAR,");
                    builder.AppendLine("MediumUrl VARCHAR,");
                    builder.AppendLine("LargeUrl VARCHAR,");
                    builder.AppendLine("Cached INTEGER,");
                    builder.AppendLine("AttemptCount INTEGER");
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
                    builder.AppendLine($"CREATE INDEX Idx_{Name}_Cached ON {Name}(Cached);");

                    command.CommandText = builder.ToString();
                    command.ExecuteNonQuery();
                }
            }
        }

        public ThumbnailInfo Update(ThumbnailInfo info)
        {
            if (!info.Id.HasValue)
            {
                throw new ArgumentException(nameof(info.Id));
            }

            using (var command = connection.CreateCommand())
            {
                var builder = new StringBuilder();
                builder.AppendLine($"UPDATE {Name}");
                builder.AppendLine("SET SmallUrl = @SmallUrl,");
                builder.AppendLine("MediumUrl = @MediumUrl,");
                builder.AppendLine("LargeUrl = @LargeUrl,");
                builder.AppendLine("Cached = @Cached,");
                builder.AppendLine("AttemptCount = @AttemptCount");
                builder.AppendLine($"WHERE Id = @Id");

                command.CommandText = builder.ToString();
                AddParameters(info, command);
                command.ExecuteNonQuery();
            }

            return info;
        }

        private ThumbnailInfo ExtractThumbnailInfo(SqliteDataReader reader)
        {
            return new ThumbnailInfo()
            {
                Id = reader.GetInt64(0),
                SmallUrl = reader.IsDBNull(1) ? null : reader.GetString(1),
                MediumUrl = reader.IsDBNull(2) ? null : reader.GetString(2),
                LargeUrl = reader.IsDBNull(3) ? null : reader.GetString(3),
                Cached = Convert.ToBoolean(reader.GetInt32(4)),
                AttemptCount = reader.GetInt32(5),
            };
        }

        private static void AddParameters(ThumbnailInfo info, SqliteCommand command)
        {
            command.Parameters.AddWithValue("@Id", info.Id);
            command.Parameters.AddWithNullableValue("@SmallUrl", info.SmallUrl);
            command.Parameters.AddWithNullableValue("@MediumUrl", info.MediumUrl);
            command.Parameters.AddWithNullableValue("@LargeUrl", info.LargeUrl);
            command.Parameters.AddWithValue("@Cached", info.Cached);
            command.Parameters.AddWithValue("@AttemptCount", info.AttemptCount);
        }
    }
}

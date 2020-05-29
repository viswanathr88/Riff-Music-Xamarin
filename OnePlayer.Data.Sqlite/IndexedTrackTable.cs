using Microsoft.Data.Sqlite;
using OnePlayer.Data.Access;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnePlayer.Data.Sqlite
{
    public sealed class IndexedTrackTable : IIndexAccessor, ITable
    {
        private readonly SqliteConnection connection;

        public IndexedTrackTable(SqliteConnection connection)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public string Name { get; } = "IndexedTrack";

        public void HandleUpgrade(Version version)
        {
            if (version == Version.Initial)
            {
                using (var command = connection.CreateCommand())
                {
                    var builder = new StringBuilder();
                    builder.AppendLine($"CREATE VIRTUAL TABLE {Name} USING FTS4");
                    builder.AppendLine("(");
                    builder.AppendLine("ArtistName VARCHAR,");
                    builder.AppendLine("AlbumName VARCHAR,");
                    builder.AppendLine("TrackName VARCHAR,");
                    builder.AppendLine("TrackArtist VARCHAR,");
                    builder.AppendLine("GenreName VARCHAR,");
                    builder.AppendLine("AlbumId VARCHAR,");
                    builder.AppendLine("ArtistId VARCHAR,");
                    builder.AppendLine("GenreId VARCHAR");
                    builder.AppendLine(")");

                    command.CommandText = builder.ToString();
                    command.ExecuteNonQuery();
                }
            }
        }

        public IndexedTrack Add(IndexedTrack track)
        {
            if (track == null)
            {
                throw new ArgumentNullException(nameof(track));
            }

            if (!track.Id.HasValue)
            {
                throw new ArgumentException(nameof(track.Id));
            }

            using (var command = connection.CreateCommand())
            {
                var builder = new StringBuilder();
                builder.AppendLine($"INSERT INTO {Name} (docid, ArtistName, AlbumName, TrackName, TrackArtist, GenreName, AlbumId, ArtistId, GenreId)");
                builder.AppendLine("VALUES (@Id, @ArtistName, @AlbumName, @TrackName, @TrackArtist, @GenreName, @AlbumId, @ArtistId, @GenreId);");
                builder.AppendLine("select last_insert_rowid();");

                command.CommandText = builder.ToString();
                AddParameters(track, command);

                track.Id = (long)command.ExecuteScalar();
            }

            return track;
        }

        public IndexedTrack Get(long id)
        {
            IndexedTrack track = null;

            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT docid AS Id, ArtistName, AlbumName, TrackName, TrackArtist, GenreName, AlbumId, ArtistId, GenreId FROM {Name} WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", id);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        track = ExtractIndexedTrack(reader);
                    }
                }
            }

            return track;
        }

        public IndexedTrack Update(IndexedTrack track)
        {
            if (track == null)
            {
                throw new ArgumentNullException(nameof(track));
            }

            if (!track.Id.HasValue)
            {
                throw new ArgumentNullException(nameof(track.Id));
            }

            using (var command = connection.CreateCommand())
            {
                var builder = new StringBuilder();
                builder.AppendLine($"UPDATE {Name}");
                builder.AppendLine("SET ArtistName = @ArtistName,");
                builder.AppendLine("AlbumName = @AlbumName,");
                builder.AppendLine("TrackName = @TrackName,");
                builder.AppendLine("TrackArtist = @TrackArtist,");
                builder.AppendLine("GenreName = @GenreName,");
                builder.AppendLine("AlbumId = @AlbumId,");
                builder.AppendLine("ArtistId = @ArtistId,");
                builder.AppendLine("GenreId = @GenreId");
                builder.AppendLine("WHERE docid = @Id");

                command.CommandText = builder.ToString();
                AddParameters(track, command);

                command.ExecuteNonQuery();
            }

            return track;
        }

        public IList<IndexedTrackWithOffset> Search(string term)
        {
            throw new System.NotImplementedException();
        }

        public IList<AlbumQueryItem> FindMatchingAlbums(string term, int? maxCount)
        {
            List<AlbumQueryItem> albums = new List<AlbumQueryItem>();
            if (!string.IsNullOrEmpty(term))
            {
                using (var command = connection.CreateCommand())
                {
                    var builder = new StringBuilder();
                    builder.AppendLine("SELECT Id, AlbumName, ArtistName, Rank FROM");
                    {
                        builder.AppendLine("(SELECT CAST(AlbumId AS INTEGER) AS Id,");
                        builder.AppendLine("AlbumName,");
                        builder.AppendLine("ArtistName,");
                        builder.AppendLine($"CAST(substr(offsets({Name}),5,2) AS INTEGER) AS Rank");
                        builder.AppendLine($"FROM {Name}");
                        builder.AppendLine($"WHERE AlbumName MATCH \"{term}*\" ORDER BY RANK ASC)");
                    }
                    builder.AppendLine("GROUP BY Id, AlbumName, ArtistName, Rank");
                    builder.AppendLine("ORDER BY Rank ASC, AlbumName ASC");

                    if (maxCount.HasValue)
                    {
                        builder.AppendLine($"LIMIT @MaxCount");
                        command.Parameters.AddWithValue("@MaxCount", maxCount.Value);
                    }

                    command.CommandText = builder.ToString();
                    command.Parameters.AddWithValue("@term", term);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            albums.Add(ExtractAlbumQueryItem(reader));
                        }
                    }
                }
            }

            return albums;
        }

        public IList<ArtistQueryItem> FindMatchingArtists(string term, int? maxCount)
        {
            List<ArtistQueryItem> artist = new List<ArtistQueryItem>();
            if (!string.IsNullOrEmpty(term))
            {
                using (var command = connection.CreateCommand())
                {
                    var builder = new StringBuilder();
                    builder.AppendLine("SELECT Id, ArtistName, COUNT(ArtistName) AS TrackCount, Rank FROM");
                    {
                        builder.AppendLine("(SELECT CAST(ArtistId AS INTEGER) AS Id,");
                        builder.AppendLine("ArtistName,");
                        builder.AppendLine($"CAST(substr(offsets({Name}),5,2) AS INTEGER) AS Rank");
                        builder.AppendLine($"FROM {Name}");
                        builder.AppendLine($"WHERE ArtistName MATCH \"{term}*\" ORDER BY RANK ASC)");
                    }
                    builder.AppendLine("GROUP BY Id, ArtistName, Rank");
                    builder.AppendLine("ORDER BY Rank ASC, ArtistName ASC");

                    if (maxCount.HasValue)
                    {
                        builder.AppendLine($"LIMIT @MaxCount");
                        command.Parameters.AddWithValue("@MaxCount", maxCount.Value);
                    }

                    command.CommandText = builder.ToString();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            artist.Add(ExtractArtistQueryItem(reader));
                        }
                    }
                }
            }

            return artist;
        }

        public IList<GenreQueryItem> FindMatchingGenres(string term, int? maxCount)
        {
            List<GenreQueryItem> genres = new List<GenreQueryItem>();
            if (!string.IsNullOrEmpty(term))
            {
                using (var command = connection.CreateCommand())
                {
                    var builder = new StringBuilder();
                    builder.AppendLine("SELECT Id, GenreName, COUNT(GenreName) AS TrackCount, Rank FROM");
                    {
                        builder.AppendLine("(SELECT CAST(GenreId AS INTEGER) AS Id,");
                        builder.AppendLine("GenreName,");
                        builder.AppendLine($"CAST(substr(offsets({Name}),5,2) AS INTEGER) AS Rank");
                        builder.AppendLine($"FROM {Name}");
                        builder.AppendLine($"WHERE GenreName MATCH \"{term}*\" ORDER BY RANK ASC)");
                    }
                    builder.AppendLine("GROUP BY Id, GenreName, Rank");
                    builder.AppendLine("ORDER BY Rank ASC, GenreName ASC");

                    if (maxCount.HasValue)
                    {
                        builder.AppendLine($"LIMIT @MaxCount");
                        command.Parameters.AddWithValue("@MaxCount", maxCount.Value);
                    }

                    command.CommandText = builder.ToString();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            genres.Add(ExtractGenreQueryItem(reader));
                        }
                    }
                }
            }

            return genres;
        }

        public IList<TrackQueryItem> FindMatchingTracks(string term, int? maxCount)
        {
            List<TrackQueryItem> tracks = new List<TrackQueryItem>();

            if (!string.IsNullOrEmpty(term))
            {
                using (var command = connection.CreateCommand())
                {
                    var builder = new StringBuilder();
                    builder.AppendLine($"SELECT docid AS Id, TrackName, TrackArtist, CAST(substr(offsets({Name}), 5, 2) AS INTEGER) AS Rank, CAST(AlbumId AS INTEGER) AS AlbumId");
                    builder.AppendLine($"FROM {Name}");
                    builder.AppendLine($"WHERE TrackName MATCH \"{term}*\"");
                    builder.AppendLine("ORDER BY RANK ASC, TrackName ASC");

                    if (maxCount.HasValue)
                    {
                        builder.AppendLine("LIMIT @MaxCount");
                        command.Parameters.AddWithValue("@MaxCount", maxCount.Value);
                    }

                    command.CommandText = builder.ToString();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tracks.Add(ExtractTrackQueryItem(reader));
                        }
                    }
                }
            }

            return tracks;
        }

        public IList<TrackQueryItem> FindMatchingTracksWithArtists(string term, int? maxCount)
        {
            List<TrackQueryItem> tracks = new List<TrackQueryItem>();

            if (!string.IsNullOrEmpty(term))
            {
                using (var command = connection.CreateCommand())
                {
                    var builder = new StringBuilder();
                    builder.AppendLine($"SELECT docid AS Id, TrackName, TrackArtist, CAST(substr(offsets({Name}), 5, 2) AS INTEGER) AS Rank, CAST(AlbumId AS INTEGER) AS AlbumId");
                    builder.AppendLine($"FROM {Name}");
                    builder.AppendLine($"WHERE TrackArtist MATCH \"{term}*\"");
                    builder.AppendLine("ORDER BY RANK ASC, TrackName ASC");

                    if (maxCount.HasValue)
                    {
                        builder.AppendLine("LIMIT @MaxCount");
                        command.Parameters.AddWithValue("@MaxCount", maxCount.Value);
                    }

                    command.CommandText = builder.ToString();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tracks.Add(ExtractTrackQueryItem(reader));
                        }
                    }
                }
            }

            return tracks;
        }

        private IndexedTrack ExtractIndexedTrack(SqliteDataReader reader)
        {
            int index = 0;
            return new IndexedTrack()
            {
                Id = reader.GetInt64(index++),
                ArtistName = reader.IsDBNull(index++) ? null : reader.GetString(index - 1),
                AlbumName = reader.IsDBNull(index++) ? null : reader.GetString(index - 1),
                TrackName = reader.IsDBNull(index++) ? null : reader.GetString(index - 1),
                TrackArtist = reader.IsDBNull(index++) ? null : reader.GetString(index - 1),
                GenreName = reader.IsDBNull(index++) ? null : reader.GetString(index - 1),
                AlbumId = reader.IsDBNull(index++) ? null : reader.GetString(index - 1),
                ArtistId = reader.IsDBNull(index++) ? null : reader.GetString(index - 1),
                GenreId = reader.IsDBNull(index++) ? null : reader.GetString(index - 1)
            };
        }

        private AlbumQueryItem ExtractAlbumQueryItem(SqliteDataReader reader)
        {
            return new AlbumQueryItem()
            {
                Id = reader.GetInt64(0),
                AlbumName = reader.GetString(1),
                ArtistName = reader.GetString(2),
                Rank = reader.GetInt32(3)
            };
        }

        private ArtistQueryItem ExtractArtistQueryItem(SqliteDataReader reader)
        {
            return new ArtistQueryItem()
            {
                Id = reader.GetInt64(0),
                ArtistName = reader.GetString(1),
                TrackCount = reader.GetInt32(2),
                Rank = reader.GetInt32(3)
            };
        }

        private GenreQueryItem ExtractGenreQueryItem(SqliteDataReader reader)
        {
            return new GenreQueryItem()
            {
                Id = reader.GetInt64(0),
                GenreName = reader.GetString(1),
                TrackCount = reader.GetInt32(2),
                Rank = reader.GetInt32(3)
            };
        }

        private TrackQueryItem ExtractTrackQueryItem(SqliteDataReader reader)
        {
            return new TrackQueryItem()
            {
                Id = reader.GetInt64(0),
                TrackName = reader.GetString(1),
                TrackArtist = reader.GetString(2),
                Rank = reader.GetInt32(3),
                AlbumId = reader.GetInt64(4)
            };
        }

        private static void AddParameters(IndexedTrack track, SqliteCommand command)
        {
            command.Parameters.AddWithValue("@Id", track.Id.Value);
            command.Parameters.AddWithNullableValue("@ArtistName", track.ArtistName);
            command.Parameters.AddWithNullableValue("@AlbumName", track.AlbumName);
            command.Parameters.AddWithNullableValue("@TrackName", track.TrackName);
            command.Parameters.AddWithNullableValue("@TrackArtist", track.TrackArtist);
            command.Parameters.AddWithNullableValue("@GenreName", track.GenreName);
            command.Parameters.AddWithNullableValue("@AlbumId", track.AlbumId);
            command.Parameters.AddWithNullableValue("@ArtistId", track.ArtistId);
            command.Parameters.AddWithNullableValue("@GenreId", track.GenreId);
        }
    }
}

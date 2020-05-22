using Microsoft.Data.Sqlite;
using OnePlayer.Data.Access;
using System;
using System.Collections.Generic;

namespace OnePlayer.Data.Sqlite
{
    public sealed class ArtistTable : IArtistAccessor, ITable
    {
        private readonly SqliteConnection connection;

        public ArtistTable(SqliteConnection connection)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public string Name { get; } = "Artist";

        public void HandleUpgrade(Version version)
        {
            if (version == Version.Initial)
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"CREATE TABLE {Name} (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name VARCHAR)";
                    command.ExecuteNonQuery();
                }
            }
            
            if (version == Version.AddIndexes)
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"CREATE INDEX Idx_{Name}_Name ON {Name}(Name)";
                    command.ExecuteNonQuery();
                }
            }
        }

        public Artist Add(Artist artist)
        {
            if (artist.Id.HasValue)
            {
                throw new ArgumentException(nameof(artist.Id));
            }

            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"INSERT INTO {Name} (Name) VALUES(@Name); select last_insert_rowid()";
                command.Parameters.AddWithNullableValue("@Name", artist.Name);

                artist.Id = (long)command.ExecuteScalar();
            }

            return artist;
        }

        public Artist Find(string artistName)
        {
            if (string.IsNullOrEmpty(artistName))
            {
                throw new ArgumentNullException(nameof(artistName));
            }

            Artist artist = null;

            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT * FROM {Name} WHERE Name = @Name LIMIT 1";
                command.Parameters.AddWithNullableValue("@Name", artistName);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        artist = ExtractArtist(reader);
                    }
                }
            }

            return artist;
        }

        public Artist Get(long id)
        {
            Artist artist = null;

            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT * FROM {Name} WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", id);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        artist = ExtractArtist(reader);
                    }
                }
            }

            return artist;
        }

        public IList<Artist> GetAll()
        {
            List<Artist> artists = new List<Artist>();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT * FROM {Name} ORDER BY Name ASC";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        artists.Add(ExtractArtist(reader));
                    }
                }
            }

            return artists;
        }

        public Artist Update(Artist artist)
        {
            if (!artist.Id.HasValue)
            {
                throw new ArgumentNullException(nameof(artist.Id));
            }

            if (string.IsNullOrEmpty(artist.Name))
            {
                throw new ArgumentNullException(nameof(artist.Name));
            }

            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"UPDATE {Name} SET Name = @Name WHERE Id = @Id";
                command.Parameters.AddWithNullableValue("@Name", artist.Name);
                command.Parameters.AddWithValue("@Id", artist.Id.Value);

                command.ExecuteNonQuery();
            }

            return artist;
        }

        private Artist ExtractArtist(SqliteDataReader reader)
        {
            return new Artist()
            {
                Id = reader.GetInt64(0),
                Name = !reader.IsDBNull(1) ? reader.GetString(1) : null
            };
        }
    }
}

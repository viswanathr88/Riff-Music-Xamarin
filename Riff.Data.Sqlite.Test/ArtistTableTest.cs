using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using Xunit;

namespace Riff.Data.Sqlite.Test
{
    public sealed class ArtistTableTest : IDisposable
    {
        private readonly string dbPath = ":memory:";
        private readonly SqliteConnection connection;
        private readonly ArtistTable artistTable;

        public ArtistTableTest()
        {
            connection = new SqliteConnection($"Data Source = {dbPath}");
            connection.Open();

            artistTable = new ArtistTable(connection, new DataExtractor());
            artistTable.HandleUpgrade(Version.Initial);
        }

        [Fact]
        public void Constructor_NullConnection_ThrowException()
        {
            Assert.Throws<ArgumentNullException>(() => new ArtistTable(null, new DataExtractor()));
        }

        [Fact]
        public void Add_Success_ValidateId()
        {
            var artist = artistTable.Add(new Data.Artist() { Name = "TestArtist" });
            Assert.Equal(1, artist.Id);
        }

        [Fact]
        public void Add_Success_ValidateName()
        {
            var artistName = "TestArtist";
            artistTable.Add(new Data.Artist() { Name = artistName });

            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT * FROM {artistTable.Name}";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Assert.Equal(1, reader.GetInt64(0));
                        Assert.Equal(artistName, reader.GetString(1));
                    }
                }
            }
        }

        [Fact]
        public void Add_NullArtistName_ReturnDbNull()
        {
            string artistName = null;
            artistTable.Add(new Artist() { Name = artistName });
            
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT * FROM {artistTable.Name}";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Assert.Equal(1, reader.GetInt64(0));
                        Assert.True(reader.IsDBNull(1));
                    }
                }
            }
        }

        [Fact]
        public void Add_MultipleArtists_ValidateNames()
        {
            var artistIds = new List<long>() { 1, 2, 3 };
            var artists = new List<string>() { "TestArtist1", "TestArtist2", "TestArtist3" };
            foreach (var artist in artists)
            {
                artistTable.Add(new Artist() { Name = artist });
            }

            var actualArtistIds = new List<long>();
            var actualArtists = new List<string>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT * FROM {artistTable.Name}";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        actualArtistIds.Add(reader.GetInt64(0));
                        actualArtists.Add(reader.GetString(1));
                    }
                }
            }

            Assert.Equal(artistIds, actualArtistIds);
            Assert.Equal(artists, actualArtists);
        }

        [Fact]
        public void Add_NonNullId_Throw()
        {
            Assert.ThrowsAny<ArgumentException>(() => artistTable.Add(new Data.Artist() { Id = 5, Name = "TestArtist" }));
        }

        [Fact]
        public void Find_ArtistNotFound_ReturnNull()
        {
            Assert.Null(artistTable.Find("TestArtist"));
        }

        [Fact]
        public void Find_ArtistFound_ValidateNameAndId()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var foundArtist = artistTable.Find("TestArtist");
            CompareAndAssert(artist, foundArtist);
        }

        [Fact]
        public void Find_ArtistNameNull_EnsureIdMatches()
        {
            var artist = new Artist();
            artistTable.Add(artist);

            var actualArtist = artistTable.Find(null);
            CompareAndAssert(actualArtist, artist);
        }

        [Fact]
        public void Find_CaseSensitiveArtistName_ReturnNull()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            Assert.Null(artistTable.Find("testArtist"));
        }

        [Fact]
        public void Get_ArtistNotFound_ReturnNull()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            Assert.Null(artistTable.Get(artist.Id.Value + 1));
        }

        [Fact]
        public void Get_ArtistFound_Validate()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var actualArtist = artistTable.Get(artist.Id.Value);
            CompareAndAssert(artist, actualArtist);
        }

        [Fact]
        public void Get_ExistingArtistWithEmptyName_ValidateFields()
        {
            var artist = artistTable.Add(new Artist());

            CompareAndAssert(artist, artistTable.Get(artist.Id.Value));
        }

        [Fact]
        public void GetAll_EmptyTable_ReturnEmptyList()
        {
            Assert.Equal(0, artistTable.GetAll().Count);
        }

        [Fact]
        public void GetAll_FiveAdds_ReturnListOfLength5()
        {
            artistTable.Add(new Artist() { Name = "TestArtist1" });
            artistTable.Add(new Artist() { Name = "TestArtist2" });
            artistTable.Add(new Artist() { Name = "TestArtist3" });
            artistTable.Add(new Artist() { Name = "TestArtist4" });
            artistTable.Add(new Artist() { Name = "TestArtist5" });

            Assert.Equal(5, artistTable.GetAll().Count);
        }

        [Fact]
        public void GetAll_FiveAdds_ReturnAllFiveWithCorrectName()
        {
            var ids = new long[] { 1, 2, 3, 4, 5 };
            var names = new string[] { "TestArtist1", "TestArtist2", "TestArtist3", "TestArtist4", "TestArtist5" };

            foreach (var name in names)
            {
                artistTable.Add(new Artist() { Name = name });
            }

            var result = artistTable.GetAll();
            foreach (var artist in result)
            {
                Assert.Contains(artist.Id.Value, ids);
                Assert.Contains(artist.Name, names);
            }

            Assert.Equal(ids.Length, result.Count);
        }

        [Fact]
        public void Update_NullId_Throw()
        {
            Assert.Throws<ArgumentNullException>(() => artistTable.Update(new Artist() { Name = "TestArtist" }));
        }

        [Fact]
        public void Update_NullName_Throw()
        {
            Assert.Throws<ArgumentNullException>(() => artistTable.Update(new Artist()));
            Assert.Throws<ArgumentNullException>(() => artistTable.Update(new Artist() { Name = null }));
            Assert.Throws<ArgumentNullException>(() => artistTable.Update(new Artist() { Name = string.Empty }));
        }

        [Fact]
        public void Update_ValidArtist_ValidateGet()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            artist.Name = "UpdatedTestArtist";
            var updatedArtist = artistTable.Update(artist);
            CompareAndAssert(artist, updatedArtist);

            var getArtist = artistTable.Get(artist.Id.Value);
            CompareAndAssert(updatedArtist, getArtist);
        }

        public void Dispose()
        {
            connection?.Dispose();
        }

        internal static void CompareAndAssert(Artist first, Artist second)
        {
            if (first == second) return;
            Assert.Equal(first.Id, second.Id);
            Assert.Equal(first.Name, second.Name);
        }
    }
}

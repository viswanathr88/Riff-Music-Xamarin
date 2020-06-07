using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using Xunit;

namespace Riff.Data.Sqlite.Test
{
    public sealed class GenreTableTest : IDisposable
    {
        private readonly string dbPath = ":memory:";
        private readonly SqliteConnection connection;
        private readonly GenreTable genreTable;

        public GenreTableTest()
        {
            connection = new SqliteConnection($"Data Source = {dbPath}");
            connection.Open();

            genreTable = new GenreTable(connection, new DataExtractor());
            genreTable.HandleUpgrade(Version.Initial);
        }
        [Fact]
        public void Constructor_NullConnection_ThrowException()
        {
            Assert.Throws<ArgumentNullException>(() => new GenreTable(null, new DataExtractor()));
        }

        [Fact]
        public void Add_Success_ValidateId()
        {
            var genre = genreTable.Add(new Data.Genre() { Name = "TestGenre" });
            Assert.Equal(1, genre.Id);
        }

        [Fact]
        public void Add_Success_ValidateName()
        {
            var genreName = "TestGenre";
            genreTable.Add(new Data.Genre() { Name = genreName });

            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT * FROM {genreTable.Name}";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Assert.Equal(1, reader.GetInt64(0));
                        Assert.Equal(genreName, reader.GetString(1));
                    }
                }
            }
        }

        [Fact]
        public void Add_MultipleGenres_ValidateNames()
        {
            var genreIds = new List<long>() { 1, 2, 3 };
            var genres = new List<string>() { "TestGenre1", "TestGenre2", "TestGenre3" };
            foreach (var genre in genres)
            {
                genreTable.Add(new Genre() { Name = genre });
            }

            var actualGenreIds = new List<long>();
            var actualGenres = new List<string>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT * FROM {genreTable.Name}";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        actualGenreIds.Add(reader.GetInt64(0));
                        actualGenres.Add(reader.GetString(1));
                    }
                }
            }

            Assert.Equal(genreIds, actualGenreIds);
            Assert.Equal(genres, actualGenres);
        }

        [Fact]
        public void Add_NonNullId_Throw()
        {
            Assert.ThrowsAny<ArgumentException>(() => genreTable.Add(new Data.Genre() { Id = 5, Name = "TestGenre" }));
        }

        [Fact]
        public void Find_NullParameter_Throw()
        {
            Assert.Throws<ArgumentNullException>(() => genreTable.Find(string.Empty));
            Assert.Throws<ArgumentNullException>(() => genreTable.Find(null));
        }

        [Fact]
        public void Find_GenreNotFound_ReturnNull()
        {
            Assert.Null(genreTable.Find("TestGenre"));
        }

        [Fact]
        public void Find_GenreFound_ValidateNameAndId()
        {
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var foundGenre = genreTable.Find("TestGenre");
            CompareAndAssert(genre, foundGenre);
        }

        [Fact]
        public void Find_CaseSensitiveGenreName_ReturnNull()
        {
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            Assert.Null(genreTable.Find("testGenre"));
        }

        [Fact]
        public void Get_GenreNotFound_ReturnNull()
        {
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            Assert.Null(genreTable.Get(genre.Id.Value + 1));
        }

        [Fact]
        public void Get_GenreFound_Validate()
        {
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var actualGenre = genreTable.Get(genre.Id.Value);
            CompareAndAssert(genre, actualGenre);
        }

        [Fact]
        public void GetAll_EmptyTable_ReturnEmptyList()
        {
            Assert.Equal(0, genreTable.GetAll().Count);
        }

        [Fact]
        public void GetAll_FiveAdds_ReturnListOfLength5()
        {
            genreTable.Add(new Genre() { Name = "TestGenre1" });
            genreTable.Add(new Genre() { Name = "TestGenre2" });
            genreTable.Add(new Genre() { Name = "TestGenre3" });
            genreTable.Add(new Genre() { Name = "TestGenre4" });
            genreTable.Add(new Genre() { Name = "TestGenre5" });

            Assert.Equal(5, genreTable.GetAll().Count);
        }

        [Fact]
        public void GetAll_FiveAdds_ReturnAllFiveWithCorrectName()
        {
            var ids = new long[] { 1, 2, 3, 4, 5 };
            var names = new string[] { "TestGenre1", "TestGenre2", "TestGenre3", "TestGenre4", "TestGenre5" };

            foreach (var name in names)
            {
                genreTable.Add(new Genre() { Name = name });
            }

            var result = genreTable.GetAll();
            foreach (var genre in result)
            {
                Assert.Contains(genre.Id.Value, ids);
                Assert.Contains(genre.Name, names);
            }

            Assert.Equal(ids.Length, result.Count);
        }

        [Fact]
        public void Update_NullId_Throw()
        {
            Assert.Throws<ArgumentNullException>(() => genreTable.Update(new Genre() { Name = "TestGenre" }));
        }

        [Fact]
        public void Update_NullName_Throw()
        {
            Assert.Throws<ArgumentNullException>(() => genreTable.Update(new Genre()));
            Assert.Throws<ArgumentNullException>(() => genreTable.Update(new Genre() { Name = null }));
            Assert.Throws<ArgumentNullException>(() => genreTable.Update(new Genre() { Name = string.Empty }));
        }

        [Fact]
        public void Update_ValidGenre_ValidateGet()
        {
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            genre.Name = "UpdatedTestGenre";
            var updatedGenre = genreTable.Update(genre);
            CompareAndAssert(genre, updatedGenre);

            var getGenre = genreTable.Get(genre.Id.Value);
            CompareAndAssert(updatedGenre, getGenre);
        }

        public void Dispose()
        {
            connection?.Dispose();
        }

        internal static void CompareAndAssert(Genre first, Genre second)
        {
            if (first == second) return;
            Assert.Equal(first.Id, second.Id);
            Assert.Equal(first.Name, second.Name);
        }
    }
}

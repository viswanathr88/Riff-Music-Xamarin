using Microsoft.Data.Sqlite;
using Riff.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Version = Riff.Data.Sqlite.Version;

namespace Riff.Data.Test
{
    public sealed class AlbumTableTest : IDisposable
    {
        private readonly string dbPath = ":memory:";
        private readonly SqliteConnection connection;
        private readonly AlbumTable albumTable;
        private readonly ArtistTable artistTable;
        private readonly GenreTable genreTable;

        public AlbumTableTest()
        {
            connection = new SqliteConnection($"Data Source = {dbPath};foreign keys=true;");
            connection.Open();

            var extractor = new DataExtractor();

            artistTable = new ArtistTable(connection, extractor);
            artistTable.HandleUpgrade(Version.Initial);

            genreTable = new GenreTable(connection, extractor);
            genreTable.HandleUpgrade(Version.Initial);

            albumTable = new AlbumTable(connection, extractor);
            albumTable.HandleUpgrade(Version.Initial);
        }

        [Fact]
        public void Add_NoArtistId_Throw()
        {
            var album = new Album()
            {
                Name = "TestAlbum",
                ReleaseYear = 2010,
                Artist = new Artist()
                {
                    Name = "Test Artist"
                },
                Genre = new Genre()
                {
                    Id = 1,
                    Name = "TestGenre"
                }
            };

            Assert.Throws<ArgumentNullException>(() => albumTable.Add(album));
        }

        [Fact]
        public void Add_NoGenreId_Throw()
        {
            var album = new Album()
            {
                Name = "TestAlbum",
                ReleaseYear = 2010,
                Artist = new Artist()
                {
                    Id = 1,
                    Name = "Test Artist"
                },
                Genre = new Genre()
                {
                    Name = "TestGenre"
                }
            };

            Assert.Throws<ArgumentNullException>(() => albumTable.Add(album));
        }

        [Fact]
        public void Add_IdAlreadyExists_Throw()
        {
            var album = new Album()
            {
                Id = 1,
                Name = "TestAlbum",
                ReleaseYear = 2010,
                Artist = new Artist()
                {
                    Id = 1,
                    Name = "Test Artist"
                },
                Genre = new Genre()
                {
                    Id = 1,
                    Name = "TestGenre"
                }
            };

            Assert.Throws<ArgumentException>(() => albumTable.Add(album));
        }

        [Fact]
        public void Add_NonExistentArtistId_Throw()
        {
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            Assert.ThrowsAny<Exception>(() => albumTable.Add(new Album()
            {
                Name = "TestAlbum",
                ReleaseYear = 2000,
                Artist = new Artist()
                {
                    Id = 1,
                    Name = "TestArtist"
                },
                Genre = genre
            }));
        }

        [Fact]
        public void Add_NonExistentGenreId_Throw()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            Assert.ThrowsAny<Exception>(() => albumTable.Add(new Album()
            {
                Name = "TestAlbum",
                ReleaseYear = 2000,
                Artist = artist,
                Genre = new Genre()
                {
                    Id = 1,
                    Name = "TestGenre"
                }
            }));
        }

        [Fact]
        public void Add_ValidGenreAndArtist_ValidateAlbumId()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist " });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });

            var album = albumTable.Add(new Album()
            {
                Name = "TestAlbum",
                ReleaseYear = 2000,
                Artist = artist,
                Genre = genre
            });

            Assert.Equal(1, album.Id);
        }

        [Fact]
        public void Add_Album_ValidateReturnFields()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist " });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });

            var album = albumTable.Add(new Album()
            {
                Name = "TestAlbum",
                ReleaseYear = 2000,
                Artist = artist,
                Genre = genre
            });

            Assert.Equal(1, album.Id);
            Assert.Equal("TestAlbum", album.Name);
            Assert.Equal(2000, album.ReleaseYear);
            Assert.Equal(artist.Id, album.Artist.Id);
            Assert.Equal(genre.Id, album.Genre.Id);
        }

        [Fact]
        public void Add_MultipleAlbums_ValidateAlbumIdIncrement()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist " });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });

            var album1 = albumTable.Add(new Album()
            {
                Name = "TestAlbum1",
                ReleaseYear = 2000,
                Artist = artist,
                Genre = genre
            });

            var album2 = albumTable.Add(new Album()
            {
                Name = "TestAlbum2",
                ReleaseYear = 2001,
                Artist = artist,
                Genre = genre
            });

            var album3 = albumTable.Add(new Album()
            {
                Name = "TestAlbum3",
                ReleaseYear = 2002,
                Artist = artist,
                Genre = genre
            });

            Assert.Equal(1, album1.Id);
            Assert.Equal(2, album2.Id);
            Assert.Equal(3, album3.Id);
        }

        [Fact]
        public void Get_NonExistentAlbum_ReturnNull()
        {
            Assert.Null(albumTable.Get(1));
        }

        [Fact]
        public void FindByArtist_EmptyAlbumName_Validate()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var album = albumTable.Add(new Album()
            {
                Artist = new Artist() { Id = artist.Id },
                Genre = new Genre() { Id = genre.Id },
                ReleaseYear = 2000
            });

            var actualAlbum = albumTable.FindByArtist(artist.Id.Value, string.Empty);
            CompareAndAssert(actualAlbum, album);
        }

        [Fact]
        public void FindByArtist_TwoDifferentArtistsWithEmptyAlbums_Validate()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var artist2 = artistTable.Add(new Artist() { Name = "TestArtist2" });

            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var album = albumTable.Add(new Album()
            {
                Artist = new Artist() { Id = artist.Id },
                Genre = new Genre() { Id = genre.Id },
                ReleaseYear = 2000
            });

            var album2 = albumTable.Add(new Album()
            {
                Artist = new Artist() { Id = artist2.Id },
                Genre = new Genre() { Id = genre.Id },
                ReleaseYear = 1999
            });

            var actualAlbum = albumTable.FindByArtist(artist.Id.Value, string.Empty);
            CompareAndAssert(actualAlbum, album);

            actualAlbum = albumTable.FindByArtist(artist2.Id.Value, null);
            CompareAndAssert(actualAlbum, album2);
        }

        [Fact]
        public void FindByArtist_NonExistentArtist_ReturnNull()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var album = albumTable.Add(new Album()
            {
                Name = "TestAlbum",
                Artist = artist,
                Genre = genre,
                ReleaseYear = 2000
            });

            Assert.Null(albumTable.FindByArtist(23, "TestAlbum"));
        }

        [Fact]
        public void FindByArtist_ExistingArtistButDifferentAlbum_ReturnNull()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            albumTable.Add(new Album()
            {
                Name = "TestAlbum",
                Artist = artist,
                Genre = genre,
                ReleaseYear = 2000
            });

            Assert.Null(albumTable.FindByArtist(artist.Id.Value, "TestAlbumNIL"));
        }

        [Fact]
        public void FindByArtist_ExistingArtist_ValidateFields()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var album = albumTable.Add(new Album()
            {
                Name = "TestAlbum",
                Artist = new Artist() { Id = artist.Id },
                Genre = new Genre() { Id = genre.Id },
                ReleaseYear = 2000
            });

            CompareAndAssert(album, albumTable.FindByArtist(artist.Id.Value, album.Name));
        }

        [Fact]
        public void Get_ExistingAlbum_ValidateFields()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var album = albumTable.Add(new Album()
            {
                Name = "TestAlbum",
                Artist = new Artist() { Id = artist.Id },
                Genre = new Genre() { Id = genre.Id },
                ReleaseYear = 2000
            });

            var actualAlbum = albumTable.Get(album.Id.Value);
            CompareAndAssert(album, actualAlbum);
        }

        [Fact]
        public void Get_ExistingAlbumWithEmptyName_ValidateFields()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var album = albumTable.Add(new Album()
            {
                Artist = new Artist() { Id = artist.Id },
                Genre = new Genre() { Id = genre.Id },
                ReleaseYear = 2000
            });

            var actualAlbum = albumTable.Get(album.Id.Value);
            CompareAndAssert(album, actualAlbum);
        }

        [Fact]
        public void Get_ManyExistingAlbums_ValidateFields()
        {
            for (int i = 0; i < 5; i++)
            {
                artistTable.Add(new Artist() { Name = "TestArtist" + (i + 1) });
            }

            for (int i = 0; i < 5; i++)
            {
                genreTable.Add(new Genre() { Name = "TestGenre" + (i + 1) });
            }

            for (int i = 0; i < 5; i++)
            {
                albumTable.Add(new Album() { Name = "TestAlbum" + (i + 1), Artist = artistTable.Get(i + 1), Genre = genreTable.Get(i + 1), ReleaseYear = 1990 + i });
            }

            var album = albumTable.Add(new Album() { Name = "TestAlbum10", Artist = new Artist() { Id = 2 }, Genre = new Genre() { Id = 3 }, ReleaseYear = 1990 });

            CompareAndAssert(album, albumTable.Get(album.Id.Value));
        }

        [Fact]
        public void Get_AllAlbums_ValidateCountAndFields()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });

            int albumCount = 10;
            IList<Album> expectedAlbums = new List<Album>();
            for (int i = 0; i< albumCount; i++)
            {
                expectedAlbums.Add(albumTable.Add(new Album()
                {
                    Name = "TestAlbum" + (i+1),
                    Artist = new Artist() { Id = artist.Id },
                    Genre = new Genre() { Id = genre.Id },
                    ReleaseYear = (i+1)
                }));
            }

            var albums = albumTable.Get();
            Assert.Equal(albums, expectedAlbums, new AlbumComparer());
        }

        [Fact]
        public void Get_AllAlbumsEmptyTable_ReturnEmpty()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });

            var albums = albumTable.Get();
            Assert.Empty(albums);
        }

        [Fact]
        public void Get_AllAlbumsSortedByReleaseYear_ValidateOrder()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });

            int albumCount = 10;
            IList<Album> expectedAlbums = new List<Album>();
            for (int i = 0; i < albumCount; i++)
            {
                expectedAlbums.Add(albumTable.Add(new Album()
                {
                    Name = "TestAlbum" + (i + 1),
                    Artist = new Artist() { Id = artist.Id },
                    Genre = new Genre() { Id = genre.Id },
                    ReleaseYear = (i + 1)
                }));
            }

            expectedAlbums = expectedAlbums.OrderByDescending(album => album.ReleaseYear).ToList();

            var albums = albumTable.Get(AlbumSortType.ReleaseYear, SortOrder.Descending);
            Assert.Equal(albums, expectedAlbums, new AlbumComparer());
        }

        [Fact]
        public void Get_AllAlbumsSortedByName_ValidateOrder()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });

            int albumCount = 10;
            IList<Album> expectedAlbums = new List<Album>();
            for (int i = 0; i < albumCount; i++)
            {
                expectedAlbums.Add(albumTable.Add(new Album()
                {
                    Name = "TestAlbum" + (i + 1),
                    Artist = new Artist() { Id = artist.Id },
                    Genre = new Genre() { Id = genre.Id },
                    ReleaseYear = (i + 1)
                }));
            }

            expectedAlbums = expectedAlbums.OrderByDescending(album => album.Name).ToList();

            var albums = albumTable.Get(AlbumSortType.Name, SortOrder.Descending);
            Assert.Equal(albums, expectedAlbums, new AlbumComparer());

            expectedAlbums = expectedAlbums.OrderBy(album => album.Name).ToList();
            albums = albumTable.Get(AlbumSortType.Name, SortOrder.Ascending);
            Assert.Equal(albums, expectedAlbums, new AlbumComparer());
        }

        [Fact]
        public void Get_AllAlbumsSortedByReleaseYear_ValidateLimitAndOffset()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });

            int albumCount = 10;
            IList<Album> expectedAlbums = new List<Album>();
            for (int i = 0; i < albumCount; i++)
            {
                expectedAlbums.Add(albumTable.Add(new Album()
                {
                    Name = "TestAlbum" + (i + 1),
                    Artist = new Artist() { Id = artist.Id },
                    Genre = new Genre() { Id = genre.Id },
                    ReleaseYear = (i + 1)
                }));
            }

            var options = new AlbumAccessOptions()
            {
                SortType = AlbumSortType.ReleaseYear,
                SortOrder = SortOrder.Descending,
                StartPosition = 5,
                Count = 4
            };

            expectedAlbums = expectedAlbums.OrderByDescending(album => album.ReleaseYear).Skip(5).Take(4).ToList();
            var albums = albumTable.Get(options);
            Assert.Equal(expectedAlbums, albums, new AlbumComparer());
        }

        [Fact]
        public void Get_AllAlbumsOffsetOutOfBounds_ReturnEmpty()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });

            int albumCount = 10;
            IList<Album> expectedAlbums = new List<Album>();
            for (int i = 0; i < albumCount; i++)
            {
                expectedAlbums.Add(albumTable.Add(new Album()
                {
                    Name = "TestAlbum" + (i + 1),
                    Artist = new Artist() { Id = artist.Id },
                    Genre = new Genre() { Id = genre.Id },
                    ReleaseYear = (i + 1)
                }));
            }

            var options = new AlbumAccessOptions()
            {
                SortType = AlbumSortType.ReleaseYear,
                SortOrder = SortOrder.Descending,
                StartPosition = 11,
                Count = 4
            };

            var albums = albumTable.Get(options);
            Assert.Empty(albums);
        }

        [Fact]
        public void Get_AllAlbumsCountGreaterThanTableSize_ReturnTableSize()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });

            int albumCount = 10;
            IList<Album> expectedAlbums = new List<Album>();
            for (int i = 0; i < albumCount; i++)
            {
                expectedAlbums.Add(albumTable.Add(new Album()
                {
                    Name = "TestAlbum" + (i + 1),
                    Artist = new Artist() { Id = artist.Id },
                    Genre = new Genre() { Id = genre.Id },
                    ReleaseYear = (i + 1)
                }));
            }

            var options = new AlbumAccessOptions()
            {
                SortType = AlbumSortType.ReleaseYear,
                SortOrder = SortOrder.Descending,
                StartPosition = 0,
                Count = 25
            };

            var albums = albumTable.Get(options);
            Assert.Equal(10, albums.Count);
        }

        [Fact]
        public void GetWithOptions_NullOptions_Throw()
        {
            Assert.Throws<ArgumentNullException>(() => albumTable.Get(null));
        }

        [Fact]
        public void GetWithOptions_AlbumFilterWithNonExistentAlbum_ReturnEmpty()
        {
            var options = new AlbumAccessOptions()
            {
                AlbumFilter = 1
            };

            Assert.Empty(albumTable.Get(options));
        }

        [Fact]
        public void GetWithOptions_AlbumFilterForExistingAlbum_ReturnOne()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            
            int albumCount = 10;
            IList<Album> expectedAlbums = new List<Album>();
            for (int i = 0; i < albumCount; i++)
            {
                expectedAlbums.Add(albumTable.Add(new Album()
                {
                    Name = "TestAlbum" + (i + 1),
                    Artist = new Artist() { Id = artist.Id },
                    Genre = new Genre() { Id = genre.Id },
                    ReleaseYear = (i + 1)
                }));
            }

            var options = new AlbumAccessOptions()
            {
                AlbumFilter = 3
            };

            var albums = albumTable.Get(options);
            Assert.Equal(1, albums.Count);
            Assert.Equal(expectedAlbums[2], albums[0], new AlbumComparer());
        }

        [Fact]
        public void GetWithOptions_AlbumNameFilterForEmptyTable_ReturnEmpty()
        {
            var options = new AlbumAccessOptions()
            {
                AlbumNameFilter = "TestAlbum"
            };

            Assert.Empty(albumTable.Get(options));
        }

        [Fact]
        public void GetWithOptions_AlbumNameFilterForExistingAlbum_ReturnOne()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });

            int albumCount = 10;
            IList<Album> expectedAlbums = new List<Album>();
            for (int i = 0; i < albumCount; i++)
            {
                expectedAlbums.Add(albumTable.Add(new Album()
                {
                    Name = "TestAlbum" + (i + 1),
                    Artist = new Artist() { Id = artist.Id },
                    Genre = new Genre() { Id = genre.Id },
                    ReleaseYear = (i + 1)
                }));
            }

            var options = new AlbumAccessOptions()
            {
                AlbumNameFilter = "TestAlbum4"
            };

            var albums = albumTable.Get(options);
            Assert.Equal(1, albums.Count);
            Assert.Equal(expectedAlbums[3], albums[0], new AlbumComparer());
        }

        [Fact]
        public void GetWithOptions_ArtistFilterForEmptyTable_ReturnEmpty()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist " });
            var options = new AlbumAccessOptions()
            {
                ArtistFilter = artist.Id
            };

            Assert.Empty(albumTable.Get(options));
        }

        [Fact]
        public void GetWithOptions_ArtistFilterForExistingAlbum_ValidateReturnCollection()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });

            int albumCount = 10;
            IList<Album> expectedAlbums = new List<Album>();
            for (int i = 0; i < albumCount; i++)
            {
                expectedAlbums.Add(albumTable.Add(new Album()
                {
                    Name = "TestAlbum" + (i + 1),
                    Artist = new Artist() { Id = artist.Id },
                    Genre = new Genre() { Id = genre.Id },
                    ReleaseYear = (i + 1)
                }));
            }

            var options = new AlbumAccessOptions()
            {
                ArtistFilter = artist.Id
            };

            var albums = albumTable.Get(options);
            Assert.Equal(expectedAlbums, albums, new AlbumComparer());
        }

        [Fact]
        public void GetWithOptions_GenreFilterForEmptyTable_ReturnEmpty()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist " });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var options = new AlbumAccessOptions()
            {
                GenreFilter = 1
            };

            Assert.Empty(albumTable.Get(options));
        }

        [Fact]
        public void GetWithOptions_GenreFilterForExistingAlbum_ValidateReturnCollection()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });

            int albumCount = 10;
            IList<Album> expectedAlbums = new List<Album>();
            for (int i = 0; i < albumCount; i++)
            {
                expectedAlbums.Add(albumTable.Add(new Album()
                {
                    Name = "TestAlbum" + (i + 1),
                    Artist = new Artist() { Id = artist.Id },
                    Genre = new Genre() { Id = genre.Id },
                    ReleaseYear = (i + 1)
                }));
            }

            var options = new AlbumAccessOptions()
            {
                GenreFilter = genre.Id
            };

            var albums = albumTable.Get(options);
            Assert.Equal(expectedAlbums, albums, new AlbumComparer());
        }

        [Fact]
        public void GetWithOptions_AlbumFilterAndArtistFilter_ReturnEmpty()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist " });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var options = new AlbumAccessOptions()
            {
                AlbumFilter = 1,
                ArtistFilter = 1
            };

            Assert.Empty(albumTable.Get(options));
        }

        [Fact]
        public void GetWithOptions_AlbumFilterAndArtistFilterForExistingAlbumButDifferentArtist_ReturnEmpty()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });

            int albumCount = 10;
            IList<Album> expectedAlbums = new List<Album>();
            for (int i = 0; i < albumCount; i++)
            {
                expectedAlbums.Add(albumTable.Add(new Album()
                {
                    Name = "TestAlbum" + (i + 1),
                    Artist = new Artist() { Id = artist.Id },
                    Genre = new Genre() { Id = genre.Id },
                    ReleaseYear = (i + 1)
                }));
            }

            var options = new AlbumAccessOptions()
            {
                AlbumFilter = 5,
                ArtistFilter = 2
            };

            Assert.Empty(albumTable.Get(options));
        }

        [Fact]
        public void GetWithOptions_AlbumFilterAndArtistFilterForExistingAlbum_ValidateReturnAlbum()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });

            int albumCount = 10;
            IList<Album> expectedAlbums = new List<Album>();
            for (int i = 0; i < albumCount; i++)
            {
                expectedAlbums.Add(albumTable.Add(new Album()
                {
                    Name = "TestAlbum" + (i + 1),
                    Artist = new Artist() { Id = artist.Id },
                    Genre = new Genre() { Id = genre.Id },
                    ReleaseYear = (i + 1)
                }));
            }

            var options = new AlbumAccessOptions()
            {
                AlbumFilter = 5,
                ArtistFilter = 1
            };

            var albums = albumTable.Get(options);
            Assert.Equal(1, albums.Count);
            Assert.Equal(expectedAlbums[4], albums[0], new AlbumComparer());
        }

        [Fact]
        public void GetWithOptions_GenreFilterAndArtistFilter_ReturnEmpty()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist " });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var options = new AlbumAccessOptions()
            {
                GenreFilter = genre.Id,
                ArtistFilter = artist.Id
            };

            Assert.Empty(albumTable.Get(options));
        }

        [Fact]
        public void GetWithOptions_GenreFilterAndArtistFilterForExistingAlbumWithDifferentGenre_ReturnEmpty()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });

            int albumCount = 10;
            IList<Album> expectedAlbums = new List<Album>();
            for (int i = 0; i < albumCount; i++)
            {
                expectedAlbums.Add(albumTable.Add(new Album()
                {
                    Name = "TestAlbum" + (i + 1),
                    Artist = new Artist() { Id = artist.Id },
                    Genre = new Genre() { Id = genre.Id },
                    ReleaseYear = (i + 1)
                }));
            }

            var options = new AlbumAccessOptions()
            {
                GenreFilter = 5,
                ArtistFilter = 1
            };

            Assert.Empty(albumTable.Get(options));
        }

        [Fact]
        public void GetWithOptions_GenreFilterAndArtistFilterForExistingAlbumWithDifferentArtist_ReturnEmpty()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });

            int albumCount = 10;
            IList<Album> expectedAlbums = new List<Album>();
            for (int i = 0; i < albumCount; i++)
            {
                expectedAlbums.Add(albumTable.Add(new Album()
                {
                    Name = "TestAlbum" + (i + 1),
                    Artist = new Artist() { Id = artist.Id },
                    Genre = new Genre() { Id = genre.Id },
                    ReleaseYear = (i + 1)
                }));
            }

            var options = new AlbumAccessOptions()
            {
                GenreFilter = 1,
                ArtistFilter = 2
            };

            Assert.Empty(albumTable.Get(options));
        }

        [Fact]
        public void GetWithOptions_GenreFilterAndArtistFilterForExistingAlbum_ValidateReturnCollection()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });

            int albumCount = 10;
            IList<Album> expectedAlbums = new List<Album>();
            for (int i = 0; i < albumCount; i++)
            {
                expectedAlbums.Add(albumTable.Add(new Album()
                {
                    Name = "TestAlbum" + (i + 1),
                    Artist = new Artist() { Id = artist.Id },
                    Genre = new Genre() { Id = genre.Id },
                    ReleaseYear = (i + 1)
                }));
            }

            var options = new AlbumAccessOptions()
            {
                GenreFilter = 1,
                ArtistFilter = 1
            };

            var albums = albumTable.Get(options);
            Assert.Equal(expectedAlbums, albums, new AlbumComparer());
        }

        [Fact]
        public void GetWithOptions_AllFilters_ValidateReturnAlbum()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });

            int albumCount = 10;
            IList<Album> expectedAlbums = new List<Album>();
            for (int i = 0; i < albumCount; i++)
            {
                expectedAlbums.Add(albumTable.Add(new Album()
                {
                    Name = "TestAlbum" + (i + 1),
                    Artist = new Artist() { Id = artist.Id },
                    Genre = new Genre() { Id = genre.Id },
                    ReleaseYear = (i + 1)
                }));
            }

            var options = new AlbumAccessOptions()
            {
                GenreFilter = 1,
                ArtistFilter = 1,
                AlbumFilter = 4,
                AlbumNameFilter = "TestAlbum4"
            };

            var albums = albumTable.Get(options);
            Assert.Equal(1, albums.Count);
            Assert.Equal(expectedAlbums[3], albums[0], new AlbumComparer());
        }

        [Fact]
        public void GetWithOptions_IncludeArtist_Validate()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var album = albumTable.Add(new Album()
            {
                Name = "TestAlbum",
                ReleaseYear = 2000,
                Artist = artist,
                Genre = new Genre { Id = genre.Id }
            });

            var options = new AlbumAccessOptions()
            {
                IncludeArtist = true
            };
            var albums = albumTable.Get(options);
            Assert.Equal(1, albums.Count);
            Assert.Equal(album, albums[0], new AlbumComparer());
        }

        [Fact]
        public void GetWithOptions_IncludeGenre_Validate()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var album = albumTable.Add(new Album()
            {
                Name = "TestAlbum",
                ReleaseYear = 2000,
                Artist = new Artist { Id = artist.Id },
                Genre = genre
            });

            var options = new AlbumAccessOptions()
            {
                IncludeGenre = true
            };
            var albums = albumTable.Get(options);
            Assert.Equal(1, albums.Count);
            Assert.Equal(album, albums[0], new AlbumComparer());
        }

        [Fact]
        public void GetWithOptions_IncludeArtistAndGenre_Validate()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var album = albumTable.Add(new Album()
            {
                Name = "TestAlbum",
                ReleaseYear = 2000,
                Artist = artist,
                Genre = genre
            });

            var options = new AlbumAccessOptions()
            {
                IncludeGenre = true,
                IncludeArtist = true
            };
            var albums = albumTable.Get(options);
            Assert.Equal(1, albums.Count);
            Assert.Equal(album, albums[0], new AlbumComparer());
        }

        [Fact]
        public void GetCount_NoAlbums_ValidateZero()
        {
            Assert.Equal(0, albumTable.GetCount());
        }

        [Fact]
        public void GetCount_FiveAlbums_ValidateFive()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });

            int albumCount = 5;
            for (int i = 0; i < albumCount; i++)
            {
                albumTable.Add(new Album()
                {
                    Name = "TestAlbum" + (i + 1),
                    Artist = new Artist() { Id = artist.Id },
                    Genre = new Genre() { Id = genre.Id },
                    ReleaseYear = (i + 1)
                });
            }

            Assert.Equal(5, albumTable.GetCount());
        }

        [Fact]
        public void GetCount_AlbumFilter_ValidateCount()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var album = albumTable.Add(new Album()
            {
                Name = "TestAlbum",
                Artist = new Artist() { Id = artist.Id },
                Genre = new Genre() { Id = genre.Id },
                ReleaseYear = 2000
            });


            var options = new AlbumAccessOptions()
            {
                AlbumFilter = album.Id
            };

            Assert.Equal(1, albumTable.GetCount(options));
        }

        [Fact]
        public void GetCount_ArtistFilter_ValidateCount()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });

            for (int i = 0; i < 5; i++)
            {
                albumTable.Add(new Album()
                {
                    Name = "TestAlbum",
                    Artist = new Artist() { Id = artist.Id },
                    Genre = new Genre() { Id = genre.Id },
                    ReleaseYear = 2000
                });
            }

            var artist2 = artistTable.Add(new Artist() { Name = "TestArtist2" });
            albumTable.Add(new Album()
            {
                Name = "TestAlbum",
                Artist = new Artist() { Id = artist2.Id },
                Genre = new Genre() { Id = genre.Id },
                ReleaseYear = 2000
            });


            var options = new AlbumAccessOptions()
            {
                ArtistFilter = artist.Id
            };

            Assert.Equal(6, albumTable.GetCount());
            Assert.Equal(5, albumTable.GetCount(options));
        }

        [Fact]
        public void GetCount_GenreFilter_ValidateCount()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });

            for (int i = 0; i < 5; i++)
            {
                albumTable.Add(new Album()
                {
                    Name = "TestAlbum",
                    Artist = new Artist() { Id = artist.Id },
                    Genre = new Genre() { Id = genre.Id },
                    ReleaseYear = 2000
                });
            }

            var genre2 = genreTable.Add(new Genre() { Name = "TestGenre2" });
            albumTable.Add(new Album()
            {
                Name = "TestAlbum",
                Artist = new Artist() { Id = artist.Id },
                Genre = new Genre() { Id = genre2.Id },
                ReleaseYear = 2000
            });


            var options = new AlbumAccessOptions()
            {
                GenreFilter = genre.Id
            };

            Assert.Equal(6, albumTable.GetCount());
            Assert.Equal(5, albumTable.GetCount(options));
        }

        [Fact]
        public void Update_NoId_Throw()
        {
            var album = new Album()
            {
                Name = "TestAlbum",
                ReleaseYear = 2000,
            };

            Assert.Throws<ArgumentException>(() => albumTable.Update(album));
        }

        [Fact]
        public void Update_ArtistNullOrNoArtistId_Throw()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var album = albumTable.Add(new Album()
            {
                Name = "TestAlbum",
                ReleaseYear = 1999,
                Artist = artist,
                Genre = genre
            });

            album.Name = "ModifiedTestAlbum";
            album.Artist = null;
            Assert.Throws<ArgumentNullException>(() => albumTable.Update(album));

            album.Artist = new Artist() { Name = "ArtistWithNoId" };
            Assert.Throws<ArgumentNullException>(() => albumTable.Update(album));
        }

        [Fact]
        public void Update_GenreNullOrNoGenreId_Throw()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var album = albumTable.Add(new Album()
            {
                Name = "TestAlbum",
                ReleaseYear = 1999,
                Artist = artist,
                Genre = genre
            });

            album.Name = "ModifiedTestAlbum";
            album.Genre = null;
            Assert.Throws<ArgumentNullException>(() => albumTable.Update(album));

            album.Genre = new Genre() { Name = "GenreWithNoId" };
            Assert.Throws<ArgumentNullException>(() => albumTable.Update(album));
        }

        class AlbumComparer : IEqualityComparer<Album>
        {
            public bool Equals(Album x, Album y)
            {
                CompareAndAssert(x, y);
                return true;
            }

            public int GetHashCode(Album obj)
            {
                return obj.GetHashCode();
            }
        }

        internal static void CompareAndAssert(Album first, Album second)
        {
            if (first == second) return;
            Assert.Equal(first.Id, second.Id);
            Assert.Equal(first.Name, second.Name);
            Assert.Equal(first.ReleaseYear, second.ReleaseYear);
            ArtistTableTest.CompareAndAssert(first.Artist, second.Artist);
            GenreTableTest.CompareAndAssert(first.Genre, second.Genre);
        }

        public void Dispose()
        {
            connection?.Dispose();
        }

    }
}

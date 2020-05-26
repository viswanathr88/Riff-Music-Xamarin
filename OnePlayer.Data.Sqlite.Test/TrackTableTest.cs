using Microsoft.Data.Sqlite;
using OnePlayer.Data.Access;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace OnePlayer.Data.Sqlite.Test
{
    public sealed class TrackTableTest : IDisposable
    {
        private readonly string dbPath = ":memory:";
        private readonly SqliteConnection connection;
        private readonly ArtistTable artistTable;
        private readonly GenreTable genreTable;
        private readonly AlbumTable albumTable;
        private readonly TrackTable trackTable;

        public TrackTableTest()
        {
            connection = new SqliteConnection($"Data Source = {dbPath}");
            connection.Open();

            artistTable = new ArtistTable(connection);
            artistTable.HandleUpgrade(Version.Initial);

            genreTable = new GenreTable(connection);
            genreTable.HandleUpgrade(Version.Initial);

            albumTable = new AlbumTable(connection);
            albumTable.HandleUpgrade(Version.Initial);

            trackTable = new TrackTable(connection);
            trackTable.HandleUpgrade(Version.Initial);
        }

        [Fact]
        public void Constructor_NullConnection_Throw()
        {
            Assert.Throws<ArgumentNullException>(() => new TrackTable(null));
        }

        [Fact]
        public void Add_NullObject_Throw()
        {
            Assert.Throws<ArgumentNullException>(() => trackTable.Add(null));
        }

        [Fact]
        public void Add_WithExistingId_Throw()
        {
            Assert.Throws<ArgumentException>(() => trackTable.Add(new Track() { Id = 1 }));
        }

        [Fact]
        public void Add_NoAlbumId_Throw()
        {
            Assert.Throws<ArgumentNullException>(() => trackTable.Add(new Track() { Album = new Album() { Id = null } }));
        }

        [Fact]
        public void Add_NoGenreId_Throw()
        {
            Assert.Throws<ArgumentNullException>(() => trackTable.Add(new Track() { Album = new Album() { Id = 1 }, Genre = new Genre() }));
        }

        [Fact]
        public void Add_ValidItem_ValidateId()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var album = albumTable.Add(new Album() { Name = "TestAlbum", ReleaseYear = 2000, Artist = new Artist() { Id = artist.Id }, Genre = new Genre() { Id = genre.Id } });

            var track = trackTable.Add(new Track()
            {
                Bitrate = 256,
                ReleaseYear = 1999,
                Title = "TestTrack",
                Album = new Album() { Id = album.Id },
                Artist = "TestArtist",
                Genre = new Genre() { Id = genre.Id},
                Number = 5
            });

            Assert.NotNull(track);
            Assert.NotNull(track.Id);
            Assert.Equal(1, track.Id);
        }

        [Fact]
        public void Get_NonExistentTrack_ReturnNull()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var album = albumTable.Add(new Album() { Name = "TestAlbum", ReleaseYear = 2000, Artist = new Artist() { Id = artist.Id }, Genre = new Genre() { Id = genre.Id } });

            Assert.Null(trackTable.Get(1));
        }

        [Fact]
        public void Get_ExistingTrack_ValidateFields()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var album = albumTable.Add(new Album() { Name = "TestAlbum", ReleaseYear = 2000, Artist = new Artist() { Id = artist.Id }, Genre = new Genre() { Id = genre.Id } });

            var track = trackTable.Add(new Track()
            {
                Bitrate = 256,
                ReleaseYear = 1999,
                Title = "TestTrack",
                Album = new Album() { Id = album.Id },
                Artist = "TestArtist",
                Genre = new Genre() { Id = genre.Id },
                Number = 5
            });

            Assert.Equal(track, trackTable.Get(track.Id.Value), new TrackComparer());
        }

        [Fact]
        public void Get_ManyExistingTracks_ValidateFields()
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

            IList<Track> tracks = new List<Track>();
            for (int i = 0; i < 5; i++)
            {
                tracks.Add(trackTable.Add(new Track() { Title = "TestTrack" + (i + 1), Album = new Album() { Id = 3 }, Genre = new Genre() { Id = 4 }, Number = 4 }));
            }


            CompareAndAssert(tracks[2], trackTable.Get(tracks[2].Id.Value));
        }

        [Fact]
        public void GetWithOptions_NullOptions_Throw()
        {
            Assert.Throws<ArgumentNullException>(() => trackTable.Get(null));
        }

        [Fact]
        public void GetWithOptions_AlbumFilterWithNonExistentAlbum_ReturnEmpty()
        {
            var options = new TrackAccessOptions()
            {
                AlbumFilter = 1
            };

            Assert.Empty(trackTable.Get(options));
        }

        [Fact]
        public void GetWithOptions_AlbumFilterForExistingAlbum_ReturnTracksFromAlbum()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var album = albumTable.Add(new Album() { Name = "TestAlbum", Artist = new Artist() { Id = artist.Id }, Genre = new Genre() { Id = genre.Id } });

            int trackCount = 10;
            IList<Track> expectedTracks = new List<Track>();
            for (int i = 0; i < trackCount; i++)
            {
                expectedTracks.Add(trackTable.Add(new Track()
                {
                    Title = "TestTrack" + (i + 1),
                    Genre = new Genre() { Id = genre.Id },
                    Album = new Album() { Id = album.Id },
                    ReleaseYear = (i + 1)
                }));
            }

            var options = new TrackAccessOptions()
            {
                AlbumFilter = album.Id
            };

            var tracks = trackTable.Get(options);
            Assert.Equal(trackCount, tracks.Count);
            Assert.Equal(expectedTracks, tracks, new TrackComparer());
        }

        [Fact]
        public void GetWithOptions_GenreFilterWithNonExistentGenre_ReturnEmpty()
        {
            var options = new TrackAccessOptions()
            {
                GenreFilter = 1
            };

            Assert.Empty(trackTable.Get(options));
        }

        [Fact]
        public void GetWithOptions_GenreFilterForExistingGenre_ReturnTracksFromGenre()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var album = albumTable.Add(new Album() { Name = "TestAlbum", Artist = new Artist() { Id = artist.Id }, Genre = new Genre() { Id = genre.Id } });

            int trackCount = 10;
            IList<Track> expectedTracks = new List<Track>();
            for (int i = 0; i < trackCount; i++)
            {
                expectedTracks.Add(trackTable.Add(new Track()
                {
                    Title = "TestTrack" + (i + 1),
                    Genre = new Genre() { Id = genre.Id },
                    Album = new Album() { Id = album.Id },
                    ReleaseYear = (i + 1)
                }));
            }

            var options = new TrackAccessOptions()
            {
                GenreFilter = genre.Id
            };

            var tracks = trackTable.Get(options);
            Assert.Equal(trackCount, tracks.Count);
            Assert.Equal(expectedTracks, tracks, new TrackComparer());
        }

        [Fact]
        public void GetWithOptions_ExistingAlbumFilterAndNonExistingGenreFilter_ReturnEmpty()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var album = albumTable.Add(new Album() { Name = "TestAlbum", Artist = new Artist() { Id = artist.Id }, Genre = new Genre() { Id = genre.Id } });
            var track = trackTable.Add(new Track() { Title = "TestTrack", Album = new Album() { Id = album.Id }, Genre = new Genre() { Id = genre.Id } });

            var options = new TrackAccessOptions()
            {
                AlbumFilter = album.Id,
                GenreFilter = 3
            };

            Assert.Empty(trackTable.Get(options));
        }

        [Fact]
        public void GetWithOptions_NonExistingAlbumFilterAndExistingGenreFilter_ReturnEmpty()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var album = albumTable.Add(new Album() { Name = "TestAlbum", Artist = new Artist() { Id = artist.Id }, Genre = new Genre() { Id = genre.Id } });
            var track = trackTable.Add(new Track() { Title = "TestTrack", Album = new Album() { Id = album.Id }, Genre = new Genre() { Id = genre.Id } });
            var options = new TrackAccessOptions()
            {
                AlbumFilter = 3,
                GenreFilter = genre.Id
            };

            Assert.Empty(trackTable.Get(options));
        }

        [Fact]
        public void GetWithOptions_ExistingAlbumFilterAndExistingGenreFilter_ReturnValidCollection()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var album = albumTable.Add(new Album() { Name = "TestAlbum", Artist = new Artist() { Id = artist.Id }, Genre = new Genre() { Id = genre.Id } });
            var track = trackTable.Add(new Track() { Title = "TestTrack", Album = new Album() { Id = album.Id }, Genre = new Genre() { Id = genre.Id } });

            var options = new TrackAccessOptions()
            {
                AlbumFilter = album.Id,
                GenreFilter = genre.Id
            };

            Assert.Equal(track, trackTable.Get(options).First(), new TrackComparer());
        }

        [Fact]
        public void GetWithOptions_IncludeAlbum_Validate()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var album = albumTable.Add(new Album() { Name = "TestAlbum", ReleaseYear = 2000, Artist = artist, Genre = new Genre { Id = genre.Id }});
            var track = trackTable.Add(new Track() { Title = "TestTrack", Album = album, Genre = new Genre() { Id = genre.Id } });
            track.Album.Artist = new Artist() { Id = album.Id };

            var options = new TrackAccessOptions()
            {
                IncludeAlbum = true
            };
            var tracks = trackTable.Get(options);
            Assert.Equal(1, tracks.Count);
            Assert.Equal(track, tracks[0], new TrackComparer());
        }

        [Fact]
        public void GetWithOptions_IncludeGenre_Validate()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var album = albumTable.Add(new Album() { Name = "TestAlbum", ReleaseYear = 2000, Artist = new Artist { Id = artist.Id }, Genre = genre });
            var track = trackTable.Add(new Track() { Title = "TestTrack", Album = new Album() { Id = album.Id }, Genre = genre });

            var options = new TrackAccessOptions()
            {
                IncludeGenre = true
            };
            var tracks = trackTable.Get(options);
            Assert.Equal(1, tracks.Count);
            Assert.Equal(track, tracks[0], new TrackComparer());
        }

        [Fact]
        public void GetWithOptions_IncludeAlbumAndGenre_Validate()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var album = albumTable.Add(new Album() { Name = "TestAlbum", ReleaseYear = 2000, Artist = artist, Genre = genre });
            var track = trackTable.Add(new Track() { Title = "TestTrack", Album = album, Genre = genre });
            track.Album.Artist = new Artist() { Id = album.Id };
            track.Album.Genre = new Genre() { Id = genre.Id };

            var options = new TrackAccessOptions()
            {
                IncludeGenre = true,
                IncludeAlbum = true
            };
            var tracks = trackTable.Get(options);
            Assert.Equal(1, tracks.Count);
            Assert.Equal(track, tracks[0], new TrackComparer());
        }

        [Fact]
        public void GetCount_NoTracks_ValidateZero()
        {
            Assert.Equal(0, trackTable.GetCount());
        }

        [Fact]
        public void GetCount_FiveTracks_ValidateFive()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var album = albumTable.Add(new Album() { Name = "TestAlbum", ReleaseYear = 2000, Artist = artist, Genre = genre });


            int trackCount = 5;
            for (int i = 0; i < trackCount; i++)
            {
                trackTable.Add(new Track()
                {
                    Title = "TestTrack" + (i + 1),
                    Album = new Album() { Id = album.Id },
                    Genre = new Genre() { Id = genre.Id },
                    ReleaseYear = (i + 1)
                });
            }

            Assert.Equal(5, trackTable.GetCount());
        }

        [Fact]
        public void GetCount_AlbumFilter_ValidateCount()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var album = albumTable.Add(new Album() { Name = "TestAlbum", ReleaseYear = 2000, Artist = artist, Genre = genre });
            var track = trackTable.Add(new Track() { Title = "TestTrack", Album = album, Genre = genre });

            var options = new TrackAccessOptions()
            {
                AlbumFilter = album.Id
            };

            Assert.Equal(1, trackTable.GetCount(options));
        }

        [Fact]
        public void GetCount_TrackFilter_ValidateCount()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var album = albumTable.Add(new Album() { Name = "TestAlbum", ReleaseYear = 2000, Artist = artist, Genre = genre });
            var track = trackTable.Add(new Track() { Title = "TestTrack", Album = album, Genre = genre });

            var options = new TrackAccessOptions()
            {
                TrackFilter = track.Id
            };

            Assert.Equal(1, trackTable.GetCount(options));
        }

        [Fact]
        public void GetCount_GenreFilter_ValidateCount()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var album = albumTable.Add(new Album() { Name = "TestAlbum", ReleaseYear = 2000, Artist = artist, Genre = genre });
            var track = trackTable.Add(new Track() { Title = "TestTrack", Album = album, Genre = genre });

            var options = new TrackAccessOptions()
            {
                GenreFilter = genre.Id
            };

            Assert.Equal(1, trackTable.GetCount(options));
        }

        [Fact]
        public void Update_NullTrack_Throw()
        {
            Assert.Throws<ArgumentNullException>(() => trackTable.Update(null));
        }

        [Fact]
        public void Update_NoId_Throw()
        {
            var track = new Track()
            {
                Title="TestTile",
                ReleaseYear = 2000,
            };

            Assert.Throws<ArgumentNullException>(() => trackTable.Update(track));
        }

        [Fact]
        public void Update_AlbumNullOrNoAlbumId_Throw()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var album = albumTable.Add(new Album() { Name = "TestAlbum", ReleaseYear = 2000, Artist = artist, Genre = genre });
            var track = trackTable.Add(new Track() { Title = "TestTrack", Album = album, Genre = genre });

            track.Title = "ModifiedTestTrack";
            track.Album = null;
            Assert.Throws<ArgumentNullException>(() => trackTable.Update(track));

            track.Album = new Album() { Name = "AlbumWithNoId" };
            Assert.Throws<ArgumentNullException>(() => trackTable.Update(track));
        }

        [Fact]
        public void Update_GenreNullOrNoGenreId_Throw()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var album = albumTable.Add(new Album() { Name = "TestAlbum", ReleaseYear = 2000, Artist = artist, Genre = genre });
            var track = trackTable.Add(new Track() { Title = "TestTrack", Album = album, Genre = genre });

            track.Title = "ModifiedTestTrack";
            track.Genre = null;
            Assert.Throws<ArgumentNullException>(() => trackTable.Update(track));

            track.Genre = new Genre() { Name = "GenreWithNoId" };
            Assert.Throws<ArgumentNullException>(() => trackTable.Update(track));
        }

        [Fact]
        public void Update_ValidFields_ValidateReturn()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var album = albumTable.Add(new Album() { Name = "TestAlbum", ReleaseYear = 2000, Artist = artist, Genre = genre });
            var track = trackTable.Add(new Track() { Title = "TestTrack", Album = album, Genre = genre, Composers = "TestComposers", Duration = TimeSpan.FromMilliseconds(42454), Bitrate = 500, Number = 4, ReleaseYear = 2000, Artist = "TestTrackArtist" });

            track.Title = "ModifiedTestTrack";
            track.Composers = "ModifiedTestComposers";
            track.Artist = "ModifiedTestTrackArtist";
            track.ReleaseYear = 2050;

            Assert.Equal(track, trackTable.Update(track), new TrackComparer());
        }

        [Fact]
        public void Update_ValidFields_ValidateGet()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var album = albumTable.Add(new Album() { Name = "TestAlbum", ReleaseYear = 2000, Artist = artist, Genre = genre });
            var track = trackTable.Add(new Track() { Title = "TestTrack", Album = new Album() { Id = album.Id }, Genre = new Genre() { Id = genre.Id }, Composers = "TestComposers", Duration = TimeSpan.FromMilliseconds(42454), Bitrate = 500, Number = 4, ReleaseYear = 2000, Artist = "TestTrackArtist" });

            track.Title = "ModifiedTestTrack";
            track.Composers = "ModifiedTestComposers";
            track.Artist = "ModifiedTestTrackArtist";
            track.ReleaseYear = 2050;

            var modifiedTrack = trackTable.Update(track);

            Assert.Equal(modifiedTrack, trackTable.Get(track.Id.Value), new TrackComparer());
        }

        public void Dispose()
        {
            connection?.Dispose();
        }

        internal static void CompareAndAssert(Track x, Track y)
        {
            Assert.Equal(x.Id, y.Id);
            Assert.Equal(x.Number, y.Number);
            Assert.Equal(x.ReleaseYear, y.ReleaseYear);
            Assert.Equal(x.Title, y.Title);
            Assert.Equal(x.Artist, y.Artist);
            Assert.Equal(x.Bitrate, y.Bitrate);
            Assert.Equal(x.Composers, y.Composers);
            Assert.Equal(x.Duration, y.Duration);
            GenreTableTest.CompareAndAssert(x.Genre, y.Genre);
            AlbumTableTest.CompareAndAssert(x.Album, y.Album);
        }

        class TrackComparer : IEqualityComparer<Track>
        {
            public bool Equals(Track x, Track y)
            {
                CompareAndAssert(x, y);
                return true;
            }

            public int GetHashCode(Track obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}

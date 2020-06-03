using Microsoft.Data.Sqlite;
using OnePlayer.Data.Access;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace OnePlayer.Data.Sqlite.Test
{
    public sealed class DriveItemTableTest
    {
        private readonly string dbPath = ":memory:";
        private readonly SqliteConnection connection;
        private readonly DriveItemTable driveItemTable;
        private readonly TrackTable trackTable;
        private readonly AlbumTable albumTable;
        private readonly ArtistTable artistTable;
        private readonly GenreTable genreTable;

        public DriveItemTableTest()
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

            driveItemTable = new DriveItemTable(connection);
            driveItemTable.HandleUpgrade(Version.Initial);
        }

        [Fact]
        public void Add_NullObject_Throw()
        {
            Assert.Throws<ArgumentNullException>(() => driveItemTable.Add(null));
        }

        [Fact]
        public void Add_ObjectWithNoId_Throw()
        {
            Assert.Throws<ArgumentException>(() => driveItemTable.Add(new DriveItem() { Id = string.Empty }));
            Assert.Throws<ArgumentException>(() => driveItemTable.Add(new DriveItem() { Id = null }));
        }

        [Fact]
        public void Add_ItemWithNoTrack_Throw()
        {
            Assert.Throws<ArgumentNullException>(() => driveItemTable.Add(new DriveItem() { Id = "1", Track = null }));
            Assert.Throws<ArgumentNullException>(() => driveItemTable.Add(new DriveItem() { Id = "1", Track = new Track() }));
        }

        [Fact]
        public void Add_ValidItem_ValidateReturn()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var album = albumTable.Add(new Album() { Name = "TestAlbum", Artist = new Artist() { Id = artist.Id }, Genre = new Genre() { Id = genre.Id } });
            var track = trackTable.Add(new Track() { Title = "TestTrack", Album = new Album() { Id = album.Id }, Genre = new Genre() { Id = genre.Id } });

            var expectedItem = new DriveItem()
            {
                Id = "TestDriveItemId",
                AddedDate = new DateTime(2000, 10, 5),
                CTag = "TestCTag",
                ETag = "TestETag",
                DownloadUrl = "TestDownloadUrl",
                LastModified = new DateTime(2010, 10, 15),
                Size = 200204,
                Source = DriveItemSource.OneDrive,
                Track = new Track() { Id = track.Id }
            };

            var actualItem = driveItemTable.Add(expectedItem);

            Assert.Equal(actualItem, expectedItem, new DriveItemComparer());
        }

        [Fact]
        public void Get_EmptyNullId_Throw()
        {
            Assert.Throws<ArgumentNullException>(() => driveItemTable.Get(string.Empty));
        }

        [Fact]
        public void Get_ValidItem_Validate()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var album = albumTable.Add(new Album() { Name = "TestAlbum", ReleaseYear = 2000, Artist = new Artist() { Id = artist.Id }, Genre = new Genre() { Id = genre.Id } });
            var track = trackTable.Add(new Track() { Title = "TestTrack", Album = new Album() { Id = album.Id }, Genre = new Genre() { Id = genre.Id } });

            var expectedItem = driveItemTable.Add(new DriveItem()
            {
                Id = "TestDriveItemId",
                AddedDate = new DateTime(2000, 10, 5),
                CTag = "TestCTag",
                ETag = "TestETag",
                DownloadUrl = "TestDownloadUrl",
                LastModified = new DateTime(2010, 10, 15),
                Size = 200204,
                Source = DriveItemSource.OneDrive,
                Track = new Track() { Id = track.Id }
            });

            var actualItem = driveItemTable.Get("TestDriveItemId");
            Assert.Equal(actualItem, expectedItem, new DriveItemComparer());
        }

        [Fact]
        public void Get_WithOptions_Validate()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var album = albumTable.Add(new Album() { Name = "TestAlbum", ReleaseYear = 2000, Artist = new Artist() { Id = artist.Id }, Genre = new Genre() { Id = genre.Id } });
            var track = trackTable.Add(new Track() { Title = "TestTrack", Album = new Album() { Id = album.Id }, Genre = new Genre() { Id = genre.Id } });

            var expectedItem = driveItemTable.Add(new DriveItem()
            {
                Id = "TestDriveItemId",
                AddedDate = new DateTime(2000, 10, 5),
                CTag = "TestCTag",
                ETag = "TestETag",
                DownloadUrl = "TestDownloadUrl",
                LastModified = new DateTime(2010, 10, 15),
                Size = 200204,
                Source = DriveItemSource.OneDrive,
                Track = new Track() { Id = track.Id }
            });

            track.Album = album;
            expectedItem.Track = track;

            var options = new DriveItemAccessOptions()
            {
                IncludeTrack = true,
                IncludeTrackAlbum = true,
                SortOrder = SortOrder.Descending,
                SortType = TrackSortType.ReleaseYear
            };

            var actualItems = driveItemTable.Get(options);
            Assert.Equal(actualItems, new List<DriveItem>() { expectedItem }, new DriveItemComparer());
        }

        [Fact]
        public void Update_NullId_Throw()
        {
            Assert.Throws<ArgumentNullException>(() => driveItemTable.Update(null));
        }

        [Fact]
        public void Update_NoTrack_Throw()
        {
            var driveItem = new DriveItem()
            {
                Id = "TestId",
                AddedDate = new DateTime(2000, 10, 5),
                CTag = "TestCTag",
                ETag = "TestETag",
                DownloadUrl = "TestDownloadUrl",
                LastModified = new DateTime(2010, 10, 15),
                Size = 200204,
                Source = DriveItemSource.OneDrive
            };

            Assert.Throws<ArgumentNullException>(() => driveItemTable.Update(driveItem));
        }

        [Fact]
        public void Update_NoTrackId_Throw()
        {
            var driveItem = new DriveItem()
            {
                Id = "TestId",
                AddedDate = new DateTime(2000, 10, 5),
                CTag = "TestCTag",
                ETag = "TestETag",
                DownloadUrl = "TestDownloadUrl",
                LastModified = new DateTime(2010, 10, 15),
                Size = 200204,
                Source = DriveItemSource.OneDrive,
                Track = new Track()
            };

            Assert.Throws<ArgumentNullException>(() => driveItemTable.Update(driveItem));
        }

        [Fact]
        public void Update_NoId_Throw()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var album = albumTable.Add(new Album() { Name = "TestAlbum", Artist = new Artist() { Id = artist.Id }, Genre = new Genre() { Id = genre.Id } });
            var track = trackTable.Add(new Track() { Title = "TestTrack", Album = new Album() { Id = album.Id }, Genre = new Genre() { Id = genre.Id } });

            var driveItem = new DriveItem()
            {
                AddedDate = new DateTime(2000, 10, 5),
                CTag = "TestCTag",
                ETag = "TestETag",
                DownloadUrl = "TestDownloadUrl",
                LastModified = new DateTime(2010, 10, 15),
                Size = 200204,
                Source = DriveItemSource.OneDrive,
                Track = new Track() { Id = track.Id }
            };

            Assert.Throws<ArgumentException>(() => driveItemTable.Update(driveItem));
        }

        [Fact]
        public void Update_ValidModify_VaidateReturn()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var album = albumTable.Add(new Album() { Name = "TestAlbum", Artist = new Artist() { Id = artist.Id }, Genre = new Genre() { Id = genre.Id } });
            var track = trackTable.Add(new Track() { Title = "TestTrack", Album = new Album() { Id = album.Id }, Genre = new Genre() { Id = genre.Id } });

            var driveItem = driveItemTable.Add(new DriveItem()
            {
                Id = "TestDriveItemId",
                AddedDate = new DateTime(2000, 10, 5),
                CTag = "TestCTag",
                ETag = "TestETag",
                DownloadUrl = "TestDownloadUrl",
                LastModified = new DateTime(2010, 10, 15),
                Size = 200204,
                Source = DriveItemSource.OneDrive,
                Track = new Track() { Id = track.Id }
            });

            driveItem.CTag = "ModifiedCTag";
            driveItem.ETag = "ModifiedETag";
            driveItem.DownloadUrl = "ModifiedDownloadUrl";
            driveItem.Size = 200400;
            driveItem.LastModified = DateTime.Now;

            var actualItem = driveItemTable.Update(driveItem);
            Assert.Equal(driveItem, actualItem, new DriveItemComparer());
        }

        [Fact]
        public void Update_ValidModify_VaidateWithGet()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var album = albumTable.Add(new Album() { Name = "TestAlbum", Artist = new Artist() { Id = artist.Id }, Genre = new Genre() { Id = genre.Id } });
            var track = trackTable.Add(new Track() { Title = "TestTrack", Album = new Album() { Id = album.Id }, Genre = new Genre() { Id = genre.Id } });

            var driveItem = driveItemTable.Add(new DriveItem()
            {
                Id = "TestDriveItemId",
                AddedDate = new DateTime(2000, 10, 5),
                CTag = "TestCTag",
                ETag = "TestETag",
                DownloadUrl = "TestDownloadUrl",
                LastModified = new DateTime(2010, 10, 15),
                Size = 200204,
                Source = DriveItemSource.OneDrive,
                Track = new Track() { Id = track.Id }
            });

            driveItem.CTag = "ModifiedCTag";
            driveItem.ETag = "ModifiedETag";
            driveItem.DownloadUrl = "ModifiedDownloadUrl";
            driveItem.Size = 200400;
            driveItem.LastModified = DateTime.Now;

            driveItemTable.Update(driveItem);
            var actualItem = driveItemTable.Get(driveItem.Id);
            Assert.Equal(driveItem, actualItem, new DriveItemComparer());
        }

        [Fact]
        public void Delete_NullEmptyId_Throw()
        {
            Assert.Throws<ArgumentNullException>(() => driveItemTable.Delete(null));
            Assert.Throws<ArgumentNullException>(() => driveItemTable.Delete(string.Empty));
        }

        [Fact]
        public void Delete_EntryDoesntExist_NoThrow()
        {
            Assert.Null(driveItemTable.Get("TestId"));
            driveItemTable.Delete("TestId");
        }

        [Fact]
        public void Delete_ExistingItem_ValidateWithGet()
        {
            var artist = artistTable.Add(new Artist() { Name = "TestArtist" });
            var genre = genreTable.Add(new Genre() { Name = "TestGenre" });
            var album = albumTable.Add(new Album() { Name = "TestAlbum", Artist = new Artist() { Id = artist.Id }, Genre = new Genre() { Id = genre.Id } });
            var track = trackTable.Add(new Track() { Title = "TestTrack", Album = new Album() { Id = album.Id }, Genre = new Genre() { Id = genre.Id } });

            var driveItem = driveItemTable.Add(new DriveItem()
            {
                Id = "TestDriveItemId",
                AddedDate = new DateTime(2000, 10, 5),
                CTag = "TestCTag",
                ETag = "TestETag",
                DownloadUrl = "TestDownloadUrl",
                LastModified = new DateTime(2010, 10, 15),
                Size = 200204,
                Source = DriveItemSource.OneDrive,
                Track = new Track() { Id = track.Id }
            });

            Assert.NotNull(driveItemTable.Get("TestDriveItemId"));
            driveItemTable.Delete("TestDriveItemId");
            Assert.Null(driveItemTable.Get("TestDriveItemId"));
        }

        internal static void CompareAndAssert(DriveItem first, DriveItem second)
        {
            if (first == second) return;
            Assert.Equal(first.AddedDate, second.AddedDate);
            Assert.Equal(first.CTag, second.CTag);
            Assert.Equal(first.DownloadUrl, second.DownloadUrl);
            Assert.Equal(first.ETag, second.ETag);
            Assert.Equal(first.Id, second.Id);
            Assert.Equal(first.LastModified, second.LastModified);
            Assert.Equal(first.Size, second.Size);
            Assert.Equal(first.Source, second.Source);
            TrackTableTest.CompareAndAssert(first.Track, second.Track);
        }

        public class DriveItemComparer : IEqualityComparer<DriveItem>
        {
            public bool Equals(DriveItem x, DriveItem y)
            {
                CompareAndAssert(x, y);
                return true;
            }

            public int GetHashCode(DriveItem obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}

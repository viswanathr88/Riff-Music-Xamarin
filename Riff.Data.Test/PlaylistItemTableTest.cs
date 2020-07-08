using Microsoft.Data.Sqlite;
using Riff.Data.Sqlite;
using System;
using System.Collections.Generic;
using Xunit;
using Version = Riff.Data.Sqlite.Version;

namespace Riff.Data.Test
{
    public sealed class PlaylistItemTableTest : IDisposable
    {
        private readonly string dbPath = ":memory:";
        private readonly SqliteConnection connection;
        private readonly PlaylistItemTable playlistItemTable;
        private readonly PlaylistTable playlistTable;
        private readonly DriveItemTable driveItemTable;
        private readonly TrackTable trackTable;
        private readonly AlbumTable albumTable;
        private readonly ArtistTable artistTable;
        private readonly GenreTable genreTable;

        public PlaylistItemTableTest()
        {
            connection = new SqliteConnection($"Data Source = {dbPath}");
            connection.Open();

            var extractor = new DataExtractor();

            artistTable = new ArtistTable(connection, extractor);
            artistTable.HandleUpgrade(Version.Initial);

            genreTable = new GenreTable(connection, extractor);
            genreTable.HandleUpgrade(Version.Initial);

            albumTable = new AlbumTable(connection, extractor);
            albumTable.HandleUpgrade(Version.Initial);

            trackTable = new TrackTable(connection, extractor);
            trackTable.HandleUpgrade(Version.Initial);

            driveItemTable = new DriveItemTable(connection, extractor);
            driveItemTable.HandleUpgrade(Version.Initial);

            playlistTable = new PlaylistTable(connection, extractor);
            playlistTable.HandleUpgrade(Version.AddPlaylists);

            playlistItemTable = new PlaylistItemTable(connection, extractor);
            playlistItemTable.HandleUpgrade(Version.AddPlaylists);
        }

        [Fact]
        public void Add_NullPlaylist_Throw()
        {
            Assert.Throws<ArgumentNullException>(() => playlistItemTable.Add(null, new PlaylistItem()));
        }

        [Fact]
        public void Add_NullPlaylistItem_Throw()
        {
            var playlist = new Playlist() { Id = 1, Name = "Test" };
            PlaylistItem item = null;
            Assert.Throws<ArgumentNullException>(() => playlistItemTable.Add(playlist, item));
        }

        [Fact]
        public void Add_NullPlaylistId_Throw()
        {
            var playlist = new Playlist() { Name = "Test" };
            var playlistItem = new PlaylistItem() { DriveItem = new DriveItem() { Id = "TestId" }, PlaylistId = 1 };
            Assert.Throws<ArgumentNullException>(() => playlistItemTable.Add(playlist, playlistItem));
        }

        [Fact]
        public void Add_EmptyTable_EnsureSuccessAndValidateFields()
        {
            var artist = artistTable.Add(new Artist() { Name = "Test Artist" });
            var genre = genreTable.Add(new Genre() { Name = "Test Genre" });
            var album = albumTable.Add(new Album() { Name = "Test Album", Artist = artist, Genre = genre, ReleaseYear = 2000 });
            var track = trackTable.Add(new Track() { Title = "Test Track", Artist = "Test Track Artist", Album = album, Genre = genre });
            var driveItem = driveItemTable.Add(new DriveItem() { Id = "TestDriveItemId", Track = track });
            var playlist = playlistTable.Add(new Playlist() { Name = "Test Playlist", LastModified = DateTime.Now });
            var playlistItem = new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value };

            Assert.Null(playlistItem.Id);
            var result = playlistItemTable.Add(playlist, playlistItem);
            Assert.NotNull(playlistItem.Id);
            Assert.NotNull(result.Id);
            Assert.Equal(1, playlistItem.Id);
            Assert.Equal(1, result.Id);
            Assert.NotNull(result);
            Assert.Null(result.Previous);
            Assert.Null(result.Next);
        }

        [Fact]
        public void Add_ThreeItems_ValidateNextAndPrevious()
        {
            var artist = artistTable.Add(new Artist() { Name = "Test Artist" });
            var genre = genreTable.Add(new Genre() { Name = "Test Genre" });
            var album = albumTable.Add(new Album() { Name = "Test Album", Artist = artist, Genre = genre, ReleaseYear = 2000 });
            var track = trackTable.Add(new Track() { Title = "Test Track", Artist = "Test Track Artist", Album = album, Genre = genre });
            var driveItem = driveItemTable.Add(new DriveItem() { Id = "TestDriveItemId", Track = track });
            var playlist = playlistTable.Add(new Playlist() { Name = "Test Playlist", LastModified = DateTime.Now });
            var playlistItem1 = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value });
            var playlistItem2 = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value });
            var playlistItem3 = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value });

            playlistItem1 = playlistItemTable.Get(playlistItem1.Id.Value);
            playlistItem2 = playlistItemTable.Get(playlistItem2.Id.Value);
            playlistItem3 = playlistItemTable.Get(playlistItem3.Id.Value);
            Assert.Null(playlistItem1.Previous);
            Assert.Equal(playlistItem2.Id, playlistItem1.Next);
            Assert.Equal(playlistItem1.Id, playlistItem2.Previous);
            Assert.Equal(playlistItem3.Id, playlistItem2.Next);
            Assert.Equal(playlistItem2.Id, playlistItem3.Previous);
            Assert.Null(playlistItem3.Next);
        }

        [Fact]
        public void Add_FewItems_ValidateWithGet()
        {
            var artist = artistTable.Add(new Artist() { Name = "Test Artist" });
            var genre = genreTable.Add(new Genre() { Name = "Test Genre" });
            var album = albumTable.Add(new Album() { Name = "Test Album", Artist = new Artist() { Id = artist.Id }, Genre = new Genre() { Id = genre.Id }, ReleaseYear = 2000 });
            var track = trackTable.Add(new Track() { Title = "Test Track", Artist = "Test Track Artist", Album = new Album() { Id = album.Id }, Genre = new Genre() { Id = genre.Id } });
            var driveItem = driveItemTable.Add(new DriveItem() { Id = "TestDriveItemId", Track = new Track() { Id = track.Id } });
            var playlist = playlistTable.Add(new Playlist() { Name = "Test Playlist", LastModified = DateTime.Now });
            var playlistItem1 = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value });
            var playlistItem2 = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value, Previous = playlistItem1.Id });
            var playlistItem3 = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value, Previous = playlistItem2.Id });
            driveItem.Track = null; // Don't expect Track to be returned
            playlistItem1.Previous = null;
            playlistItem1.Next = playlistItem2.Id;
            playlistItem2.Next = playlistItem3.Id;
            playlistItem3.Next = null;

            var actualItem1 = playlistItemTable.Get(playlistItem1.Id.Value);
            var actualItem2 = playlistItemTable.Get(playlistItem2.Id.Value);
            var actualItem3 = playlistItemTable.Get(playlistItem3.Id.Value);

            var comparer = new PlaylistItemComparer();
            Assert.Equal(playlistItem1, actualItem1, comparer);
            Assert.Equal(playlistItem2, actualItem2, comparer);
            Assert.Equal(playlistItem3, actualItem3, comparer);
        }

        [Fact]
        public void Get_NullOptions_Throw()
        {
            Assert.Throws<ArgumentNullException>(() => playlistItemTable.Get(null));
        }

        [Fact]
        public void Get_OptionsWithPlaylistItemFilter_ValidateItem()
        {
            var artist = artistTable.Add(new Artist() { Name = "Test Artist" });
            var genre = genreTable.Add(new Genre() { Name = "Test Genre" });
            var album = albumTable.Add(new Album() { Name = "Test Album", Artist = new Artist() { Id = artist.Id }, Genre = new Genre() { Id = genre.Id }, ReleaseYear = 2000 });
            var track = trackTable.Add(new Track() { Title = "Test Track", Artist = "Test Track Artist", Album = new Album() { Id = album.Id }, Genre = new Genre() { Id = genre.Id } });
            var driveItem = driveItemTable.Add(new DriveItem() { Id = "TestDriveItemId", Track = new Track() { Id = track.Id } });
            var playlist = playlistTable.Add(new Playlist() { Name = "Test Playlist", LastModified = DateTime.Now });
            var playlistItem1 = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value });
            var playlistItem2 = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value, Previous = playlistItem1.Id });
            driveItem.Track = null; // Don't expect Track to be returned
            playlistItem1.Previous = null;
            playlistItem1.Next = playlistItem2.Id;
            playlistItem2.Next = null;

            var options = new PlaylistItemAccessOptions()
            {
                PlaylistItemFilter = playlistItem2.Id
            };

            var actualPlaylistItems = playlistItemTable.Get(options);
            Assert.Equal(new List<PlaylistItem>() { playlistItem2 }, actualPlaylistItems, new PlaylistItemComparer());
        }

        [Fact]
        public void Get_OptionsWithNonExistentPlaylistFilter_ReturnEmptyCollection()
        {
            var artist = artistTable.Add(new Artist() { Name = "Test Artist" });
            var genre = genreTable.Add(new Genre() { Name = "Test Genre" });
            var album = albumTable.Add(new Album() { Name = "Test Album", Artist = new Artist() { Id = artist.Id }, Genre = new Genre() { Id = genre.Id }, ReleaseYear = 2000 });
            var track = trackTable.Add(new Track() { Title = "Test Track", Artist = "Test Track Artist", Album = new Album() { Id = album.Id }, Genre = new Genre() { Id = genre.Id } });
            var driveItem = driveItemTable.Add(new DriveItem() { Id = "TestDriveItemId", Track = new Track() { Id = track.Id } });
            var playlist = playlistTable.Add(new Playlist() { Name = "Test Playlist", LastModified = DateTime.Now });
            var playlistItem1 = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value });
            var playlistItem2 = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value, Previous = playlistItem1.Id });
            driveItem.Track = null; // Don't expect Track to be returned
            playlistItem1.Previous = null;
            playlistItem1.Next = playlistItem2.Id;
            playlistItem2.Next = null;

            var options = new PlaylistItemAccessOptions()
            {
                PlaylistFilter = 3
            };

            var actualPlaylistItems = playlistItemTable.Get(options);
            Assert.Empty(actualPlaylistItems);
        }

        [Fact]
        public void Get_OptionsWithExistingPlaylistFilter_ReturnOnlyPlaylistItems()
        {
            var artist = artistTable.Add(new Artist() { Name = "Test Artist" });
            var genre = genreTable.Add(new Genre() { Name = "Test Genre" });
            var album = albumTable.Add(new Album() { Name = "Test Album", Artist = new Artist() { Id = artist.Id }, Genre = new Genre() { Id = genre.Id }, ReleaseYear = 2000 });
            var track = trackTable.Add(new Track() { Title = "Test Track", Artist = "Test Track Artist", Album = new Album() { Id = album.Id }, Genre = new Genre() { Id = genre.Id } });
            var driveItem = driveItemTable.Add(new DriveItem() { Id = "TestDriveItemId", Track = new Track() { Id = track.Id } });
            var playlist = playlistTable.Add(new Playlist() { Name = "Test Playlist", LastModified = DateTime.Now });
            var playlist2 = playlistTable.Add(new Playlist() { Name = "Test Playlist 2", LastModified = DateTime.Now });

            var playlistItem1 = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value });
            var playlistItem2 = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist2.Id.Value, Previous = playlistItem1.Id });
            driveItem.Track = null; // Don't expect Track to be returned
            playlistItem1.Previous = null;
            playlistItem1.Next = playlistItem2.Id;
            playlistItem2.Next = null;

            var options = new PlaylistItemAccessOptions()
            {
                PlaylistFilter = playlist.Id
            };

            var actualPlaylistItems = playlistItemTable.Get(options);
            Assert.Equal(new List<PlaylistItem>() { playlistItem1 }, actualPlaylistItems, new PlaylistItemComparer());
        }

        [Fact]
        public void Get_OptionsWithBothPlaylistItemIdAndPlaylistFilterMismatch_ReturnPlaylistItemIdFilter()
        {
            var artist = artistTable.Add(new Artist() { Name = "Test Artist" });
            var genre = genreTable.Add(new Genre() { Name = "Test Genre" });
            var album = albumTable.Add(new Album() { Name = "Test Album", Artist = new Artist() { Id = artist.Id }, Genre = new Genre() { Id = genre.Id }, ReleaseYear = 2000 });
            var track = trackTable.Add(new Track() { Title = "Test Track", Artist = "Test Track Artist", Album = new Album() { Id = album.Id }, Genre = new Genre() { Id = genre.Id } });
            var driveItem = driveItemTable.Add(new DriveItem() { Id = "TestDriveItemId", Track = new Track() { Id = track.Id } });
            var playlist = playlistTable.Add(new Playlist() { Name = "Test Playlist", LastModified = DateTime.Now });
            var playlist2 = playlistTable.Add(new Playlist() { Name = "Test Playlist 2", LastModified = DateTime.Now });

            var playlistItem1 = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value });
            var playlistItem2 = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist2.Id.Value, Previous = playlistItem1.Id });
            driveItem.Track = null; // Don't expect Track to be returned
            playlistItem1.Previous = null;
            playlistItem1.Next = playlistItem2.Id;
            playlistItem2.Next = null;

            var options = new PlaylistItemAccessOptions()
            {
                PlaylistFilter = playlist.Id,
                PlaylistItemFilter = playlistItem2.Id
            };

            var actualPlaylistItems = playlistItemTable.Get(options);
            Assert.Equal(new List<PlaylistItem>() { playlistItem2 }, actualPlaylistItems, new PlaylistItemComparer());
        }

        [Fact]
        public void Get_OptionsIncludeDriveItemTrue_ValidateDriveItem()
        {
            var artist = artistTable.Add(new Artist() { Name = "Test Artist" });
            var genre = genreTable.Add(new Genre() { Name = "Test Genre" });
            var album = albumTable.Add(new Album() { Name = "Test Album", Artist = artist, Genre = genre, ReleaseYear = 2000 });
            var track = trackTable.Add(new Track() { Title = "Test Track", Artist = "Test Track Artist", Album = album, Genre = genre });
            var driveItem = driveItemTable.Add(new DriveItem() { Id = "TestDriveItemId", Track = track });
            var playlist = playlistTable.Add(new Playlist() { Name = "Test Playlist", LastModified = DateTime.Now });
            var playlistItem = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value });

            // Deeper Album properties wont be returned
            album.Genre = new Genre() { Id = album.Genre.Id };
            album.Artist = new Artist() { Id = album.Artist.Id };

            var options = new PlaylistItemAccessOptions()
            {
                PlaylistItemFilter = playlistItem.Id,
                IncludeDriveItem = true
            };

            var actualItems = playlistItemTable.Get(options);
            Assert.Equal(new List<PlaylistItem>() { playlistItem }, actualItems, new PlaylistItemComparer());
        }

        [Fact]
        public void Update_NullPlaylistItem_Throw()
        {
            Assert.Throws<ArgumentNullException>(() => playlistItemTable.Update(null));
        }

        [Fact]
        public void Update_PlaylistItemWithNoId_Throw()
        {
            var playlistItem = new PlaylistItem() { PlaylistId = 1, Next = null, Previous = null, DriveItem = new DriveItem() { Id = "TestId" } };
            Assert.Throws<ArgumentNullException>(() => playlistItemTable.Update(playlistItem));
        }

        [Fact]
        public void Update_PlaylistItemWithNoDriveItem_Throw()
        {
            var playlistItem = new PlaylistItem() { PlaylistId = 1, Next = null, Previous = null, DriveItem = null };
            Assert.Throws<ArgumentNullException>(() => playlistItemTable.Update(playlistItem));
        }

        [Fact]
        public void Update_PlaylistItemWithNoDriveItemId_Throw()
        {
            var playlistItem = new PlaylistItem() { Id = 1, PlaylistId = 1, Next = null, Previous = null, DriveItem = new DriveItem() };
            Assert.Throws<ArgumentNullException>(() => playlistItemTable.Update(playlistItem));
        }

        [Fact]
        public void Update_PlaylistItemUpdateDriveItem_ValidateUpdate()
        {
            var artist = artistTable.Add(new Artist() { Name = "Test Artist" });
            var genre = genreTable.Add(new Genre() { Name = "Test Genre" });
            var album = albumTable.Add(new Album() { Name = "Test Album", Artist = artist, Genre = genre, ReleaseYear = 2000 });
            var track = trackTable.Add(new Track() { Title = "Test Track", Artist = "Test Track Artist", Album = album, Genre = genre });
            var driveItem = driveItemTable.Add(new DriveItem() { Id = "TestDriveItemId", Track = track });
            var driveItem2 = driveItemTable.Add(new DriveItem() { Id = "TestDriveItemId2", Track = track });
            var playlist = playlistTable.Add(new Playlist() { Name = "Test Playlist", LastModified = DateTime.Now });
            var playlistItem = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value });

            Assert.Equal(playlistItem.DriveItem.Id, driveItem.Id);
            playlistItem.DriveItem = driveItem2;
            playlistItemTable.Update(playlistItem);

            var actualItem = playlistItemTable.Get(playlistItem.Id.Value);
            Assert.Equal(playlistItem.DriveItem.Id, actualItem.DriveItem.Id);
        }

        [Fact]
        public void Update_PlaylistItemUpdatePlaylist_ValidatePlaylistId()
        {
            var artist = artistTable.Add(new Artist() { Name = "Test Artist" });
            var genre = genreTable.Add(new Genre() { Name = "Test Genre" });
            var album = albumTable.Add(new Album() { Name = "Test Album", Artist = artist, Genre = genre, ReleaseYear = 2000 });
            var track = trackTable.Add(new Track() { Title = "Test Track", Artist = "Test Track Artist", Album = album, Genre = genre });
            var driveItem = driveItemTable.Add(new DriveItem() { Id = "TestDriveItemId", Track = track });
            var playlist = playlistTable.Add(new Playlist() { Name = "Test Playlist", LastModified = DateTime.Now });
            var playlist2 = playlistTable.Add(new Playlist() { Name = "Test Playlist 2", LastModified = DateTime.Now });
            var playlistItem = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value });

            Assert.Equal(playlistItem.PlaylistId, playlist.Id);
            playlistItem.PlaylistId = playlist2.Id.Value;
            playlistItemTable.Update(playlistItem);

            var actualItem = playlistItemTable.Get(playlistItem.Id.Value);
            Assert.Equal(playlist2.Id, actualItem.PlaylistId);
        }

        [Fact]
        public void Update_PlaylistItemUpdatePrevious_ValidatePrevious()
        {
            var artist = artistTable.Add(new Artist() { Name = "Test Artist" });
            var genre = genreTable.Add(new Genre() { Name = "Test Genre" });
            var album = albumTable.Add(new Album() { Name = "Test Album", Artist = artist, Genre = genre, ReleaseYear = 2000 });
            var track = trackTable.Add(new Track() { Title = "Test Track", Artist = "Test Track Artist", Album = album, Genre = genre });
            var driveItem = driveItemTable.Add(new DriveItem() { Id = "TestDriveItemId", Track = track });
            var playlist = playlistTable.Add(new Playlist() { Name = "Test Playlist", LastModified = DateTime.Now });
            var playlistItem = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value });
            var playlistItem2 = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value });
            Assert.Equal(playlistItem.Id, playlistItemTable.Get(playlistItem2.Id.Value).Previous);

            // Update previous
            playlistItem2.Previous = null;
            playlistItemTable.Update(playlistItem2);

            Assert.Null(playlistItemTable.Get(playlistItem2.Id.Value).Previous);
        }

        [Fact]
        public void Update_PlaylistItemUpdateNext_ValidateNext()
        {
            var artist = artistTable.Add(new Artist() { Name = "Test Artist" });
            var genre = genreTable.Add(new Genre() { Name = "Test Genre" });
            var album = albumTable.Add(new Album() { Name = "Test Album", Artist = artist, Genre = genre, ReleaseYear = 2000 });
            var track = trackTable.Add(new Track() { Title = "Test Track", Artist = "Test Track Artist", Album = album, Genre = genre });
            var driveItem = driveItemTable.Add(new DriveItem() { Id = "TestDriveItemId", Track = track });
            var playlist = playlistTable.Add(new Playlist() { Name = "Test Playlist", LastModified = DateTime.Now });
            var playlistItem = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value });
            var playlistItem2 = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value });
            Assert.Equal(playlistItem2.Id, playlistItemTable.Get(playlistItem.Id.Value).Next);
            Assert.NotNull(playlistItemTable.Get(playlistItem.Id.Value).Next);

            // Update next
            playlistItem.Next = null;
            playlistItemTable.Update(playlistItem);

            Assert.Null(playlistItemTable.Get(playlistItem.Id.Value).Next);
        }

        [Fact]
        public void Delete_PlaylistItemDoesntExist_NoThrow()
        {
            var artist = artistTable.Add(new Artist() { Name = "Test Artist" });
            var genre = genreTable.Add(new Genre() { Name = "Test Genre" });
            var album = albumTable.Add(new Album() { Name = "Test Album", Artist = artist, Genre = genre, ReleaseYear = 2000 });
            var track = trackTable.Add(new Track() { Title = "Test Track", Artist = "Test Track Artist", Album = album, Genre = genre });
            var driveItem = driveItemTable.Add(new DriveItem() { Id = "TestDriveItemId", Track = track });
            var playlist = playlistTable.Add(new Playlist() { Name = "Test Playlist", LastModified = DateTime.Now });
            var playlistItem = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value });

            Assert.Equal(1, playlistItem.Id);
            playlistItemTable.Delete(2);
        }

        [Fact]
        public void Delete_PlaylistItemExists_EnsureItemRemoved()
        {
            var artist = artistTable.Add(new Artist() { Name = "Test Artist" });
            var genre = genreTable.Add(new Genre() { Name = "Test Genre" });
            var album = albumTable.Add(new Album() { Name = "Test Album", Artist = artist, Genre = genre, ReleaseYear = 2000 });
            var track = trackTable.Add(new Track() { Title = "Test Track", Artist = "Test Track Artist", Album = album, Genre = genre });
            var driveItem = driveItemTable.Add(new DriveItem() { Id = "TestDriveItemId", Track = track });
            var playlist = playlistTable.Add(new Playlist() { Name = "Test Playlist", LastModified = DateTime.Now });
            var playlistItem = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value });

            Assert.NotNull(playlistItemTable.Get(playlistItem.Id.Value));
            playlistItemTable.Delete(playlist.Id.Value);
            Assert.Null(playlistItemTable.Get(playlistItem.Id.Value));
        }

        [Fact]
        public void Delete_RemoveFirstItem_EnsureSecondItemPreviousIsFixed()
        {
            var artist = artistTable.Add(new Artist() { Name = "Test Artist" });
            var genre = genreTable.Add(new Genre() { Name = "Test Genre" });
            var album = albumTable.Add(new Album() { Name = "Test Album", Artist = artist, Genre = genre, ReleaseYear = 2000 });
            var track = trackTable.Add(new Track() { Title = "Test Track", Artist = "Test Track Artist", Album = album, Genre = genre });
            var driveItem = driveItemTable.Add(new DriveItem() { Id = "TestDriveItemId", Track = track });
            var playlist = playlistTable.Add(new Playlist() { Name = "Test Playlist", LastModified = DateTime.Now });
            var playlistItem = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value });
            var playlistItem2 = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value });

            Assert.Equal(playlistItem.Id, playlistItemTable.Get(playlistItem2.Id.Value).Previous);
            playlistItemTable.Delete(playlistItem.Id.Value);
            Assert.Null(playlistItemTable.Get(playlistItem2.Id.Value).Previous);
        }

        [Fact]
        public void Delete_RemoveSecondItem_EnsureFirstItemNextIsFixed()
        {
            var artist = artistTable.Add(new Artist() { Name = "Test Artist" });
            var genre = genreTable.Add(new Genre() { Name = "Test Genre" });
            var album = albumTable.Add(new Album() { Name = "Test Album", Artist = artist, Genre = genre, ReleaseYear = 2000 });
            var track = trackTable.Add(new Track() { Title = "Test Track", Artist = "Test Track Artist", Album = album, Genre = genre });
            var driveItem = driveItemTable.Add(new DriveItem() { Id = "TestDriveItemId", Track = track });
            var playlist = playlistTable.Add(new Playlist() { Name = "Test Playlist", LastModified = DateTime.Now });
            var playlistItem = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value });
            var playlistItem2 = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value });

            Assert.Equal(playlistItem2.Id, playlistItemTable.Get(playlistItem.Id.Value).Next);
            playlistItemTable.Delete(playlistItem2.Id.Value);
            Assert.Null(playlistItemTable.Get(playlistItem.Id.Value).Next);
        }

        [Fact]
        public void Delete_MiddleItem_EnsureFirstAndSecondLinksAreFixed()
        {
            var artist = artistTable.Add(new Artist() { Name = "Test Artist" });
            var genre = genreTable.Add(new Genre() { Name = "Test Genre" });
            var album = albumTable.Add(new Album() { Name = "Test Album", Artist = artist, Genre = genre, ReleaseYear = 2000 });
            var track = trackTable.Add(new Track() { Title = "Test Track", Artist = "Test Track Artist", Album = album, Genre = genre });
            var driveItem = driveItemTable.Add(new DriveItem() { Id = "TestDriveItemId", Track = track });
            var playlist = playlistTable.Add(new Playlist() { Name = "Test Playlist", LastModified = DateTime.Now });
            var playlistItem = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value });
            var playlistItem2 = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value });
            var playlistItem3 = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value });

            Assert.Equal(2, playlistItemTable.Get(playlistItem.Id.Value).Next);
            Assert.Equal(3, playlistItemTable.Get(playlistItem2.Id.Value).Next);
            Assert.Null(playlistItemTable.Get(playlistItem3.Id.Value).Next);

            Assert.Null(playlistItemTable.Get(playlistItem.Id.Value).Previous);
            Assert.Equal(1, playlistItemTable.Get(playlistItem2.Id.Value).Previous);
            Assert.Equal(2, playlistItemTable.Get(playlistItem3.Id.Value).Previous);

            playlistItemTable.Delete(playlistItem2.Id.Value);

            Assert.Null(playlistItemTable.Get(playlistItem.Id.Value).Previous);
            Assert.Equal(1, playlistItemTable.Get(playlistItem3.Id.Value).Previous);

            Assert.Equal(3, playlistItemTable.Get(playlistItem.Id.Value).Next);
            Assert.Null(playlistItemTable.Get(playlistItem3.Id.Value).Next);
        }

        [Fact]
        public void Reorder_NullPlaylist_Throw()
        {
            Playlist playlist = null;
            Assert.Throws<ArgumentNullException>(() => playlistItemTable.Reorder(playlist, 0, 1, 4));
        }

        [Fact]
        public void Reorder_NewIndexOutOfBounds_Throw()
        {
            var artist = artistTable.Add(new Artist() { Name = "Test Artist" });
            var genre = genreTable.Add(new Genre() { Name = "Test Genre" });
            var album = albumTable.Add(new Album() { Name = "Test Album", Artist = artist, Genre = genre, ReleaseYear = 2000 });
            var track = trackTable.Add(new Track() { Title = "Test Track", Artist = "Test Track Artist", Album = album, Genre = genre });
            var driveItem = driveItemTable.Add(new DriveItem() { Id = "TestDriveItemId", Track = track });
            var playlist = playlistTable.Add(new Playlist() { Name = "Test Playlist", LastModified = DateTime.Now });
            var playlistItem = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value });
            var playlistItem2 = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value });

            Assert.Throws<ArgumentOutOfRangeException>(() => playlistItemTable.Reorder(playlist, 4, 0, 1));
        }

        [Fact]
        public void Reorder_OldIndexOutOfBounds_Throw()
        {
            var artist = artistTable.Add(new Artist() { Name = "Test Artist" });
            var genre = genreTable.Add(new Genre() { Name = "Test Genre" });
            var album = albumTable.Add(new Album() { Name = "Test Album", Artist = artist, Genre = genre, ReleaseYear = 2000 });
            var track = trackTable.Add(new Track() { Title = "Test Track", Artist = "Test Track Artist", Album = album, Genre = genre });
            var driveItem = driveItemTable.Add(new DriveItem() { Id = "TestDriveItemId", Track = track });
            var playlist = playlistTable.Add(new Playlist() { Name = "Test Playlist", LastModified = DateTime.Now });
            var playlistItem = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value });
            var playlistItem2 = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value });

            Assert.Throws<ArgumentOutOfRangeException>(() => playlistItemTable.Reorder(playlist, 0, 4, 1));
        }

        [Fact]
        public void Reorder_CountOutOfBounds_Throw()
        {
            var artist = artistTable.Add(new Artist() { Name = "Test Artist" });
            var genre = genreTable.Add(new Genre() { Name = "Test Genre" });
            var album = albumTable.Add(new Album() { Name = "Test Album", Artist = artist, Genre = genre, ReleaseYear = 2000 });
            var track = trackTable.Add(new Track() { Title = "Test Track", Artist = "Test Track Artist", Album = album, Genre = genre });
            var driveItem = driveItemTable.Add(new DriveItem() { Id = "TestDriveItemId", Track = track });
            var playlist = playlistTable.Add(new Playlist() { Name = "Test Playlist", LastModified = DateTime.Now });
            var playlistItem = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value });
            var playlistItem2 = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value });

            Assert.Throws<ArgumentOutOfRangeException>(() => playlistItemTable.Reorder(playlist, 0, 1, 3));
        }

        [Fact]
        public void Reorder_TwoItemsFromBottomToTop_ValidateOrder()
        {
            var artist = artistTable.Add(new Artist() { Name = "Test Artist" });
            var genre = genreTable.Add(new Genre() { Name = "Test Genre" });
            var album = albumTable.Add(new Album() { Name = "Test Album", Artist = artist, Genre = genre, ReleaseYear = 2000 });
            var track = trackTable.Add(new Track() { Title = "Test Track", Artist = "Test Track Artist", Album = album, Genre = genre });
            var driveItem = driveItemTable.Add(new DriveItem() { Id = "TestDriveItemId", Track = track });
            var playlist = playlistTable.Add(new Playlist() { Name = "Test Playlist", LastModified = DateTime.Now });
            var playlistItem = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value });
            var playlistItem2 = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value });

            var items = playlistItemTable.Get(new PlaylistItemAccessOptions() { PlaylistFilter = playlist.Id });
            Assert.Collection(items, item1 => Assert.Equal(item1.Id, playlistItem.Id), item2 => Assert.Equal(item2.Id, playlistItem2.Id));

            playlistItemTable.Reorder(playlist, 0, 1, 1);
            var reorderedItems = playlistItemTable.Get(new PlaylistItemAccessOptions() { PlaylistFilter = playlist.Id });
            Assert.Collection(reorderedItems, item1 => Assert.Equal(item1.Id, playlistItem2.Id), item2 => Assert.Equal(item2.Id, playlistItem.Id));
        }

        [Fact]
        public void Reorder_TwoItemsFromTopToBottom_ValidateOrder()
        {
            var artist = artistTable.Add(new Artist() { Name = "Test Artist" });
            var genre = genreTable.Add(new Genre() { Name = "Test Genre" });
            var album = albumTable.Add(new Album() { Name = "Test Album", Artist = artist, Genre = genre, ReleaseYear = 2000 });
            var track = trackTable.Add(new Track() { Title = "Test Track", Artist = "Test Track Artist", Album = album, Genre = genre });
            var driveItem = driveItemTable.Add(new DriveItem() { Id = "TestDriveItemId", Track = track });
            var playlist = playlistTable.Add(new Playlist() { Name = "Test Playlist", LastModified = DateTime.Now });
            var playlistItem = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value });
            var playlistItem2 = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value });

            var items = playlistItemTable.Get(new PlaylistItemAccessOptions() { PlaylistFilter = playlist.Id });
            Assert.Collection(items, item1 => Assert.Equal(item1.Id, playlistItem.Id), item2 => Assert.Equal(item2.Id, playlistItem2.Id));

            playlistItemTable.Reorder(playlist, 1, 0, 1);
            var reorderedItems = playlistItemTable.Get(new PlaylistItemAccessOptions() { PlaylistFilter = playlist.Id });
            Assert.Collection(reorderedItems, item1 => Assert.Equal(item1.Id, playlistItem2.Id), item2 => Assert.Equal(item2.Id, playlistItem.Id));
        }

        [Fact]
        public void Reorder_FourItemsMoveFromSecondIndexToFirstIndex_ValidateOrder()
        {
            var artist = artistTable.Add(new Artist() { Name = "Test Artist" });
            var genre = genreTable.Add(new Genre() { Name = "Test Genre" });
            var album = albumTable.Add(new Album() { Name = "Test Album", Artist = artist, Genre = genre, ReleaseYear = 2000 });
            var track = trackTable.Add(new Track() { Title = "Test Track", Artist = "Test Track Artist", Album = album, Genre = genre });
            var driveItem = driveItemTable.Add(new DriveItem() { Id = "TestDriveItemId", Track = track });
            var playlist = playlistTable.Add(new Playlist() { Name = "Test Playlist", LastModified = DateTime.Now });
            var playlistItem = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value });
            var playlistItem2 = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value });
            var playlistItem3 = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value });
            var playlistItem4 = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value });

            var items = playlistItemTable.Get(new PlaylistItemAccessOptions() { PlaylistFilter = playlist.Id });
            Assert.Collection(items, 
                item1 => Assert.Equal(item1.Id, playlistItem.Id), 
                item2 => Assert.Equal(item2.Id, playlistItem2.Id),
                item3 => Assert.Equal(item3.Id, playlistItem3.Id),
                item4 => Assert.Equal(item4.Id, playlistItem4.Id));

            playlistItemTable.Reorder(playlist, 1, 2, 1);

            var reorderedItems = playlistItemTable.Get(new PlaylistItemAccessOptions() { PlaylistFilter = playlist.Id });
            Assert.Collection(reorderedItems,
                item1 => Assert.Equal(item1.Id, playlistItem.Id),
                item2 => Assert.Equal(item2.Id, playlistItem3.Id),
                item3 => Assert.Equal(item3.Id, playlistItem2.Id),
                item4 => Assert.Equal(item4.Id, playlistItem4.Id));
        }

        [Fact]
        public void Reorder_FourItemsMoveFromFirstIndexToSecondIndex_ValidateOrder()
        {
            var artist = artistTable.Add(new Artist() { Name = "Test Artist" });
            var genre = genreTable.Add(new Genre() { Name = "Test Genre" });
            var album = albumTable.Add(new Album() { Name = "Test Album", Artist = artist, Genre = genre, ReleaseYear = 2000 });
            var track = trackTable.Add(new Track() { Title = "Test Track", Artist = "Test Track Artist", Album = album, Genre = genre });
            var driveItem = driveItemTable.Add(new DriveItem() { Id = "TestDriveItemId", Track = track });
            var playlist = playlistTable.Add(new Playlist() { Name = "Test Playlist", LastModified = DateTime.Now });
            var playlistItem = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value });
            var playlistItem2 = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value });
            var playlistItem3 = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value });
            var playlistItem4 = playlistItemTable.Add(playlist, new PlaylistItem() { DriveItem = driveItem, PlaylistId = playlist.Id.Value });

            var items = playlistItemTable.Get(new PlaylistItemAccessOptions() { PlaylistFilter = playlist.Id });
            Assert.Collection(items,
                item1 => Assert.Equal(item1.Id, playlistItem.Id),
                item2 => Assert.Equal(item2.Id, playlistItem2.Id),
                item3 => Assert.Equal(item3.Id, playlistItem3.Id),
                item4 => Assert.Equal(item4.Id, playlistItem4.Id));

            playlistItemTable.Reorder(playlist, 2, 1, 1);

            var reorderedItems = playlistItemTable.Get(new PlaylistItemAccessOptions() { PlaylistFilter = playlist.Id });
            Assert.Collection(reorderedItems,
                item1 => Assert.Equal(item1.Id, playlistItem.Id),
                item2 => Assert.Equal(item2.Id, playlistItem3.Id),
                item3 => Assert.Equal(item3.Id, playlistItem2.Id),
                item4 => Assert.Equal(item4.Id, playlistItem4.Id));
        }

        [Fact]
        public void Reorder_EmptyPlaylist_NoThrow()
        {
            var artist = artistTable.Add(new Artist() { Name = "Test Artist" });
            var genre = genreTable.Add(new Genre() { Name = "Test Genre" });
            var album = albumTable.Add(new Album() { Name = "Test Album", Artist = artist, Genre = genre, ReleaseYear = 2000 });
            var track = trackTable.Add(new Track() { Title = "Test Track", Artist = "Test Track Artist", Album = album, Genre = genre });
            var driveItem = driveItemTable.Add(new DriveItem() { Id = "TestDriveItemId", Track = track });
            var playlist = playlistTable.Add(new Playlist() { Name = "Test Playlist", LastModified = DateTime.Now });

            var items = playlistItemTable.Get(new PlaylistItemAccessOptions() { PlaylistFilter = playlist.Id });
            Assert.Empty(items);

            playlistItemTable.Reorder(playlist, -1, -1, 0);
        }

        internal class PlaylistItemComparer : IEqualityComparer<PlaylistItem>
        {
            public bool Equals(PlaylistItem x, PlaylistItem y)
            {
                if (x == y) return true;
                CompareAndAssert(x, y);
                return true;
            }

            public int GetHashCode(PlaylistItem obj)
            {
                return obj.GetHashCode();
            }
        }

        internal static void CompareAndAssert(PlaylistItem first, PlaylistItem second)
        {
            Assert.Equal(first.Id, second.Id);
            Assert.Equal(first.Previous, second.Previous);
            Assert.Equal(first.Next, second.Next);
            DriveItemTableTest.CompareAndAssert(first.DriveItem, second.DriveItem);
        }

        public void Dispose()
        {
            connection?.Dispose();
        }
    }
}

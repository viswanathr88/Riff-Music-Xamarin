using Microsoft.Data.Sqlite;
using Riff.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Riff.Data.Test
{
    public sealed class PlaylistTableTest
    {
        private readonly string dbPath = ":memory:";
        private readonly SqliteConnection connection;
        private readonly PlaylistTable playlistTable;

        public PlaylistTableTest()
        {
            connection = new SqliteConnection($"Data Source = {dbPath};foreign keys=true;");
            connection.Open();

            var extractor = new DataExtractor();

            playlistTable = new PlaylistTable(connection, extractor);
            playlistTable.HandleUpgrade(Sqlite.Version.AddPlaylists);
        }

        [Fact]
        public void Add_NonEmptyId_Throw()
        {
            var playlist = new Playlist2() { Id = 1, Name = "TestPlaylist" };
            Assert.Throws<ArgumentException>(() => playlistTable.Add(playlist));
        }

        [Fact]
        public void Add_NullName_Throw()
        {
            var playlist = new Playlist2() { Name = null };
            Assert.Throws<ArgumentException>(() => playlistTable.Add(playlist));
        }

        [Fact]
        public void Add_Success_ValidateId()
        {
            var playlist = new Playlist2() { Name = "TestPlaylist", LastModified = DateTime.Now };
            var p = playlistTable.Add(playlist);
            Assert.NotNull(p);
            Assert.Equal(1, p.Id);
        }

        [Fact]
        public void Add_ExistingPlaylistWithSameName_Throw()
        {
            playlistTable.Add(new Playlist2() { Name = "TestPlaylist" });
            Assert.ThrowsAny<Exception>(() => playlistTable.Add(new Playlist2() { Name = "TestPlaylist" }));
        }

        [Fact]
        public void Get_NonExistentId_ReturnNull()
        {
            var playlist = new Playlist2() { Name = "TestPlaylist" };
            playlistTable.Add(playlist);

            Assert.Null(playlistTable.Get(2));
        }

        [Fact]
        public void Get_ExistingId_ValidateFields()
        {
            var playlist = new Playlist2() { Name = "TestPlaylist", LastModified = DateTime.Now };
            playlistTable.Add(playlist);

            var actualPlaylist = playlistTable.Get(playlist.Id.Value);
            Assert.Equal(playlist, actualPlaylist, new PlaylistEqualityComparer());
        }

        [Fact]
        public void Get_NullOptions_Throw()
        {
            Assert.Throws<ArgumentNullException>(() => playlistTable.Get(null));
        }

        [Fact]
        public void Get_PlaylistFilter_ValidateFields()
        {
            var playlist = new Playlist2() { Name = "TestPlaylist", LastModified = DateTime.Now };
            playlistTable.Add(playlist);

            var options = new PlaylistAccessOptions()
            {
                PlaylistFilter = playlist.Id.Value
            };
            var playlists = playlistTable.Get(options);
            Assert.Single(playlists);
            Assert.Equal(playlist, playlists[0], new PlaylistEqualityComparer());
        }

        [Theory]
        [InlineData(PlaylistSortType.Name, SortOrder.Descending)]
        [InlineData(PlaylistSortType.Name, SortOrder.Ascending)]
        [InlineData(PlaylistSortType.LastModified, SortOrder.Descending)]
        [InlineData(PlaylistSortType.LastModified, SortOrder.Ascending)]

        public void Get_VariousSortType_ValidateOrder(PlaylistSortType type, SortOrder order)
        {
            List<Playlist2> items = new List<Playlist2>();
            items.Add(playlistTable.Add(new Playlist2() { Name = "TestPlaylist1" }));
            items.Add(playlistTable.Add(new Playlist2() { Name = "TestPlaylist4" }));
            items.Add(playlistTable.Add(new Playlist2() { Name = "TestPlaylist2" }));
            items.Add(playlistTable.Add(new Playlist2() { Name = "TestPlaylist7" }));
            items.Add(playlistTable.Add(new Playlist2() { Name = "TestPlaylist5" }));

            if (type == PlaylistSortType.Name)
            {
                items = (order == SortOrder.Descending ? items.OrderByDescending(p => p.Name) : items.OrderBy(p => p.Name)).ToList();
            }
            else if (type == PlaylistSortType.LastModified)
            {
                items = (order == SortOrder.Descending ? items.OrderByDescending(p => p.LastModified) : items.OrderBy(p => p.LastModified)).ToList();
            }

            var options = new PlaylistAccessOptions() { SortType = type, SortOrder = order };
            var actualItems = playlistTable.Get(options);

            Assert.Equal(items, actualItems, new PlaylistEqualityComparer());
        }

        [Fact]
        public void Delete_ExistingId_Success()
        {
            var playlist = new Playlist2() { Name = "TestPlaylist" };
            playlistTable.Add(playlist);
            playlistTable.Delete(playlist.Id.Value);
            Assert.Null(playlistTable.Get(playlist.Id.Value));
        }

        [Fact]
        public void Update_NoId_Throw()
        {
            var playlist = new Playlist2() { Name = "TestPlaylist" };
            playlistTable.Add(playlist);

            playlist.Id = null;
            playlist.Name = "TestPlaylist2";
            Assert.Throws<ArgumentNullException>(() => playlistTable.Update(playlist));
        }

        [Fact]
        public void Update_NullName_Throw()
        {
            var playlist = new Playlist2() { Name = "TestPlaylist" };
            playlistTable.Add(playlist);

            playlist.Name = string.Empty;
            Assert.ThrowsAny<Exception>(() => playlistTable.Update(playlist));

            playlist.Name = null;
            Assert.ThrowsAny<Exception>(() => playlistTable.Update(playlist));
        }

        [Fact]
        public void Update_ValidName_Success()
        {
            var playlist = new Playlist2() { Name = "TestPlaylist" };
            playlistTable.Add(playlist);

            playlist.Name = "TestPlaylist2";
            playlistTable.Update(playlist);

            Assert.Equal(playlist, playlistTable.Get(1), new PlaylistEqualityComparer());
        }

        class PlaylistEqualityComparer : IEqualityComparer<Playlist2>
        {
            public bool Equals(Playlist2 x, Playlist2 y)
            {
                CompareAndAssert(x, y);
                return true;
            }

            public int GetHashCode(Playlist2 obj)
            {
                return obj.GetHashCode();
            }
        }

        internal static void CompareAndAssert(Playlist2 first, Playlist2 second)
        {
            if (first == second) return;
            Assert.Equal(first.Id, second.Id);
            Assert.Equal(first.Name, second.Name);
            Assert.Equal(first.LastModified, second.LastModified);
        }
    }
}

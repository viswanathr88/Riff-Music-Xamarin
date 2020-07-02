using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Riff.Data.Test
{
    public sealed class PlaylistManagerTest : IDisposable
    {
        private readonly PlaylistManager playlistManager;
        private readonly string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Playlists");

        public PlaylistManagerTest()
        {
            playlistManager = new PlaylistManager(path);
        }

        [Fact]
        public void Constructor_Ensure_Directory_Exists()
        {
            Assert.True(Directory.Exists(path));
        }

        [Fact]
        public void CreatePlaylist_NoName_Throw()
        {
            Assert.Throws<ArgumentNullException>(() => playlistManager.CreatePlaylist(string.Empty));
            Assert.Throws<ArgumentNullException>(() => playlistManager.CreatePlaylist(null));
        }

        [Fact]
        public void CreatePlaylist_ValidName_EnsureFileExists()
        {
            string name = "TestPlaylist";
            string fullPath = Path.Combine(path, name);
            Assert.False(File.Exists(fullPath));

            var playlist = playlistManager.CreatePlaylist(name);
            Assert.NotNull(playlist);
            Assert.True(File.Exists(fullPath));
        }

        [Fact]
        public void CreatePlaylist_ExistingName_Throw()
        {
            string name = "TestPlaylist";
            string fullPath = Path.Combine(path, name);
            using (var stream = File.Create(fullPath))
            {
            }

            Assert.ThrowsAny<Exception>(() => playlistManager.CreatePlaylist(name));
        }

        [Fact]
        public void DeletePlaylist_NoName_Throw()
        {
            Assert.Throws<ArgumentNullException>(() => playlistManager.DeletePlaylist(null));
            Assert.Throws<ArgumentNullException>(() => playlistManager.DeletePlaylist(string.Empty));
        }

        [Fact]
        public void DeletePlaylist_ExistingPlaylist_EnsureFileDeleted()
        {
            string name = "TestPlaylist";
            string fullPath = Path.Combine(path, name);
            using (var stream = File.Create(fullPath))
            {
            }

            playlistManager.DeletePlaylist(name);
            Assert.False(File.Exists(fullPath));
        }

        [Fact]
        public void DeletePlaylist_RandomName_NoThrow()
        {
            string name = "blah";
            string fullPath = Path.Combine(path, name);
            Assert.False(File.Exists(fullPath));

            playlistManager.DeletePlaylist(name);
        }

        [Fact]
        public void GetPlaylists_EmptyDirectory_VerifyCollectionEmpty()
        {
            Assert.Empty(playlistManager.GetPlaylists());
        }

        [Fact]
        public void GetPlaylists_FewPlaylists_VerifyCollectionSize()
        {
            for (int i = 0; i < 5; i++)
            {
                playlistManager.CreatePlaylist("TestPlaylist" + i);
            }

            Assert.Equal(5, playlistManager.GetPlaylists().Count);
        }

        [Fact]
        public void GetPlaylists_FewPlaylists_VerifyPlaylistNames()
        {
            HashSet<string> names = new HashSet<string>() { "TestPlaylist1", "TestPlaylist2", "TestPlaylist5", "TestPlaylist10" };
            foreach (var name in names)
            {
                playlistManager.CreatePlaylist(name);
            }

            foreach (var playlist in playlistManager.GetPlaylists())
            {
                Assert.Contains(playlist.Name, names);
            }
        }

        [Fact]
        public void RenamePlaylist_OldOrNewNameEmpty_Throw()
        {
            Assert.Throws<ArgumentNullException>(() => playlistManager.RenamePlaylist(string.Empty, "Test1"));
            Assert.Throws<ArgumentNullException>(() => playlistManager.RenamePlaylist("Test", string.Empty));
            Assert.Throws<ArgumentNullException>(() => playlistManager.RenamePlaylist(null, "Test1"));
            Assert.Throws<ArgumentNullException>(() => playlistManager.RenamePlaylist("Test1", null));
        }

        [Fact]
        public void RenamePlaylist_OldNameDoesntExist_Throw()
        {
            Assert.Throws<UnauthorizedAccessException>(() => playlistManager.RenamePlaylist("Test1", "Test2"));
        }

        [Fact]
        public void RenamePlaylist_NewNameAlreadyExists_Throw()
        {
            playlistManager.CreatePlaylist("Test1");
            playlistManager.CreatePlaylist("Test2");

            Assert.ThrowsAny<Exception>(() => playlistManager.RenamePlaylist("Test1", "Test2"));
        }

        [Fact]
        public void RenamePlaylist_ValidOldAndNewName_EnsureOldFileRemovedAndNewFileAdded()
        {
            string oldName = "TestPlaylist1";
            string newName = "TestPlaylist2";

            playlistManager.CreatePlaylist(oldName);
            Assert.True(File.Exists(Path.Combine(path, oldName)));
            Assert.False(File.Exists(Path.Combine(path, newName)));

            playlistManager.RenamePlaylist(oldName, newName);
            Assert.True(File.Exists(Path.Combine(path, newName)));
            Assert.False(File.Exists(Path.Combine(path, oldName)));
        }

        public void Dispose()
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }
    }
}

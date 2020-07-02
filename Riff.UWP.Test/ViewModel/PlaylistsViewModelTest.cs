using Moq;
using Riff.Data;
using Riff.UWP.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Xunit;

namespace Riff.UWP.Test.ViewModel
{
    public sealed class PlaylistsViewModelTest : IDisposable
    {
        private readonly Mock<IMusicLibrary> mockMusicLibrary;
        private readonly Mock<IPlaylistManager> mockPlaylistManager;
        private readonly Mock<IPlayer> mockPlayer;

        private PlaylistsViewModel playlistsVM;

        public PlaylistsViewModelTest()
        {
            mockMusicLibrary = new Mock<IMusicLibrary>();
            mockPlaylistManager = new Mock<IPlaylistManager>();
            mockMusicLibrary.Setup(library => library.Playlists).Returns(mockPlaylistManager.Object);
            mockPlayer = new Mock<IPlayer>();

            playlistsVM = new PlaylistsViewModel(mockPlayer.Object, mockMusicLibrary.Object);
        }

        [Fact]
        public void Constructor_Successful_VerifyBasicProperties()
        {
            Assert.NotNull(playlistsVM.Add);
            Assert.NotNull(playlistsVM.Delete);
            Assert.NotNull(playlistsVM.Play);
            Assert.NotNull(playlistsVM.PlayNext);
            Assert.NotNull(playlistsVM.Rename);
            Assert.True(string.IsNullOrEmpty(playlistsVM.PlaylistName));
            Assert.Null(playlistsVM.Playlists);
        }

        [Fact]
        public async Task LoadAsync_NoPlaylists_VerifyPlaylistsCollectionEmpty()
        {
            mockPlaylistManager.Setup(manager => manager.GetPlaylists()).Returns(new List<Playlist>());
            await playlistsVM.LoadAsync();
            Assert.Empty(playlistsVM.Playlists);
            Assert.True(playlistsVM.IsEmpty);
        }

        [Fact]
        public async Task LoadAsync_FewPlaylists_EnsurePlaylistCollectionCount()
        {
            IList<Playlist> playlists = new List<Playlist>()
            {
                new Playlist(string.Empty, "TestPlaylist"),
                new Playlist(string.Empty, "TestPlaylist2")
            };
            mockPlaylistManager.Setup(manager => manager.GetPlaylists()).Returns(playlists);
            await playlistsVM.LoadAsync();
            Assert.Collection(playlistsVM.Playlists, (item1) => Assert.Equal("TestPlaylist", item1.Name), (item2) => Assert.Equal("TestPlaylist2", item2.Name));
            Assert.False(playlistsVM.IsEmpty);
        }

        public void Dispose()
        {
        }
    }
}

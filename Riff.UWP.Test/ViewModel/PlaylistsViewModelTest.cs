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
        private readonly Mock<IPlaylistReadOnlyAccessor> mockPlaylistAccessor;
        private readonly Mock<IPlayer> mockPlayer;

        private PlaylistsViewModel playlistsVM;

        public PlaylistsViewModelTest()
        {
            mockMusicLibrary = new Mock<IMusicLibrary>();
            mockPlaylistAccessor = new Mock<IPlaylistReadOnlyAccessor>();
            mockMusicLibrary.Setup(library => library.Playlists2).Returns(mockPlaylistAccessor.Object);
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
            Assert.Empty(playlistsVM.Playlists);
        }

        [Fact]
        public async Task LoadAsync_NoPlaylists_VerifyPlaylistsCollectionEmpty()
        {
            mockPlaylistAccessor.Setup(accessor => accessor.Get(It.IsAny<PlaylistAccessOptions>())).Returns(new List<Playlist2>());
            await playlistsVM.LoadAsync();
            Assert.Empty(playlistsVM.Playlists);
            Assert.True(playlistsVM.IsEmpty);
        }

        [Fact]
        public async Task LoadAsync_FewPlaylists_EnsurePlaylistCollectionCount()
        {
            IList<Playlist2> playlists = new List<Playlist2>()
            {
                new Playlist2() { Id = 1, Name = "TestPlaylist", LastModified = DateTime.Now },
                new Playlist2() { Id = 2, Name = "TestPlaylist2", LastModified = DateTime.Now }
            };
            mockPlaylistAccessor.Setup(accessor => accessor.Get(It.IsAny<PlaylistAccessOptions>())).Returns(playlists);
            await playlistsVM.LoadAsync();
            Assert.Collection(playlistsVM.Playlists, (item1) => Assert.Equal("TestPlaylist", item1.Name), (item2) => Assert.Equal("TestPlaylist2", item2.Name));
            Assert.False(playlistsVM.IsEmpty);
        }

        public void Dispose()
        {
        }
    }
}

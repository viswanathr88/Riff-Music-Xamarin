using Moq;
using Riff.Data;
using Riff.Sync;
using Riff.UWP.ViewModel;
using System;
using Windows.Storage;
using Xunit;

namespace Riff.UWP.Test.ViewModel
{
    public sealed class PlayerViewModelTest : IDisposable
    {
        private readonly Mock<ITrackUrlDownloader> mockTrackUrlDownloader;
        private readonly Mock<IMusicLibrary> mockLibrary;
        private readonly PlayerViewModel playerViewModel;

        public PlayerViewModelTest()
        {
            // Setup mocks
            mockTrackUrlDownloader = new Mock<ITrackUrlDownloader>();
            mockLibrary = new Mock<IMusicLibrary>();
            playerViewModel = new PlayerViewModel(mockLibrary.Object, mockTrackUrlDownloader.Object);
        }

        [Fact]
        public void Constructor_ValidateMediaPlayer()
        {
            Assert.NotNull(playerViewModel.MediaPlayer);
            Assert.Null(playerViewModel.PlaybackList);
        }


        public void Dispose()
        {
        }
    }
}

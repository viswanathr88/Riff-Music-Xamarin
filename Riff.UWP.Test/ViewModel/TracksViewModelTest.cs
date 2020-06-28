using Moq;
using Riff.Data;
using Riff.UWP.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Xunit;

namespace Riff.UWP.Test.ViewModel
{
    public sealed class TracksViewModelTest
    {
        private readonly Mock<IDriveItemReadOnlyAccessor> mockDriveItemsAccessor;
        private readonly Mock<IMusicLibrary> mockLibrary;
        private readonly TracksViewModel tracksVM;

        public TracksViewModelTest()
        {
            mockLibrary = new Mock<IMusicLibrary>();
            mockDriveItemsAccessor = new Mock<IDriveItemReadOnlyAccessor>(MockBehavior.Strict);
            mockLibrary.Setup(data => data.DriveItems).Returns(mockDriveItemsAccessor.Object);
            tracksVM = new TracksViewModel(mockLibrary.Object);
        }

        [Fact]
        public void Constructor_VerifyDefaults()
        {
            Assert.NotNull(tracksVM.Tracks);
            Assert.Equal(TrackSortType.ReleaseYear, tracksVM.SortType);
            Assert.Equal(SortOrder.Descending, tracksVM.SortOrder);
            Assert.False(tracksVM.IsLoading);
            Assert.False(tracksVM.IsLoaded);
            Assert.Null(tracksVM.Parameter);
            Assert.Null(tracksVM.Error);
        }

        [Fact]
        public async Task LoadAsync_ValidateDriveItemAccessOptions()
        {
            mockDriveItemsAccessor.Setup(accessor => accessor.Get(It.Is<DriveItemAccessOptions>(options => options.IncludeTrack == true && options.IncludeTrackAlbum == true && options.IncludeTrackGenre == true)))
                .Returns(new List<DriveItem>());
            await tracksVM.LoadAsync(null);
        }
    }
}

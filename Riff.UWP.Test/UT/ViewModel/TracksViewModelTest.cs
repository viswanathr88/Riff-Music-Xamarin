using GalaSoft.MvvmLight.Ioc;
using Moq;
using Riff.Data;
using Riff.Data.Access;
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
    public sealed class TracksViewModelTest
    {
        private readonly Mock<IMusicMetadata> mockMetadata;
        private readonly Mock<IDriveItemReadOnlyAccessor> mockDriveItemsAccessor;
        private readonly MusicLibrary library;
        private readonly TracksViewModel tracksVM;

        public TracksViewModelTest()
        {
            mockMetadata = new Mock<IMusicMetadata>();
            mockDriveItemsAccessor = new Mock<IDriveItemReadOnlyAccessor>(MockBehavior.Strict);
            mockMetadata.Setup(data => data.DriveItems).Returns(mockDriveItemsAccessor.Object);
            library = new MusicLibrary(ApplicationData.Current.LocalCacheFolder.Path, mockMetadata.Object);
            tracksVM = new TracksViewModel(library);
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

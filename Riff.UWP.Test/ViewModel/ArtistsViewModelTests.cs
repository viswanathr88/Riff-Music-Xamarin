using GalaSoft.MvvmLight.Ioc;
using Mirage.ViewModel;
using Moq;
using Riff.Data;
using Riff.UWP.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Riff.UWP.Test.ViewModel
{
    public sealed class ArtistsViewModelTest : IDisposable
    {
        private readonly Mock<IMusicLibrary> mockLibrary;
        private readonly Mock<IAlbumReadOnlyAccessor> albumAccessor;
        private readonly Mock<IThumbnailCache> thumbnailCache;
        private readonly ArtistsViewModel artistsVM;

        public ArtistsViewModelTest()
        {
            mockLibrary = new Mock<IMusicLibrary>();
            albumAccessor = new Mock<IAlbumReadOnlyAccessor>();
            thumbnailCache = new Mock<IThumbnailCache>();
            mockLibrary.Setup(library => library.Albums).Returns(albumAccessor.Object);
            mockLibrary.Setup(library => library.AlbumArts).Returns(thumbnailCache.Object);
            artistsVM = new ArtistsViewModel(mockLibrary.Object);
        }

        [Fact]
        public void Constructor_NullParameter_Throw()
        {
            Assert.Throws<ArgumentNullException>(() => new ArtistsViewModel(null));
        }

        [Fact]
        public void Constructor_ProperParameter_ValidateInitialState()
        {
            Assert.NotNull(artistsVM.Items);
            Assert.Empty(artistsVM.Items);
            Assert.False(artistsVM.IsCollectionEmpty);
        }

        [Fact]
        public async Task LoadAsync_VerifyAlbumAccessOptions()
        {
            albumAccessor.Setup(accessor => accessor.Get(It.Is<AlbumAccessOptions>(options => options.IncludeArtist == true && options.IncludeGenre == false && options.SortOrder == SortOrder.Descending && options.SortType == AlbumSortType.ReleaseYear))).Returns(new List<Album>());
            await artistsVM.LoadAsync(VoidType.Empty);
        }

        [Fact]
        public async Task LoadAsync_NoData_VerifyIsCollectionEmpty()
        {
            albumAccessor.Setup(accessor => accessor.Get(It.IsAny<AlbumAccessOptions>())).Returns(new List<Album>());
            await artistsVM.LoadAsync(VoidType.Empty);
            Assert.Empty(artistsVM.Items);
            Assert.True(artistsVM.IsCollectionEmpty);
        }

        [Fact]
        public async Task LoadAsync_Some_VerifyFields()
        {
            albumAccessor.Setup(accessor => accessor.Get(It.IsAny<AlbumAccessOptions>())).Returns(new List<Album>() { new Album() { Id = 1, Name = "Test Album", Artist = new Artist() { Id = 1, Name = "Test Artist" } } });
            await artistsVM.LoadAsync(VoidType.Empty);
            Assert.NotEmpty(artistsVM.Items);
            Assert.False(artistsVM.IsCollectionEmpty);
            Assert.Single(artistsVM.Items);
        }

        public void Dispose()
        {
        }
    }
}

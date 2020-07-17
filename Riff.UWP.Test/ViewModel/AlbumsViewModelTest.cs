using Mirage.ViewModel;
using Moq;
using Riff.Data;
using Riff.UWP.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Riff.UWP.Test.ViewModel
{
    public sealed class AlbumsViewModelTest
    {
        private readonly Mock<IMusicLibrary> mockMetadata;
        private readonly Mock<IAlbumReadOnlyAccessor> albumAccessor;
        private readonly AlbumsViewModel albumsVM;

        public AlbumsViewModelTest()
        {
            mockMetadata = new Mock<IMusicLibrary>();
            albumAccessor = new Mock<IAlbumReadOnlyAccessor>();
            mockMetadata.Setup(library => library.Albums).Returns(albumAccessor.Object);
            albumsVM = new AlbumsViewModel(mockMetadata.Object);
        }

        [Fact]
        public void Constructor_NullParameter_Throw()
        {
            Assert.Throws<ArgumentNullException>(() => new AlbumsViewModel(null));
        }

        [Fact]
        public void Constructor_ProperParameter_ValidateInitialState()
        {
            Assert.NotNull(albumsVM.Items);
            Assert.Empty(albumsVM.Items);
            Assert.Equal(AlbumSortType.ReleaseYear, albumsVM.SortType);
            Assert.Equal(SortOrder.Descending, albumsVM.SortOrder);
            Assert.False(albumsVM.IsCollectionEmpty);
        }

        [Fact]
        public async Task LoadAsync_VerifyAlbumAccessOptions()
        {
            albumAccessor.Setup(accessor => accessor.Get(It.Is<AlbumAccessOptions>(options => options.IncludeArtist == true && options.IncludeGenre == false && options.SortOrder == SortOrder.Descending && options.SortType == AlbumSortType.ReleaseYear))).Returns(new List<Album>());
            await albumsVM.LoadAsync(VoidType.Empty);
        }

        [Fact]
        public async Task LoadAsync_NoData_VerifyIsCollectionEmpty()
        {
            albumAccessor.Setup(accessor => accessor.Get(It.IsAny<AlbumAccessOptions>())).Returns(new List<Album>());
            await albumsVM.LoadAsync(VoidType.Empty);
            Assert.Empty(albumsVM.Items);
            Assert.True(albumsVM.IsCollectionEmpty);
        }

        [Fact]
        public async Task LoadAsync_Some_VerifyIsCollectionEmpty()
        {
            albumAccessor.Setup(accessor => accessor.Get(It.IsAny<AlbumAccessOptions>())).Returns(new List<Album>() { new Album() { Id = 1, Name = "Test Album" } });
            await albumsVM.LoadAsync(VoidType.Empty);
            Assert.NotEmpty(albumsVM.Items);
            Assert.False(albumsVM.IsCollectionEmpty);
            Assert.Single(albumsVM.Items);
        }
    }
}

using GalaSoft.MvvmLight.Ioc;
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
        private readonly Mock<IPlayer> mockPlayer;
        private readonly Mock<IAlbumReadOnlyAccessor> albumAccessor;
        private readonly AlbumsViewModel albumsVM;

        public AlbumsViewModelTest()
        {
            mockMetadata = new Mock<IMusicLibrary>();
            albumAccessor = new Mock<IAlbumReadOnlyAccessor>();
            mockPlayer = new Mock<IPlayer>();
            mockMetadata.Setup(library => library.Albums).Returns(albumAccessor.Object);

            SimpleIoc.Default.Register(() => mockMetadata.Object);
            SimpleIoc.Default.Register(() => mockPlayer.Object);
            SimpleIoc.Default.Register<PlaylistsViewModel>();
            SimpleIoc.Default.Register<AlbumsViewModel>();

            albumsVM = SimpleIoc.Default.GetInstance<AlbumsViewModel>();
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

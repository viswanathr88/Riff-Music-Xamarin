using GalaSoft.MvvmLight.Ioc;
using Moq;
using Riff.Data;
using Riff.UWP.Pages;
using Riff.UWP.Test.Infra;
using Riff.UWP.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Xunit;

namespace Riff.UWP.Test.UI
{
    [Collection("UITests")]
    public class AlbumsPageTest : IAsyncLifetime, IDisposable
    {
        private readonly Mock<IMusicLibrary> mockLibrary;
        private readonly Mock<IAlbumReadOnlyAccessor> albumAccessor;
        private readonly Mock<IThumbnailCache> mockThumbnailCache;
        private readonly Mock<IPlayer> mockPlayer;
        

        private readonly UITree view = new UITree();

        public AlbumsPageTest()
        {
            // Setup mock album accessor
            mockLibrary = new Mock<IMusicLibrary>();
            albumAccessor = new Mock<IAlbumReadOnlyAccessor>();
            mockThumbnailCache = new Mock<IThumbnailCache>();
            mockPlayer = new Mock<IPlayer>();
            mockLibrary.Setup(library => library.Albums).Returns(albumAccessor.Object);
            mockLibrary.Setup(library => library.AlbumArts).Returns(mockThumbnailCache.Object);

            SimpleIoc.Default.Register(() => mockLibrary.Object);
            SimpleIoc.Default.Register(() => albumAccessor.Object);
            SimpleIoc.Default.Register(() => mockPlayer.Object);
            SimpleIoc.Default.Register<PlaylistsViewModel>();
            SimpleIoc.Default.Register<AlbumsViewModel>();
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            await view.UnloadAllPages();
        }

        public void Dispose()
        {
            SimpleIoc.Default.Reset();
        }

        [Fact]
        public async Task AlbumsPage_Navigate_ValidateUnknownAlbum()
        {
            // Setup mock album accessor
            var albums = new List<Album>() { new Album() { Id = 1, Name = null, Artist = new Artist() { Id = 1, Name = "MockArtist" } } };
            albumAccessor.Setup(accessor => accessor.Get(It.IsAny<AlbumAccessOptions>())).Returns(albums);

            // Load the page
            Assert.True(await view.LoadPage<AlbumsPage>(null));
            await view.WaitForElementAndCondition<TextBlock>("AlbumName", (textBlock) => textBlock.Text == Strings.Resources.UnknownAlbumText);
        }

        [Fact]
        public async Task AlbumsPage_Navigate_ValidateUnknownArtist()
        {
            // Setup mock album accessor
            var albums = new List<Album>() { new Album() { Id = 1, Name = "MockAlbum", Artist = new Artist() { Id = 1, Name = null } } };
            albumAccessor.Setup(accessor => accessor.Get(It.IsAny<AlbumAccessOptions>())).Returns(albums);

            // Load the page
            await view.LoadPage<AlbumsPage>(null);
            await view.WaitForElementAndCondition<TextBlock>("ArtistName", (textBlock) => textBlock.Text == Strings.Resources.UnknownArtistText);
        }
    }
}

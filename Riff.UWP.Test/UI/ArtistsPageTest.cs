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
    public class ArtistsPageTest : IAsyncLifetime, IDisposable
    {
        private readonly Mock<IMusicLibrary> mockLibrary;
        private readonly Mock<IAlbumReadOnlyAccessor> albumAccessor;
        private readonly Mock<IThumbnailCache> mockThumbnailCache;
        private readonly UITree view = new UITree();

        public ArtistsPageTest()
        {
            // Setup mock album accessor
            mockLibrary = new Mock<IMusicLibrary>();
            albumAccessor = new Mock<IAlbumReadOnlyAccessor>();
            mockThumbnailCache = new Mock<IThumbnailCache>();
            mockLibrary.Setup(library => library.Albums).Returns(albumAccessor.Object);
            mockLibrary.Setup(library => library.AlbumArts).Returns(mockThumbnailCache.Object);

            SimpleIoc.Default.Register(() => mockLibrary.Object);
            SimpleIoc.Default.Register(() => albumAccessor.Object);
            SimpleIoc.Default.Register<ArtistsViewModel>();
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
            SimpleIoc.Default.Unregister<IMusicLibrary>();
            SimpleIoc.Default.Unregister<IAlbumReadOnlyAccessor>();
            SimpleIoc.Default.Unregister<MusicLibrary>();
            SimpleIoc.Default.Unregister<ArtistsViewModel>();
            SimpleIoc.Default.Reset();
        }

        [Fact]
        public async Task ArtistsPage_Navigate_ValidateUnknownArtist()
        {
            // Setup mock album accessor
            var albums = new List<Album>() { new Album() { Id = 1, Name = "MockAlbum", Artist = new Artist() { Id = 1, Name = "" } } };
            albumAccessor.Setup(accessor => accessor.Get(It.IsAny<AlbumAccessOptions>())).Returns(albums);

            await view.LoadPage<ArtistsPage>(null);

            bool success = await view.WaitForElementAndExecute<TextBlock, bool>("ArtistName", (textBlock) =>
            {
                return textBlock.Text == Strings.Resources.UnknownArtistText;
            });

            Assert.True(success);
        }

        [Fact]
        public async Task ArtistsPage_NavigateArtistWithOneAlbum_ValidateAlbumCount()
        {
            // Setup mock album accessor
            var albums = new List<Album>() { new Album() { Id = 1, Name = "MockAlbum", Artist = new Artist() { Id = 1, Name = "" } } };
            albumAccessor.Setup(accessor => accessor.Get(It.IsAny<AlbumAccessOptions>())).Returns(albums);

            // Load page
            await view.LoadPage<ArtistsPage>(null);

            bool success = await view.WaitForElementAndExecute<TextBlock, bool>("AlbumCount", (textBlock) =>
            {
                return textBlock.Text.Contains(albums.Count.ToString());
            });
        }

        [Fact]
        public async Task ArtistsPage_NavigateArtistWithTwoAlbums_ValidateAlbumCount()
        {
            // Setup mock album accessor
            var albums = new List<Album>();
            albums.Add(new Album() { Id = 1, Name = "MockAlbum1", Artist = new Artist() { Id = 1, Name = "MockArtist" } });
            albums.Add(new Album() { Id = 2, Name = "MockAlbum2", Artist = new Artist() { Id = 1, Name = "MockArtist" } });

            albumAccessor.Setup(accessor => accessor.Get(It.IsAny<AlbumAccessOptions>())).Returns(albums);

            // Load page
            await view.LoadPage<ArtistsPage>(null);

            bool success = await view.WaitForElementAndExecute<TextBlock, bool>("AlbumCount", (textBlock) =>
            {
                return textBlock.Text.Contains(albums.Count.ToString());
            });
        }
    }
}

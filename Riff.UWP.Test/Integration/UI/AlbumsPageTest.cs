using GalaSoft.MvvmLight.Ioc;
using Moq;
using Riff.Data;
using Riff.Data.Access;
using Riff.UWP.Pages;
using Riff.UWP.Test.Infra;
using Riff.UWP.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Xunit;

namespace Riff.UWP.Test.UI
{
    [Collection("UITests")]
    public class AlbumsPageTest : IAsyncLifetime, IDisposable
    {
        private readonly Mock<IMusicMetadata> mockMetadata;
        private readonly Mock<IAlbumReadOnlyAccessor> albumAccessor;
        private readonly MusicLibrary library;

        private readonly UITree view = new UITree();

        public AlbumsPageTest()
        {
            // Setup mock album accessor
            mockMetadata = new Mock<IMusicMetadata>();
            albumAccessor = new Mock<IAlbumReadOnlyAccessor>();
            mockMetadata.Setup(metadata => metadata.Albums).Returns(albumAccessor.Object);

            SimpleIoc.Default.Register(() => mockMetadata.Object);
            SimpleIoc.Default.Register(() => albumAccessor.Object);

            library = new MusicLibrary(ApplicationData.Current.LocalCacheFolder.Path, SimpleIoc.Default.GetInstance<IMusicMetadata>());
            SimpleIoc.Default.Register(() => library);
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
            SimpleIoc.Default.Unregister<IMusicMetadata>();
            SimpleIoc.Default.Unregister<IAlbumReadOnlyAccessor>();
            SimpleIoc.Default.Unregister<MusicLibrary>();
            SimpleIoc.Default.Unregister<AlbumsViewModel>();
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

            bool success = await view.WaitForElementAndExecute<TextBlock, bool>("AlbumName", (textBlock) =>
            {
                return textBlock.Text == ResourceLoader.GetForCurrentView().GetString("UnknownAlbumText");
            });

            Assert.True(success);
        }

        [Fact]
        public async Task AlbumsPage_Navigate_ValidateUnknownArtist()
        {
            // Setup mock album accessor
            var albums = new List<Album>() { new Album() { Id = 1, Name = "MockAlbum", Artist = new Artist() { Id = 1, Name = null } } };
            albumAccessor.Setup(accessor => accessor.Get(It.IsAny<AlbumAccessOptions>())).Returns(albums);

            // Load the page
            await view.LoadPage<AlbumsPage>(null);

            bool success = await view.WaitForElementAndExecute<TextBlock, bool>("ArtistName", (textBlock) =>
            {
                return textBlock.Text == ResourceLoader.GetForCurrentView().GetString("UnknownArtistText");
            });

            Assert.True(success);
        }
    }
}

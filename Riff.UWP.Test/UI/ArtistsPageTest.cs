using GalaSoft.MvvmLight.Ioc;
using Moq;
using Riff.Data;
using Riff.Data.Access;
using Riff.UWP.Pages;
using Riff.UWP.Test.Infra;
using Riff.UWP.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Xunit;

namespace Riff.UWP.Test.UI
{
    [Collection("UITests")]
    public class ArtistsPageTest : IAsyncLifetime, IDisposable
    {
        private readonly Mock<IMusicMetadata> mockMetadata;
        private readonly Mock<IAlbumReadOnlyAccessor> albumAccessor;
        private readonly MusicLibrary library;

        private readonly UITree view = new UITree();

        public ArtistsPageTest()
        {
            // Setup mock album accessor
            mockMetadata = new Mock<IMusicMetadata>();
            albumAccessor = new Mock<IAlbumReadOnlyAccessor>();
            mockMetadata.Setup(metadata => metadata.Albums).Returns(albumAccessor.Object);

            SimpleIoc.Default.Register(() => mockMetadata.Object);
            SimpleIoc.Default.Register(() => albumAccessor.Object);

            library = new MusicLibrary(ApplicationData.Current.LocalCacheFolder.Path, SimpleIoc.Default.GetInstance<IMusicMetadata>());
            SimpleIoc.Default.Register(() => library);
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
            SimpleIoc.Default.Unregister<IMusicMetadata>();
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
                return textBlock.Text == ResourceLoader.GetForCurrentView().GetString("UnknownArtistText");
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

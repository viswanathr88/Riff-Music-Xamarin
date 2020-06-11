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
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Xunit;

namespace Riff.UWP.Test.UI
{
    [Collection("UITests")]
    public class AlbumPageTest : IAsyncLifetime, IDisposable
    {
        private readonly Mock<IMusicMetadata> mockMetadata;
        private readonly Mock<IAlbumReadOnlyAccessor> albumAccessor;
        private readonly Mock<ITrackReadOnlyAccessor> trackAccessor;
        private readonly MusicLibrary library;

        private readonly UITree view = new UITree();

        public AlbumPageTest()
        {
            // Setup mock album accessor
            mockMetadata = new Mock<IMusicMetadata>();
            albumAccessor = new Mock<IAlbumReadOnlyAccessor>();
            trackAccessor = new Mock<ITrackReadOnlyAccessor>();
            mockMetadata.Setup(metadata => metadata.Albums).Returns(albumAccessor.Object);
            mockMetadata.Setup(metadata => metadata.Tracks).Returns(trackAccessor.Object);

            SimpleIoc.Default.Register(() => mockMetadata.Object);
            SimpleIoc.Default.Register(() => trackAccessor.Object);

            library = new MusicLibrary(ApplicationData.Current.LocalCacheFolder.Path, SimpleIoc.Default.GetInstance<IMusicMetadata>());
            SimpleIoc.Default.Register(() => library);
            SimpleIoc.Default.Register<AlbumViewModel>();
        }

        public void Dispose()
        {
            SimpleIoc.Default.Unregister<IMusicMetadata>();
            SimpleIoc.Default.Unregister<IAlbumReadOnlyAccessor>();
            SimpleIoc.Default.Unregister<ITrackReadOnlyAccessor>();
            SimpleIoc.Default.Unregister<MusicLibrary>();
            SimpleIoc.Default.Unregister<AlbumViewModel>();
            SimpleIoc.Default.Reset();
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            await view.UnloadAllPages();
        }

        [Fact]
        public async Task AlbumPage_Navigate_ValidateAlbumUnknowns()
        {
            // Setup album mock
            var albums = new List<Album>() { new Album() { Id = 1, Name = null, Artist = new Artist() { Id = 1, Name = null }, Genre = new Genre() { Id = 1, Name = null } } };
            albumAccessor.Setup(accessor => accessor.Get(It.IsAny<AlbumAccessOptions>())).Returns(albums);
            trackAccessor.Setup(accessor => accessor.Get(It.IsAny<TrackAccessOptions>())).Returns(new List<Track>());

            // Load Tracks Page
            await view.LoadPage<AlbumPage>(albums[0]);

            Assert.True(await view.WaitForElementAndExecute<TextBlock, bool>("AlbumName", (textBlock) => ResourceLoader.GetForCurrentView().GetString("UnknownAlbumText") == textBlock.Text));
            Assert.True(await view.WaitForElementAndExecute<TextBlock, bool>("AlbumArtist", (textBlock) => ResourceLoader.GetForCurrentView().GetString("UnknownArtistText") == textBlock.Text));
            Assert.True(await view.WaitForElementAndExecute<TextBlock, bool>("AlbumGenre", (textBlock) => ResourceLoader.GetForCurrentView().GetString("UnknownGenreText") == textBlock.Text));
        }

        [Fact]
        public async Task AlbumPage_Navigate_ValidateAlbumFields()
        {
            // Setup album mock
            var albums = new List<Album>() { new Album() { Id = 1, Name = "MockAlbum", Artist = new Artist() { Id = 1, Name = "MockArtist" }, Genre = new Genre() { Id = 1, Name = "MockGenre" } } };
            albumAccessor.Setup(accessor => accessor.Get(It.IsAny<AlbumAccessOptions>())).Returns(albums);
            trackAccessor.Setup(accessor => accessor.Get(It.IsAny<TrackAccessOptions>())).Returns(new List<Track>());

            // Load Tracks Page
            var album = albums.First();
            await view.LoadPage<AlbumPage>(album);

            Assert.True(await view.WaitForElementAndExecute<TextBlock, bool>("AlbumName", (textBlock) => album.Name == textBlock.Text));
            Assert.True(await view.WaitForElementAndExecute<TextBlock, bool>("AlbumArtist", (textBlock) => album.Artist.Name == textBlock.Text));
            Assert.True(await view.WaitForElementAndExecute<TextBlock, bool>("AlbumGenre", (textBlock) => album.Genre.Name == textBlock.Text));
        }

        [Fact]
        public async Task AlbumPage_Navigate_ValidateTrackUnknowns()
        {
            // Setup album mock
            var albums = new List<Album>() { new Album() { Id = 1, Name = "MockAlbum", Artist = new Artist() { Id = 1, Name = "MockArtist" }, Genre = new Genre() { Id = 1, Name = "MockGenre" } } };
            albumAccessor.Setup(accessor => accessor.Get(It.IsAny<AlbumAccessOptions>())).Returns(albums);

            var tracks = new List<Track>() { new Track() { Id = 1, Title = null, Album = new Album() { Id = 1, Name = null }, Artist = null, Genre = new Genre() { Id = 1, Name = null }, Duration = TimeSpan.FromSeconds(120) } };
            trackAccessor.Setup(accessor => accessor.Get(It.IsAny<TrackAccessOptions>())).Returns(tracks);

            // Load Tracks Page
            var album = albums.First();
            await view.LoadPage<AlbumPage>(album);

            var track = tracks.First();
            Assert.True(await view.WaitForElementAndExecute<TextBlock, bool>("TrackTitle", (textBlock) => ResourceLoader.GetForCurrentView().GetString("UnknownTitleText") == textBlock.Text));
            Assert.True(await view.WaitForElementAndExecute<TextBlock, bool>("TrackArtist", (textBlock) => ResourceLoader.GetForCurrentView().GetString("UnknownArtistText") == textBlock.Text));
        }

        [Fact]
        public async Task AlbumPage_Navigate_ValidateTrackFields()
        {
            // Setup album mock
            var albums = new List<Album>() { new Album() { Id = 1, Name = "MockAlbum", Artist = new Artist() { Id = 1, Name = "MockArtist" }, Genre = new Genre() { Id = 1, Name = "MockGenre" } } };
            albumAccessor.Setup(accessor => accessor.Get(It.IsAny<AlbumAccessOptions>())).Returns(albums);

            var tracks = new List<Track>() { new Track() { Id = 1, Title = "TrackTitle", Album = new Album() { Id = 1, Name = "TrackAlbum" }, Artist = "TrackArtist", Genre = new Genre() { Id = 1, Name = "TrackGenre" }, Duration = TimeSpan.FromSeconds(120) } };
            trackAccessor.Setup(accessor => accessor.Get(It.IsAny<TrackAccessOptions>())).Returns(tracks);

            // Load Tracks Page
            var album = albums.First();
            await view.LoadPage<AlbumPage>(album);

            var track = tracks.First();
            Assert.True(await view.WaitForElementAndExecute<TextBlock, bool>("TrackTitle", (textBlock) => track.Title == textBlock.Text));
            Assert.True(await view.WaitForElementAndExecute<TextBlock, bool>("TrackArtist", (textBlock) => track.Artist == textBlock.Text));
            Assert.True(await view.WaitForElementAndExecute<TextBlock, bool>("TrackDuration", (textBlock) => "02:00" == textBlock.Text));
        }
    }
}

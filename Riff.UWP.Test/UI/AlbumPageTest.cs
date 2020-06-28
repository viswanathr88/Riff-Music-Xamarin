using GalaSoft.MvvmLight.Ioc;
using Moq;
using Riff.Data;
using Riff.Sync;
using Riff.UWP.Pages;
using Riff.UWP.Test.Infra;
using Riff.UWP.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Xunit;

namespace Riff.UWP.Test.UI
{
    [Collection("UITests")]
    public class AlbumPageTest : IAsyncLifetime, IDisposable
    {
        private readonly Mock<IMusicLibrary> mockLibrary;
        private readonly Mock<IAlbumReadOnlyAccessor> albumAccessor;
        private readonly Mock<IDriveItemReadOnlyAccessor> driveItemAccessor;
        private readonly Mock<ITrackUrlDownloader> mockUrlDownloader;
        private readonly Mock<IThumbnailCache> mockThumbnailCache;

        private readonly UITree view = new UITree();

        public AlbumPageTest()
        {
            // Setup mock album accessor
            mockLibrary = new Mock<IMusicLibrary>();
            albumAccessor = new Mock<IAlbumReadOnlyAccessor>();
            driveItemAccessor = new Mock<IDriveItemReadOnlyAccessor>();
            mockUrlDownloader = new Mock<ITrackUrlDownloader>();
            mockThumbnailCache = new Mock<IThumbnailCache>();
            mockLibrary.Setup(library => library.Albums).Returns(albumAccessor.Object);
            mockLibrary.Setup(library => library.DriveItems).Returns(driveItemAccessor.Object);
            mockLibrary.Setup(library => library.AlbumArts).Returns(mockThumbnailCache.Object);

            SimpleIoc.Default.Register(() => mockLibrary.Object);
            SimpleIoc.Default.Register(() => driveItemAccessor.Object);
            SimpleIoc.Default.Register(() => mockUrlDownloader.Object);

            SimpleIoc.Default.Register<AlbumViewModel>();
            SimpleIoc.Default.Register<IPlayer, PlayerViewModel>();
        }

        public void Dispose()
        {
            SimpleIoc.Default.Unregister<IMusicLibrary>();
            SimpleIoc.Default.Unregister<IAlbumReadOnlyAccessor>();
            SimpleIoc.Default.Unregister<IDriveItemReadOnlyAccessor>();
            SimpleIoc.Default.Unregister<ITrackUrlDownloader>();
            SimpleIoc.Default.Unregister<MusicLibrary>();
            SimpleIoc.Default.Unregister<AlbumViewModel>();
            SimpleIoc.Default.Unregister<IPlayer>();
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
            driveItemAccessor.Setup(accessor => accessor.Get(It.IsAny<DriveItemAccessOptions>())).Returns(new List<DriveItem>());

            // Load Tracks Page
            await view.LoadPage<AlbumPage>(albums[0]);

            Assert.True(await view.WaitForElementAndExecute<TextBlock, bool>("AlbumName", (textBlock) => Strings.Resources.UnknownAlbumText == textBlock.Text));
            Assert.True(await view.WaitForElementAndExecute<TextBlock, bool>("AlbumArtist", (textBlock) => Strings.Resources.UnknownArtistText == textBlock.Text));
            Assert.True(await view.WaitForElementAndExecute<TextBlock, bool>("AlbumGenre", (textBlock) => Strings.Resources.UnknownGenreText == textBlock.Text));
        }

        [Fact]
        public async Task AlbumPage_Navigate_ValidateAlbumFields()
        {
            // Setup album mock
            var albums = new List<Album>() { new Album() { Id = 1, Name = "MockAlbum", Artist = new Artist() { Id = 1, Name = "MockArtist" }, Genre = new Genre() { Id = 1, Name = "MockGenre" } } };
            albumAccessor.Setup(accessor => accessor.Get(It.IsAny<AlbumAccessOptions>())).Returns(albums);
            driveItemAccessor.Setup(accessor => accessor.Get(It.IsAny<DriveItemAccessOptions>())).Returns(new List<DriveItem>());

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

            var driveItems = new List<DriveItem>();
            driveItems.Add(new DriveItem()
            {
                Id = "Item1",
                Track = new Track()
                {
                    Id = 1,
                    Title = null,
                    Album = new Album()
                    {
                        Id = 1,
                        Name = null
                    },
                    Artist = null,
                    Genre = new Genre()
                    {
                        Id = 1,
                        Name = null
                    },
                    Duration = TimeSpan.FromSeconds(120)
                }
            });
            driveItemAccessor.Setup(accessor => accessor.Get(It.IsAny<DriveItemAccessOptions>())).Returns(driveItems);

            // Load Tracks Page
            var album = albums.First();
            await view.LoadPage<AlbumPage>(album);

            var track = driveItems.First().Track;
            Assert.True(await view.WaitForElementAndExecute<TextBlock, bool>("TrackTitle", (textBlock) => Strings.Resources.UnknownTitleText == textBlock.Text));
            Assert.True(await view.WaitForElementAndExecute<TextBlock, bool>("TrackArtist", (textBlock) => Strings.Resources.UnknownArtistText == textBlock.Text));
        }

        [Fact]
        public async Task AlbumPage_Navigate_ValidateTrackFields()
        {
            // Setup album mock
            var albums = new List<Album>() { new Album() { Id = 1, Name = "MockAlbum", Artist = new Artist() { Id = 1, Name = "MockArtist" }, Genre = new Genre() { Id = 1, Name = "MockGenre" } } };
            albumAccessor.Setup(accessor => accessor.Get(It.IsAny<AlbumAccessOptions>())).Returns(albums);

            var driveItems = new List<DriveItem>();
            driveItems.Add(new DriveItem()
            {
                Id = "Item1",
                Track = new Track()
                {
                    Id = 1,
                    Title = "TrackTitle",
                    Album = new Album()
                    {
                        Id = 1,
                        Name = "TrackAlbum",
                    },
                    Artist = "TrackArtist",
                    Genre = new Genre()
                    {
                        Id = 1,
                        Name = "TrackGenre"
                    },
                    Duration = TimeSpan.FromSeconds(120)
                }
            });
            driveItemAccessor.Setup(accessor => accessor.Get(It.IsAny<DriveItemAccessOptions>())).Returns(driveItems);

            // Load Tracks Page
            var album = albums.First();
            await view.LoadPage<AlbumPage>(album);

            var track = driveItems.First().Track;
            Assert.True(await view.WaitForElementAndExecute<TextBlock, bool>("TrackTitle", (textBlock) => track.Title == textBlock.Text));
            Assert.True(await view.WaitForElementAndExecute<TextBlock, bool>("TrackArtist", (textBlock) => track.Artist == textBlock.Text));
            Assert.True(await view.WaitForElementAndExecute<TextBlock, bool>("TrackDuration", (textBlock) => "02:00" == textBlock.Text));
        }
    }
}

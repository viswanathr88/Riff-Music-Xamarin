using GalaSoft.MvvmLight.Ioc;
using Moq;
using Riff.Data;
using Riff.Data.Access;
using Riff.Sync;
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
    public class ArtistPageTest : IAsyncLifetime, IDisposable
    {
        private readonly Mock<IMusicMetadata> mockMetadata;
        private readonly Mock<IDriveItemReadOnlyAccessor> driveItemAccessor;
        private readonly Mock<ITrackUrlDownloader> mockUrlDownloader;
        private readonly MusicLibrary library;
        private readonly UITree view = new UITree();

        public ArtistPageTest()
        {
            // Setup mock album accessor
            mockMetadata = new Mock<IMusicMetadata>();
            driveItemAccessor = new Mock<IDriveItemReadOnlyAccessor>();
            mockUrlDownloader = new Mock<ITrackUrlDownloader>();
            mockMetadata.Setup(metadata => metadata.DriveItems).Returns(driveItemAccessor.Object);

            SimpleIoc.Default.Register(() => mockMetadata.Object);
            SimpleIoc.Default.Register(() => driveItemAccessor.Object);
            SimpleIoc.Default.Register(() => mockUrlDownloader.Object);

            library = new MusicLibrary(ApplicationData.Current.LocalCacheFolder.Path, SimpleIoc.Default.GetInstance<IMusicMetadata>());
            SimpleIoc.Default.Register(() => library);
            SimpleIoc.Default.Register<ArtistViewModel>();
            SimpleIoc.Default.Register<IPlayer, PlayerViewModel>();
        }

        public async Task DisposeAsync()
        {
            await view.UnloadAllPages();
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }
        public void Dispose()
        {
            SimpleIoc.Default.Unregister<IMusicMetadata>();
            SimpleIoc.Default.Unregister<IDriveItemReadOnlyAccessor>();
            SimpleIoc.Default.Unregister<ITrackUrlDownloader>();
            SimpleIoc.Default.Unregister<MusicLibrary>();
            SimpleIoc.Default.Unregister<TracksViewModel>();
            SimpleIoc.Default.Unregister<IPlayer>();
            SimpleIoc.Default.Reset();
        }

        [Fact]
        public async Task ArtistPage_Navigate_ValidateUnknowns()
        {
            var items = new List<DriveItem>
            {
                new DriveItem()
                {
                    Id = "TestId",
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
                        Duration = TimeSpan.FromSeconds(100),
                        ReleaseYear = 1999
                    }
                }
            };
            driveItemAccessor.Setup(accessor => accessor.Get(It.IsAny<DriveItemAccessOptions>())).Returns(items);

            // Load Tracks Page
            var artist = new Artist() { Id = 1, Name = null };
            await view.LoadPage<ArtistPage>(artist);

            await view.WaitForElementAndExecute<TextBlock>("ArtistName", (textBlock) => Assert.Equal(Strings.Resources.UnknownArtistText, textBlock.Text));
            await view.WaitForElementAndExecute<TextBlock>("AlbumName", (textBlock) => Assert.Equal(Strings.Resources.UnknownAlbumText, textBlock.Text));
            await view.WaitForElementAndExecute<TextBlock>("TrackTitle", (textBlock) => Assert.Equal(Strings.Resources.UnknownTitleText, textBlock.Text));
            await view.WaitForElementAndExecute<TextBlock>("TrackArtist", (textBlock) => Assert.Equal(Strings.Resources.UnknownArtistText, textBlock.Text));
        }

        [Fact]
        public async Task ArtistPage_Navigate_ValidateFields()
        {
            var items = new List<DriveItem>
            {
                new DriveItem()
                {
                    Id = "TestId",
                    Track = new Track()
                    {
                        Id = 1,
                        Title = "MockTitle",
                        Album = new Album()
                        {
                            Id = 1,
                            Name = "MockAlbum"
                        },
                        Artist = "MockArtist",
                        Duration = TimeSpan.FromSeconds(100),
                        ReleaseYear = 1999
                    }
                }
            };
            driveItemAccessor.Setup(accessor => accessor.Get(It.IsAny<DriveItemAccessOptions>())).Returns(items);

            // Load Tracks Page
            var artist = new Artist() { Id = 1, Name = "MockArtist" };
            await view.LoadPage<ArtistPage>(artist);

            var track = items.First().Track;
            await view.WaitForElementAndExecute<TextBlock>("ArtistName", (textBlock) => Assert.Equal(artist.Name, textBlock.Text));
            await view.WaitForElementAndExecute<TextBlock>("AlbumName", (textBlock) => Assert.Equal(track.Album.Name, textBlock.Text));
            await view.WaitForElementAndExecute<TextBlock>("TrackTitle", (textBlock) => Assert.Equal(track.Title, textBlock.Text));
            await view.WaitForElementAndExecute<TextBlock>("TrackArtist", (textBlock) => Assert.Equal(track.Artist, textBlock.Text));
        }
    }
}

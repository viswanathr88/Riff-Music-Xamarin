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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Xunit;

namespace Riff.UWP.Test.UI
{
    [Collection("UITests")]
    public class TracksPageTest : IAsyncLifetime, IDisposable
    {
        private readonly Mock<IMusicMetadata> mockMetadata;
        private readonly Mock<IDriveItemReadOnlyAccessor> driveItemAccessor;
        private readonly MusicLibrary library;

        private readonly UITree view = new UITree();

        public TracksPageTest()
        {
            // Setup mock album accessor
            mockMetadata = new Mock<IMusicMetadata>();
            driveItemAccessor = new Mock<IDriveItemReadOnlyAccessor>();
            mockMetadata.Setup(metadata => metadata.DriveItems).Returns(driveItemAccessor.Object);

            SimpleIoc.Default.Register(() => mockMetadata.Object);
            SimpleIoc.Default.Register(() => driveItemAccessor.Object);

            library = new MusicLibrary(ApplicationData.Current.LocalCacheFolder.Path, SimpleIoc.Default.GetInstance<IMusicMetadata>());
            SimpleIoc.Default.Register(() => library);
            SimpleIoc.Default.Register<TracksViewModel>();
        }

        public void Dispose()
        {
            SimpleIoc.Default.Unregister<IMusicMetadata>();
            SimpleIoc.Default.Unregister<IDriveItemReadOnlyAccessor>();
            SimpleIoc.Default.Unregister<MusicLibrary>();
            SimpleIoc.Default.Unregister<TracksViewModel>();
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
        public async Task TracksPage_Navigate_ValidateUnknownFields()
        {
            var driveItems = new List<DriveItem>()
            {
                new DriveItem()
                {
                    Id = "TestId",
                    Track = new Track()
                    {
                        Id = 1,
                        Title = null,
                        Album = new Album() { Id = 1, Name = null },
                        Artist = null,
                        Duration = TimeSpan.FromSeconds(100),
                        Genre = new Genre() { Id = 1, Name = null },
                        ReleaseYear = 1999
                    }
                }
            };
            driveItemAccessor.Setup(accessor => accessor.Get(It.IsAny<DriveItemAccessOptions>())).Returns(driveItems);

            // Load Tracks Page
            await view.LoadPage<TracksPage>(null);

            // Validate Unknown Title
            await view.WaitForElementAndCondition<TextBlock>("TrackTitle", (textBlock) => ResourceLoader.GetForCurrentView().GetString("UnknownTitleText") == textBlock.Text);
            await view.WaitForElementAndCondition<TextBlock>("TrackArtist", (textBlock) => ResourceLoader.GetForCurrentView().GetString("UnknownArtistText") == textBlock.Text);
            await view.WaitForElementAndCondition<TextBlock>("TrackAlbum", (textBlock) => ResourceLoader.GetForCurrentView().GetString("UnknownAlbumText") == textBlock.Text);
            await view.WaitForElementAndCondition<TextBlock>("TrackDuration", (textBlock) => "01:40" == textBlock.Text);
            if ((await view.GetWindowSize()).Width > 1008)
            {
                await view.WaitForElementAndCondition<TextBlock>("TrackGenre", (textBlock) => ResourceLoader.GetForCurrentView().GetString("UnknownGenreText") == textBlock.Text);
            }
        }

        [Fact]
        public async Task TracksPage_Navigate_ValidateAllFields()
        {
            // Setup mock track accessor gets
            var driveItems = new List<DriveItem>()
            {
                new DriveItem()
                {
                    Id = "TestId",
                    Track = new Track()
                    {
                        Id = 1,
                        Title = "MockTitle",
                        Album = new Album() { Id = 1, Name = "MockArtist" },
                        Genre = new Genre() { Id = 1, Name = "MockGenre" },
                        Artist = "MockArtist",
                        Duration = TimeSpan.FromSeconds(100),
                        ReleaseYear = 1999
                    }
                }
            };
            driveItemAccessor.Setup(accessor => accessor.Get(It.IsAny<DriveItemAccessOptions>())).Returns(driveItems);

            // Load Tracks Page
            await view.LoadPage<TracksPage>(null);

            // Validate Unknown Genre
            var track = driveItems.First().Track;
            await view.WaitForElementAndCondition<TextBlock>("TrackTitle", (textBlock) => textBlock.Text == track.Title);
            await view.WaitForElementAndCondition<TextBlock>("TrackArtist", (textBlock) => textBlock.Text == track.Artist);
            await view.WaitForElementAndCondition<TextBlock>("TrackAlbum", (textBlock) => textBlock.Text == track.Album.Name);

            if ((await view.GetWindowSize()).Width > 1008)
            {
                await view.WaitForElementAndCondition<TextBlock>("TrackGenre", (textBlock) => textBlock.Text == track.Genre.Name);
            }
        }
    }
}

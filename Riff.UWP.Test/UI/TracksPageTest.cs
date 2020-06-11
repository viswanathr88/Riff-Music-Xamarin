using GalaSoft.MvvmLight.Ioc;
using Moq;
using Riff.Data;
using Riff.Data.Access;
using Riff.UWP.Pages;
using Riff.UWP.Test.Infra;
using Riff.UWP.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Xunit;

namespace Riff.UWP.Test.UI
{
    [Collection("UITests")]
    public class TracksPageTest : IAsyncLifetime, IDisposable
    {
        private readonly Mock<IMusicMetadata> mockMetadata;
        private readonly Mock<ITrackReadOnlyAccessor> trackAccessor;
        private readonly MusicLibrary library;

        private readonly UITree view = new UITree();

        public TracksPageTest()
        {
            // Setup mock album accessor
            mockMetadata = new Mock<IMusicMetadata>();
            trackAccessor = new Mock<ITrackReadOnlyAccessor>();
            mockMetadata.Setup(metadata => metadata.Tracks).Returns(trackAccessor.Object);

            SimpleIoc.Default.Register(() => mockMetadata.Object);
            SimpleIoc.Default.Register(() => trackAccessor.Object);

            library = new MusicLibrary(ApplicationData.Current.LocalCacheFolder.Path, SimpleIoc.Default.GetInstance<IMusicMetadata>());
            SimpleIoc.Default.Register(() => library);
            SimpleIoc.Default.Register<TracksViewModel>();
        }

        public void Dispose()
        {
            SimpleIoc.Default.Unregister<IMusicMetadata>();
            SimpleIoc.Default.Unregister<ITrackReadOnlyAccessor>();
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
        public async Task TracksPage_Navigate_ValidateUnknownTitle()
        {
            // Setup mock track accessor gets
            var tracks = new List<Track>();
            tracks.Add(new Track() { Id = 1, Title = null, Album = new Album() { Id = 1, Name = "MockAlbum" }, Artist = "MockArtist", Duration = TimeSpan.FromSeconds(100), ReleaseYear = 1999 });
            trackAccessor.Setup(accessor => accessor.Get(It.IsAny<TrackAccessOptions>())).Returns(tracks);

            // Load Tracks Page
            await view.LoadPage<TracksPage>(null);

            // Validate Unknown Title
            bool success = await view.WaitForElementAndExecute<TextBlock, bool>("TrackTitle", (textBlock) =>
            {
                return textBlock.Text == ResourceLoader.GetForCurrentView().GetString("UnknownTitleText");
            });

            Assert.True(success);
        }

        [Fact]
        public async Task TracksPage_Navigate_ValidateUnknownArtist()
        {
            // Setup mock track accessor gets
            var tracks = new List<Track>();
            tracks.Add(new Track() { Id = 1, Title = "MockTitle", Album = new Album() { Id = 1, Name = "MockAlbum" }, Artist = null, Duration = TimeSpan.FromSeconds(100), ReleaseYear = 1999 });
            trackAccessor.Setup(accessor => accessor.Get(It.IsAny<TrackAccessOptions>())).Returns(tracks);

            // Load Tracks Page
            await view.LoadPage<TracksPage>(null);

            // Validate Unknown Artist
            bool success = await view.WaitForElementAndExecute<TextBlock, bool>("TrackArtist", (textBlock) =>
            {
                return textBlock.Text == ResourceLoader.GetForCurrentView().GetString("UnknownArtistText");
            });

            Assert.True(success);
        }

        [Fact]
        public async Task TracksPage_Navigate_ValidateUnknownAlbum()
        {
            // Setup mock track accessor gets
            var tracks = new List<Track>();
            tracks.Add(new Track() { Id = 1, Title = "MockTitle", Album = new Album() { Id = 1, Name = null }, Artist = "MockArtist", Duration = TimeSpan.FromSeconds(100), ReleaseYear = 1999 });
            trackAccessor.Setup(accessor => accessor.Get(It.IsAny<TrackAccessOptions>())).Returns(tracks);

            // Load Tracks Page
            await view.LoadPage<TracksPage>(null);

            // Validate Unknown Album
            bool success = await view.WaitForElementAndExecute<TextBlock, bool>("TrackAlbum", (textBlock) =>
            {
                return textBlock.Text == ResourceLoader.GetForCurrentView().GetString("UnknownAlbumText");
            });

            Assert.True(success);
        }

        [Fact]
        public async Task TracksPage_Navigate_ValidateUnknownGenre()
        {
            // Setup mock track accessor gets
            var tracks = new List<Track>();
            tracks.Add(new Track() { Id = 1, Title = "MockTitle", Album = new Album() { Id = 1, Name = "Mock Artist" }, Genre = new Genre() { Id = 1, Name = null },  Artist = "MockArtist", Duration = TimeSpan.FromSeconds(100), ReleaseYear = 1999 });
            trackAccessor.Setup(accessor => accessor.Get(It.IsAny<TrackAccessOptions>())).Returns(tracks);

            // Load Tracks Page
            await view.LoadPage<TracksPage>(null);

            // Validate Unknown Genre
            bool success = await view.WaitForElementAndExecute<TextBlock, bool>("TrackGenre", (textBlock) =>
            {
                return textBlock.Text == ResourceLoader.GetForCurrentView().GetString("UnknownGenreText");
            });

            Assert.True(success);
        }

        [Fact]
        public async Task TracksPage_Navigate_ValidateDurationFormat()
        {
            // Setup mock track accessor gets
            var tracks = new List<Track>();
            tracks.Add(new Track() { Id = 1, Title = "MockTitle", Album = new Album() { Id = 1, Name = "Mock Artist" }, Genre = new Genre() { Id = 1, Name = "MockGenre" }, Artist = "MockArtist", Duration = TimeSpan.FromSeconds(100), ReleaseYear = 1999 });
            trackAccessor.Setup(accessor => accessor.Get(It.IsAny<TrackAccessOptions>())).Returns(tracks);

            // Load Tracks Page
            await view.LoadPage<TracksPage>(null);

            // Validate Unknown Genre
            bool success = await view.WaitForElementAndExecute<TextBlock, bool>("TrackDuration", (textBlock) =>
            {
                return textBlock.Text == "01:40";
            });

            Assert.True(success);
        }

        [Fact]
        public async Task TracksPage_Navigate_ValidateAllFields()
        {
            // Setup mock track accessor gets
            var tracks = new List<Track>();
            tracks.Add(new Track() { Id = 1, Title = "MockTitle", Album = new Album() { Id = 1, Name = "MockArtist" }, Genre = new Genre() { Id = 1, Name = "MockGenre" }, Artist = "MockArtist", Duration = TimeSpan.FromSeconds(100), ReleaseYear = 1999 });
            trackAccessor.Setup(accessor => accessor.Get(It.IsAny<TrackAccessOptions>())).Returns(tracks);

            // Load Tracks Page
            await view.LoadPage<TracksPage>(null);

            // Validate Unknown Genre
            var track = tracks[0];
            Assert.True(await view.WaitForElementAndExecute<TextBlock, bool>("TrackTitle", (textBlock) => textBlock.Text == track.Title));
            Assert.True(await view.WaitForElementAndExecute<TextBlock, bool>("TrackArtist", (textBlock) => textBlock.Text == track.Artist));
            Assert.True(await view.WaitForElementAndExecute<TextBlock, bool>("TrackAlbum", (textBlock) => textBlock.Text == track.Album.Name));
            Assert.True(await view.WaitForElementAndExecute<TextBlock, bool>("TrackGenre", (textBlock) => textBlock.Text == track.Genre.Name));
        }
    }
}

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
using Windows.UI.Xaml;
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
        public async Task TracksPage_Navigate_ValidateUnknownFields()
        {
            // Setup mock track accessor gets
            var tracks = new List<Track>();
            var track = new Track() 
            { 
                Id = 1, 
                Title = null, 
                Album = new Album() { Id = 1, Name = null }, 
                Artist = null, 
                Duration = TimeSpan.FromSeconds(100), 
                Genre = new Genre() { Id = 1, Name = null }, 
                ReleaseYear = 1999 
            };
            tracks.Add(track);
            trackAccessor.Setup(accessor => accessor.Get(It.IsAny<TrackAccessOptions>())).Returns(tracks);

            // Load Tracks Page
            await view.LoadPage<TracksPage>(null);

            // Validate Unknown Title
            await view.WaitForElementAndExecute<TextBlock>("TrackTitle", (textBlock) => Assert.Equal(ResourceLoader.GetForCurrentView().GetString("UnknownTitleText"), textBlock.Text));
            await view.WaitForElementAndExecute<TextBlock>("TrackArtist", (textBlock) => Assert.Equal(ResourceLoader.GetForCurrentView().GetString("UnknownArtistText"), textBlock.Text));
            await view.WaitForElementAndExecute<TextBlock>("TrackAlbum", (textBlock) => Assert.Equal(ResourceLoader.GetForCurrentView().GetString("UnknownAlbumText"), textBlock.Text));
            await view.WaitForElementAndExecute<TextBlock>("TrackDuration", (textBlock) => Assert.Equal("01:40", textBlock.Text));
            if ((await view.GetWindowSize()).Width > 642)
            {
                await view.WaitForElementAndExecute<TextBlock>("TrackGenre", (textBlock) => Assert.Equal(ResourceLoader.GetForCurrentView().GetString("UnknownGenreText"), textBlock.Text));
            }
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
            await view.WaitForElementAndExecute<TextBlock>("TrackTitle", (textBlock) => Assert.Equal(textBlock.Text, track.Title));
            await view.WaitForElementAndExecute<TextBlock>("TrackArtist", (textBlock) => Assert.Equal(textBlock.Text, track.Artist));
            await view.WaitForElementAndExecute<TextBlock>("TrackAlbum", (textBlock) => Assert.Equal(textBlock.Text, track.Album.Name));

            if ((await view.GetWindowSize()).Width > 641)
            {
                Assert.True(await view.WaitForElementAndExecute<TextBlock, bool>("TrackGenre", (textBlock) => textBlock.Text == track.Genre.Name));
            }
        }
    }
}

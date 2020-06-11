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
    public class ArtistPageTest : IAsyncLifetime, IDisposable
    {
        private readonly Mock<IMusicMetadata> mockMetadata;
        private readonly Mock<ITrackReadOnlyAccessor> trackAccessor;
        private readonly MusicLibrary library;
        private readonly UITree view = new UITree();

        public ArtistPageTest()
        {
            // Setup mock album accessor
            mockMetadata = new Mock<IMusicMetadata>();
            trackAccessor = new Mock<ITrackReadOnlyAccessor>();
            mockMetadata.Setup(metadata => metadata.Tracks).Returns(trackAccessor.Object);

            SimpleIoc.Default.Register(() => mockMetadata.Object);
            SimpleIoc.Default.Register(() => trackAccessor.Object);

            library = new MusicLibrary(ApplicationData.Current.LocalCacheFolder.Path, SimpleIoc.Default.GetInstance<IMusicMetadata>());
            SimpleIoc.Default.Register(() => library);
            SimpleIoc.Default.Register<ArtistViewModel>();
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
            SimpleIoc.Default.Unregister<ITrackReadOnlyAccessor>();
            SimpleIoc.Default.Unregister<MusicLibrary>();
            SimpleIoc.Default.Unregister<TracksViewModel>();
            SimpleIoc.Default.Reset();
        }

        [Fact]
        public async Task ArtistPage_Navigate_ValidateUnknowns()
        {
            // Setup mock track accessor gets
            var tracks = new List<Track>();
            tracks.Add(new Track() { Id = 1, Title = null, Album = new Album() { Id = 1, Name = null }, Artist = null, Duration = TimeSpan.FromSeconds(100), ReleaseYear = 1999 });
            trackAccessor.Setup(accessor => accessor.Get(It.IsAny<TrackAccessOptions>())).Returns(tracks);

            // Load Tracks Page
            var artist = new Artist() { Id = 1, Name = null };
            await view.LoadPage<ArtistPage>(artist);

            await view.WaitForElementAndExecute<TextBlock>("ArtistName", (textBlock) => Assert.Equal(ResourceLoader.GetForCurrentView().GetString("UnknownArtistText"), textBlock.Text));
            await view.WaitForElementAndExecute<TextBlock>("AlbumName", (textBlock) => Assert.Equal(ResourceLoader.GetForCurrentView().GetString("UnknownAlbumText"), textBlock.Text));
            await view.WaitForElementAndExecute<TextBlock>("TrackTitle", (textBlock) => Assert.Equal(ResourceLoader.GetForCurrentView().GetString("UnknownTitleText"), textBlock.Text));
            await view.WaitForElementAndExecute<TextBlock>("TrackArtist", (textBlock) => Assert.Equal(ResourceLoader.GetForCurrentView().GetString("UnknownArtistText"), textBlock.Text));
        }

        [Fact]
        public async Task ArtistPage_Navigate_ValidateFields()
        {
            // Setup mock track accessor gets
            var tracks = new List<Track>();
            tracks.Add(new Track() { Id = 1, Title = "MockTitle", Album = new Album() { Id = 1, Name = "MockAlbum" }, Artist = "MockArtist", Duration = TimeSpan.FromSeconds(100), ReleaseYear = 1999 });
            trackAccessor.Setup(accessor => accessor.Get(It.IsAny<TrackAccessOptions>())).Returns(tracks);

            // Load Tracks Page
            var artist = new Artist() { Id = 1, Name = "MockArtist" };
            await view.LoadPage<ArtistPage>(artist);

            var track = tracks.First();
            await view.WaitForElementAndExecute<TextBlock>("ArtistName", (textBlock) => Assert.Equal(artist.Name, textBlock.Text));
            await view.WaitForElementAndExecute<TextBlock>("AlbumName", (textBlock) => Assert.Equal(track.Album.Name, textBlock.Text));
            await view.WaitForElementAndExecute<TextBlock>("TrackTitle", (textBlock) => Assert.Equal(track.Title, textBlock.Text));
            await view.WaitForElementAndExecute<TextBlock>("TrackArtist", (textBlock) => Assert.Equal(track.Artist, textBlock.Text));
        }
    }
}

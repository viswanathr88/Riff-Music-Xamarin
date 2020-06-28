using GalaSoft.MvvmLight.Ioc;
using Moq;
using Riff.Data;
using Riff.Sync;
using Riff.UWP.Controls;
using Riff.UWP.Test.Infra;
using Riff.UWP.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Xunit;

namespace Riff.UWP.Test.UI
{
    [Collection("UITests")]
    public class TrackListTest : IAsyncLifetime, IDisposable
    {
        private readonly Mock<IMusicLibrary> mockLibrary;
        private readonly Mock<ITrackUrlDownloader> mockUrlDownloader;
        private readonly Mock<IMediaList> mockMediaList;
        private readonly Mock<IPlayer> mockPlayer;

        private readonly UITree view = new UITree();

        private IList<DriveItem> tracks = new List<DriveItem>
        {
            new DriveItem()
            {
                Id = "TestId",
                Track = new Track()
                {
                    Id = 1,
                    Title = "TestTitle",
                    Artist = "TestArtist",
                    Duration = TimeSpan.FromSeconds(100),
                    Album = new Album() { Id = 1, Name = "TestAlbum" },
                    Genre = new Genre() { Id = 2, Name = "TestGenre" },
                    Number = 3
                }
            }
        };

        public TrackListTest()
        {
            mockLibrary = new Mock<IMusicLibrary>();
            mockUrlDownloader = new Mock<ITrackUrlDownloader>();
            mockMediaList = new Mock<IMediaList>();
            mockPlayer = new Mock<IPlayer>();
            
            SimpleIoc.Default.Register(() => mockLibrary.Object);
            SimpleIoc.Default.Register(() => mockUrlDownloader.Object);
            SimpleIoc.Default.Register(() => mockPlayer.Object);
        }

        [Theory]
        [InlineData(375, 600)]
        [InlineData(640, 600)]
        [InlineData(700, 600)]
        [InlineData(800, 600)]
        [InlineData(900, 600)]
        public async Task DisableAllOptionalFields_ValidateAdaptiveBehavior(double width, double height)
        {
            // Load a blank page
            await view.LoadPage<BlankPage>(null);

            await view.WaitForElementAndExecute<Grid>("BlankPageContainer", (grid) =>
            {

                TrackList trackList = new TrackList()
                {
                    EnableAlbum = false,
                    EnableArt = false,
                    EnableGenre = false,
                    EnableReleaseYear = false,
                    EnableTrackNumbers = false,
                    Items = tracks
                };

                grid.Children.Add(trackList);
            });

            Assert.True(await view.ResizeWindow(new Size(width, height)));

            await view.WaitForElementAndCondition<TextBlock>("TrackTitle", (textBlock) => textBlock.Visibility == Visibility.Visible && textBlock.Text == tracks[0].Track.Title);
            await view.WaitForElementAndCondition<TextBlock>("TrackArtist", (textBlock) => textBlock.Visibility == Visibility.Visible && textBlock.Text == tracks[0].Track.Artist);
            await view.WaitForElementAndCondition<TextBlock>("TrackDuration", (textBlock) => textBlock.Visibility == Visibility.Visible && textBlock.Text == "01:40");

            // Verify the following elements are hidden
            await view.WaitForElementAndCondition<Border>("TrackArtContainer", (border) => border.Visibility == Visibility.Collapsed);
            await view.WaitForElementAndCondition<TextBlock>("TrackNumber", (textBlock) => textBlock.Visibility == Visibility.Collapsed);
            await view.WaitForElementAndCondition<TextBlock>("TrackAlbum", (textBlock) => textBlock.Visibility == Visibility.Collapsed);
            await view.WaitForElementAndCondition<TextBlock>("TrackGenre", (textBlock) => textBlock.Visibility == Visibility.Collapsed);
            await view.WaitForElementAndCondition<TextBlock>("TrackReleaseYear", (textBlock) => textBlock.Visibility == Visibility.Collapsed);
        }

        [Theory]
        [InlineData(375, 600)]
        [InlineData(640, 600)]
        [InlineData(700, 600)]
        [InlineData(800, 600)]
        [InlineData(900, 600)]
        public async Task EnableAllOptionalFields_ValidateAdaptiveBehavior(double width, double height)
        {
            var size = new Size(width, height);

            // Load a blank page
            await view.LoadPage<BlankPage>(null);

            await view.WaitForElementAndExecute<Grid>("BlankPageContainer", (grid) =>
            {

                TrackList trackList = new TrackList()
                {
                    EnableAlbum = true,
                    EnableGenre = true,
                    EnableReleaseYear = true,
                    EnableTrackNumbers = true,
                    Items = tracks
                };

                grid.Children.Add(trackList);
            });

            Assert.True(await view.ResizeWindow(size));

            await view.WaitForElementAndCondition<TextBlock>("TrackNumber", (textBlock) => textBlock.Visibility == Visibility.Visible && textBlock.Text == tracks[0].Track.Number.ToString());
            await view.WaitForElementAndCondition<TextBlock>("TrackTitle", (textBlock) => textBlock.Visibility == Visibility.Visible && textBlock.Text == tracks[0].Track.Title);
            await view.WaitForElementAndCondition<TextBlock>("TrackArtist", (textBlock) => textBlock.Visibility == Visibility.Visible && textBlock.Text == tracks[0].Track.Artist);
            await view.WaitForElementAndCondition<TextBlock>("TrackDuration", (textBlock) => textBlock.Visibility == Visibility.Visible && textBlock.Text == "01:40");

            // Verify the following elements are shown based on window size
            if (size.Width > 740)
            {
                await view.WaitForElementAndCondition<TextBlock>("TrackAlbum", (textBlock) => textBlock.Visibility == Visibility.Visible && textBlock.Text == tracks[0].Track.Album.Name);
            }
            else
            {
                await view.WaitForElementAndCondition<TextBlock>("TrackAlbum", (textBlock) => textBlock.Visibility == Visibility.Collapsed);
            }

            if (size.Width > 870)
            {
                await view.WaitForElementAndCondition<TextBlock>("TrackGenre", (textBlock) => textBlock.Visibility == Visibility.Visible && textBlock.Text == tracks[0].Track.Genre.Name);
            }
            else
            {
                await view.WaitForElementAndCondition<TextBlock>("TrackGenre", (textBlock) => textBlock.Visibility == Visibility.Collapsed);
            }

            if (size.Width > 1008)
            {
                await view.WaitForElementAndCondition<TextBlock>("TrackReleaseYear", (textBlock) => textBlock.Visibility == Visibility.Visible && textBlock.Text == tracks[0].Track.ReleaseYear.ToString());
            }
            else
            {
                await view.WaitForElementAndCondition<TextBlock>("TrackReleaseYear", (textBlock) => textBlock.Visibility == Visibility.Collapsed);
            }
        }

        [Fact]
        public async Task LoadTrackList_NotPlayingCurrently_VerifyTrackNotSelected()
        {
            // Load a blank page
            await view.LoadPage<BlankPage>(null);

            await view.WaitForElementAndExecute<Grid>("BlankPageContainer", (grid) =>
            {

                TrackList trackList = new TrackList()
                {
                    EnableAlbum = true,
                    EnableGenre = true,
                    EnableReleaseYear = true,
                    EnableTrackNumbers = true,
                    Items = tracks,
                    PlayableTracks = tracks
                };

                grid.Children.Add(trackList);
            });

            // Ensure when track list loads without a song playing, no item in the list is selected
            await view.WaitForElementAndCondition<ListView>("TrackListView", (listView) => listView.SelectedIndex == -1);
        }

        [Fact]
        public async Task LoadTrackList_TrackCurrentlyPlaying_VerifyTrackSelected()
        {
            // Setup mocks
            var item = new Mock<IMediaListItem>();
            item.Setup(prop => prop.ItemId).Returns(tracks.First().Id);
            item.Setup(prop => prop.TrackId).Returns(tracks.First().Track.Id);
            item.Setup(prop => prop.Artist).Returns(tracks.First().Track.Artist);
            item.Setup(prop => prop.Album).Returns(tracks.First().Track.Album.Name);

            mockPlayer.Setup(player => player.PlaybackList).Returns(mockMediaList.Object);
            mockMediaList.Setup(list => list.Count).Returns(1);
            mockMediaList.Setup(list => list.CurrentIndex).Returns(0);
            mockMediaList.Setup(list => list[It.Is<int>((value) => value == 0)]).Returns(item.Object);

            // Load a blank page
            await view.LoadPage<BlankPage>(null);

            await view.WaitForElementAndExecute<Grid>("BlankPageContainer", (grid) =>
            {

                TrackList trackList = new TrackList()
                {
                    EnableAlbum = true,
                    EnableGenre = true,
                    EnableReleaseYear = true,
                    EnableTrackNumbers = true,
                    Items = tracks,
                    PlayableTracks = tracks
                };

                grid.Children.Add(trackList);
            });

            // Ensure when track list loads with a track playing in the current view, that track is selected
            await view.WaitForElementAndCondition<ListView>("TrackListView", (listView) => listView.SelectedIndex == 0);
        }


        [Fact]
        public async Task TrackListNoTrackPlaying_ThenFireTrackPlaying_VerifyTrackSelected()
        {
            // Load a blank page
            await view.LoadPage<BlankPage>(null);

            await view.WaitForElementAndExecute<Grid>("BlankPageContainer", (grid) =>
            {

                TrackList trackList = new TrackList()
                {
                    EnableAlbum = true,
                    EnableGenre = true,
                    EnableReleaseYear = true,
                    EnableTrackNumbers = true,
                    Items = tracks,
                    PlayableTracks = tracks
                };

                grid.Children.Add(trackList);
            });

            // Ensure when track list loads without a song playing, no item in the list is selected
            await view.WaitForElementAndCondition<ListView>("TrackListView", (listView) => listView.SelectedIndex == -1);

            // Fire current track changed after setting mocks
            var item = new Mock<IMediaListItem>();
            item.Setup(prop => prop.ItemId).Returns(tracks.First().Id);
            item.Setup(prop => prop.TrackId).Returns(tracks.First().Track.Id);
            item.Setup(prop => prop.Artist).Returns(tracks.First().Track.Artist);
            item.Setup(prop => prop.Album).Returns(tracks.First().Track.Album.Name);

            mockPlayer.Setup(player => player.PlaybackList).Returns(mockMediaList.Object);
            mockMediaList.Setup(list => list.Count).Returns(1);
            mockMediaList.Setup(list => list.CurrentIndex).Returns(0);
            mockMediaList.Setup(list => list[It.Is<int>((value) => value == 0)]).Returns(item.Object);

            // Raise the current track changed event
            mockPlayer.Raise(player => player.CurrentTrackChanged += null, EventArgs.Empty);

            // Ensure track list reacts to it and selects the track
            await view.WaitForElementAndCondition<ListView>("TrackListView", (listView) => listView.SelectedIndex == 0);
        }

        [Fact]
        public async Task TrackListNoTrackPlaying_ThenFireTrackPlayingForDifferentTrack_VerifyTrackNotSelected()
        {
            // Load a blank page
            await view.LoadPage<BlankPage>(null);

            await view.WaitForElementAndExecute<Grid>("BlankPageContainer", (grid) =>
            {

                TrackList trackList = new TrackList()
                {
                    EnableAlbum = true,
                    EnableGenre = true,
                    EnableReleaseYear = true,
                    EnableTrackNumbers = true,
                    Items = tracks,
                    PlayableTracks = tracks
                };

                grid.Children.Add(trackList);
            });

            // Ensure when track list loads without a song playing, no item in the list is selected
            await view.WaitForElementAndCondition<ListView>("TrackListView", (listView) => listView.SelectedIndex == -1);

            // Fire current track changed after setting mocks
            var item = new Mock<IMediaListItem>();
            item.Setup(prop => prop.ItemId).Returns("TestId2");
            item.Setup(prop => prop.TrackId).Returns(2);
            item.Setup(prop => prop.Artist).Returns(tracks.First().Track.Artist);
            item.Setup(prop => prop.Album).Returns(tracks.First().Track.Album.Name);

            mockPlayer.Setup(player => player.PlaybackList).Returns(mockMediaList.Object);
            mockMediaList.Setup(list => list.Count).Returns(1);
            mockMediaList.Setup(list => list.CurrentIndex).Returns(0);
            mockMediaList.Setup(list => list[It.Is<int>((value) => value == 0)]).Returns(item.Object);

            // Raise the current track changed event
            mockPlayer.Raise(player => player.CurrentTrackChanged += null, EventArgs.Empty);

            // Ensure track list does not select the wrong track
            await view.WaitForElementAndCondition<ListView>("TrackListView", (listView) => listView.SelectedIndex == -1);
        }

        public void Dispose()
        {
            SimpleIoc.Default.Unregister<IMusicLibrary>();
            SimpleIoc.Default.Unregister<ITrackUrlDownloader>();
            SimpleIoc.Default.Unregister<MusicLibrary>();
            SimpleIoc.Default.Unregister<IPlayer>();
            SimpleIoc.Default.Reset();
        }

        public async Task DisposeAsync()
        {
            await view.UnloadAllPages();
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }
    }
}

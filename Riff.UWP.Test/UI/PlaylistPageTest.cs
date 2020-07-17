using GalaSoft.MvvmLight.Ioc;
using Moq;
using Riff.Data;
using Riff.Data.Sqlite;
using Riff.UWP.Pages;
using Riff.UWP.Test.Infra;
using Riff.UWP.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Xunit;

namespace Riff.UWP.Test.UI
{
    [Collection("UITests")]
    public sealed class PlaylistPageTest : IAsyncLifetime, IDisposable
    {
        private readonly Mock<IMusicLibrary> mockLibrary;
        private readonly Mock<IPlaylistItemReadOnlyAccessor> mockPlaylistAccessor;
        private readonly Mock<IPlayer> mockPlayer;

        private readonly UITree view = new UITree();

        public PlaylistPageTest()
        {
            IMediaList list = null;
            mockLibrary = new Mock<IMusicLibrary>();
            mockPlaylistAccessor = new Mock<IPlaylistItemReadOnlyAccessor>();
            mockLibrary.Setup(library => library.PlaylistItems).Returns(mockPlaylistAccessor.Object);
            mockPlayer = new Mock<IPlayer>(MockBehavior.Strict);
            mockPlayer.Setup(player => player.PlaybackList).Returns(list);

            SimpleIoc.Default.Register(() => mockLibrary.Object);
            SimpleIoc.Default.Register(() => mockPlayer.Object);
            SimpleIoc.Default.Register<PlaylistViewModel>();
        }

        public void Dispose()
        {
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

        [Fact]
        public async Task Navigate_NoItems_VerifyHeaderNameAndToolbarButtonsDisabled()
        {
            // Setup playlist access
            mockPlaylistAccessor.Setup(accessor => accessor.Get(It.IsAny<PlaylistItemAccessOptions>())).Returns(new List<PlaylistItem>());
            Playlist playlist = new Playlist()
            {
                Id = 1,
                Name = "TestPlaylist"
            };
            await view.LoadPage<PlaylistPage>(playlist);

            await view.WaitForElementAndCondition<TextBlock>("HeaderTextControl", textBlock => textBlock.Text == "TestPlaylist");
            await view.WaitForElementAndCondition<AppBarButton>("PlaylistToolbarPlayButton", button => button.IsEnabled == false);
            await view.WaitForElementAndCondition<AppBarButton>("PlaylistToolbarPlayNextButton", button => button.IsEnabled == false);
        }

        [Fact]
        public async Task Navigate_FewItems_VerifyToolbarButtonsEnabled()
        {
            // Setup playlist access
            var playlistItems = new List<PlaylistItem>
            {
                new PlaylistItem()
                {
                    Id = 1,
                    PlaylistId = 1,
                    DriveItem = new DriveItem()
                    {
                        Id = "TestItem1"
                    }
                }
            };
            mockPlaylistAccessor.Setup(accessor => accessor.Get(It.IsAny<PlaylistItemAccessOptions>())).Returns(playlistItems);
            Playlist playlist = new Playlist()
            {
                Id = 1,
                Name = "TestPlaylist"
            };
            await view.LoadPage<PlaylistPage>(playlist);

            await view.WaitForElementAndCondition<AppBarButton>("PlaylistToolbarPlayButton", button => button.IsEnabled);
            await view.WaitForElementAndCondition<AppBarButton>("PlaylistToolbarPlayNextButton", button => button.IsEnabled);
        }

        [Fact]
        public async Task Play_OneItem_VerifyPlayerCalled()
        {
            // Setup playlist access
            var playlistItems = new List<PlaylistItem>
            {
                new PlaylistItem()
                {
                    Id = 1,
                    PlaylistId = 1,
                    DriveItem = new DriveItem()
                    {
                        Id = "TestItem1"
                    }
                }
            };
            mockPlaylistAccessor.Setup(accessor => accessor.Get(It.IsAny<PlaylistItemAccessOptions>())).Returns(playlistItems);
            Playlist playlist = new Playlist()
            {
                Id = 1,
                Name = "TestPlaylist"
            };

            await view.LoadPage<PlaylistPage>(playlist);

            mockPlayer.Setup(player => player.PlayAsync(It.IsAny<IList<DriveItem>>(), It.Is<uint>(index => index == 0), It.Is<bool>(param => param == true)));

            await view.WaitForElementAndCondition<AppBarButton>("PlaylistToolbarPlayButton", button => button.IsEnabled);
            await view.WaitForElementAndExecute<AppBarButton>("PlaylistToolbarPlayButton", button =>
            {
                var automationPeer = new AppBarButtonAutomationPeer(button);
                automationPeer.Invoke();
            });

            mockPlayer.VerifyAll();
        }

        [Fact]
        public async Task PlayNext_OneItem_VerifyPlayerCalled()
        {
            // Setup playlist access
            var playlistItems = new List<PlaylistItem>
            {
                new PlaylistItem()
                {
                    Id = 1,
                    PlaylistId = 1,
                    DriveItem = new DriveItem()
                    {
                        Id = "TestItem1"
                    }
                }
            };
            mockPlaylistAccessor.Setup(accessor => accessor.Get(It.IsAny<PlaylistItemAccessOptions>())).Returns(playlistItems);
            Playlist playlist = new Playlist()
            {
                Id = 1,
                Name = "TestPlaylist"
            };

            await view.LoadPage<PlaylistPage>(playlist);

            mockPlayer.Setup(player => player.PlayAsync(It.IsAny<IList<DriveItem>>(), It.Is<uint>(index => index == 0), It.Is<bool>(param => param == false)));

            await view.WaitForElementAndCondition<AppBarButton>("PlaylistToolbarPlayNextButton", button => button.IsEnabled);
            await view.WaitForElementAndExecute<AppBarButton>("PlaylistToolbarPlayNextButton", button =>
            {
                var automationPeer = new AppBarButtonAutomationPeer(button);
                automationPeer.Invoke();
            });

            mockPlayer.VerifyAll();
        }
    }
}

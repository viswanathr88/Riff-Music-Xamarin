using GalaSoft.MvvmLight.Ioc;
using Moq;
using Riff.Data;
using Riff.UWP.Pages;
using Riff.UWP.Test.Infra;
using Riff.UWP.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Xunit;

namespace Riff.UWP.Test.UI
{
    [Collection("UITests")]
    public sealed class PlaylistsPageTest : IAsyncLifetime, IDisposable
    {
        private readonly Mock<IMusicLibrary> mockMusicLibrary;
        private readonly Mock<IPlaylistReadOnlyAccessor> mockPlaylistsAccessor;
        private readonly Mock<IPlayer> mockPlayer;

        private readonly UITree view;

        public PlaylistsPageTest()
        {
            mockMusicLibrary = new Mock<IMusicLibrary>();
            mockPlaylistsAccessor = new Mock<IPlaylistReadOnlyAccessor>();
            mockPlayer = new Mock<IPlayer>();
            mockMusicLibrary.Setup(library => library.Playlists2).Returns(mockPlaylistsAccessor.Object);

            view = new UITree();

            SimpleIoc.Default.Register(() => mockMusicLibrary.Object);
            SimpleIoc.Default.Register(() => mockPlayer.Object);
            SimpleIoc.Default.Register<PlaylistsViewModel>();
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
        public async Task Navigate_NoItems_VerifyNoPlaylistsMessageIsVisible()
        {
            mockPlaylistsAccessor.Setup(accessor => accessor.Get(It.IsAny<PlaylistAccessOptions>())).Returns(new List<Playlist>());
            Assert.True(await view.LoadPage<PlaylistsPage>(null));

            await view.WaitForElementAndCondition<TextBlock>("NoPlaylistsMessage", textblock => textblock.Visibility == Windows.UI.Xaml.Visibility.Visible);
        }

        [Fact]
        public async Task Navigate_FewItems_VerifyNoPlaylistsMessageIsCollapsed()
        {
            IList<Playlist> playlists = new List<Playlist>()
            {
                new Playlist(){ Id = 1, Name = "TestPlaylist", LastModified = DateTime.Now},
                new Playlist() { Id = 2, Name = "TestPlaylist2", LastModified = DateTime.Now}
            };

            mockPlaylistsAccessor.Setup(accessor => accessor.Get(It.IsAny<PlaylistAccessOptions>())).Returns(playlists);

            Assert.True(await view.LoadPage<PlaylistsPage>(null));

            await view.WaitForElementAndCondition<TextBlock>("NoPlaylistsMessage", textblock => textblock.Visibility == Windows.UI.Xaml.Visibility.Collapsed);
        }

        [Fact]
        public async Task PlaylistsPage_Navigate_VerifyPlaylistsGridCount()
        {
            IList<Playlist> playlists = new List<Playlist>()
            {
                new Playlist(){ Id = 1, Name = "TestPlaylist", LastModified = DateTime.Now},
                new Playlist() { Id = 2, Name = "TestPlaylist2", LastModified = DateTime.Now}
            };

            mockPlaylistsAccessor.Setup(accessor => accessor.Get(It.IsAny<PlaylistAccessOptions>())).Returns(playlists);

            Assert.True(await view.LoadPage<PlaylistsPage>(null));

            await view.WaitForElementAndCondition<GridView>("PlaylistsView", gridView => gridView.Items.Count == playlists.Count);
            await view.WaitForElementAndCondition<GridView>("PlaylistsView", gridView => (gridView.Items[0] as Playlist).Name == playlists[0].Name);
            await view.WaitForElementAndCondition<GridView>("PlaylistsView", gridView => (gridView.Items[1] as Playlist).Name == playlists[1].Name);
        }

        [Fact]
        public async Task PlaylistPage_Navigate_VerifyCommandBarButtons()
        {
            Assert.True(await view.LoadPage<PlaylistsPage>(null));

            await view.WaitForElement<CommandBar>("PlaylistsCommandBar");
            await view.WaitForElement<AppBarButton>("AddPlaylistButton");
            await view.WaitForElement<AppBarToggleButton>("ManagePlaylistsButton");
        }

        [Fact]
        public async Task Navigate_EnableManageMode_VerifyAdditionalCommandBarButtons()
        {
            Assert.True(await view.LoadPage<PlaylistsPage>(null));

            await view.WaitForElement<CommandBar>("PlaylistsCommandBar");

            // Click the manage mode button
            await view.WaitForElementAndExecute<AppBarToggleButton>("ManagePlaylistsButton", button => button.IsChecked = true);

            await view.WaitForElement<AppBarButton>("SelectAllPlaylistsButton");
            await view.WaitForElement<AppBarButton>("ClearPlaylistsSelectionButton");
            await view.WaitForElement<AppBarButton>("DeletePlaylistsButton");
            await view.WaitForElement<AppBarButton>("PlayPlaylistsButton");
            await view.WaitForElement<AppBarButton>("PlayPlaylistsNextButton");
        }

        [Fact]
        public async Task ManageMode_ClickSelectAll_VerifySelection()
        {
            IList<Playlist> playlists = new List<Playlist>()
            {
                new Playlist(){ Id = 1, Name = "TestPlaylist", LastModified = DateTime.Now},
                new Playlist() { Id = 2, Name = "TestPlaylist2", LastModified = DateTime.Now}
            };

            mockPlaylistsAccessor.Setup(accessor => accessor.Get(It.IsAny<PlaylistAccessOptions>())).Returns(playlists);

            Assert.True(await view.LoadPage<PlaylistsPage>(null));

            // Ensure playlists have loaded
            await view.WaitForElementAndCondition<GridView>("PlaylistsView", gridView => gridView.Items.Count == playlists.Count);

            await view.WaitForElement<CommandBar>("PlaylistsCommandBar");

            // Click the manage mode button
            await view.WaitForElementAndExecute<AppBarToggleButton>("ManagePlaylistsButton", button => button.IsChecked = true);

            // Ensure nothing is selected
            await view.WaitForElementAndCondition<GridView>("PlaylistsView", gridView => gridView.SelectedItems.Count == 0);

            // Click the select all button
            await view.WaitForElementAndExecute<AppBarButton>("SelectAllPlaylistsButton", button =>
            {
                AppBarButtonAutomationPeer peer = new AppBarButtonAutomationPeer(button);
                peer.Invoke();
            });

            // Ensure all items in the grid are selected
            await view.WaitForElementAndCondition<GridView>("PlaylistsView", gridView => gridView.SelectedItems.Count == playlists.Count);

            // Click the clear selection button
            await view.WaitForElementAndExecute<AppBarButton>("ClearPlaylistsSelectionButton", button =>
            {
                AppBarButtonAutomationPeer peer = new AppBarButtonAutomationPeer(button);
                peer.Invoke();
            });

            // Ensure all items in the grid are not selected anymore
            await view.WaitForElementAndCondition<GridView>("PlaylistsView", gridView => gridView.SelectedItems.Count == 0);
        }
    }
}

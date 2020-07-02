using Riff.Data;
using Riff.UWP.Controls;
using Riff.UWP.ViewModel;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Riff.UWP.Pages
{
    public class PlaylistsPageBase : NavViewPageBase<PlaylistsViewModel>
    {
    }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlaylistsPage : PlaylistsPageBase
    {
        private Playlist currentContextMenuSelection;

        public PlaylistsPage()
        {
            this.InitializeComponent();
            HeaderText = Strings.Resources.PlaylistsPageHeader;
        }

        private bool IsItemClickEnabled(bool IsSelectionMode)
        {
            return !IsSelectionMode;
        }

        private void PlaylistsView_ItemClick(object sender, Windows.UI.Xaml.Controls.ItemClickEventArgs e)
        {
            if (e.ClickedItem != null)
            {
                Frame.Navigate(typeof(PlaylistPage), e.ClickedItem as Playlist, new EntranceNavigationTransitionInfo());
            }
        }

        private async void AddPlaylistButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var dialog = new AddPlaylistDialog();
            var result = await dialog.ShowAsync();

            ViewModel.PlaylistName = string.Empty;
        }

        private Windows.UI.Xaml.Controls.ListViewSelectionMode SetSelectionMode(bool isSelectionMode)
        {
            return isSelectionMode ? Windows.UI.Xaml.Controls.ListViewSelectionMode.Multiple : Windows.UI.Xaml.Controls.ListViewSelectionMode.None;
        }

        private bool UpdateIsSelectionMode(Windows.UI.Xaml.Controls.ListViewSelectionMode mode)
        {
            return mode == Windows.UI.Xaml.Controls.ListViewSelectionMode.Multiple;
        }

        private void SelectAllButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            PlaylistsView.SelectAll();
        }

        private void ClearPlaylistsSelectionButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            PlaylistsView.SelectedItems.Clear();
        }

        private void PlaylistContextMenu_Opening(object sender, object e)
        {
            if (sender is MenuFlyout flyout && flyout.Target is ListViewItem lvitem)
            {
                var index = PlaylistsView.IndexFromContainer(lvitem);
                currentContextMenuSelection = ViewModel.Playlists[index];
            }
        }

        private void PlaylistContextMenu_Closing(Windows.UI.Xaml.Controls.Primitives.FlyoutBase sender, Windows.UI.Xaml.Controls.Primitives.FlyoutBaseClosingEventArgs args)
        {
            currentContextMenuSelection = null;
        }

        private async void RenamePlaylistContextMenuItem_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var dialog = new RenamePlaylistDialog(currentContextMenuSelection);
            await dialog.ShowAsync();
        }

        private void PlayPlaylistContextMenuItem_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel.Play.Execute(currentContextMenuSelection);
        }

        private void PlayPlaylistNextContextMenuItem_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel.PlayNext.Execute(currentContextMenuSelection);
        }

        private void DeletePlaylistContextMenuItem_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel.Delete.Execute(currentContextMenuSelection);
        }
    }
}

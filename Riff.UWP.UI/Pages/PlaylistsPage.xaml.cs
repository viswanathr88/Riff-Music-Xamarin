using Riff.Data;
using Riff.UWP.Controls;
using Riff.UWP.ViewModel;
using System;
using System.Threading.Tasks;
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

            if (result == ContentDialogResult.Primary)
            {
                ViewModel.Add.Execute(null);

                if (ViewModel.Add.Error != null)
                {
                    // Create playlist failed
                    await ShowAddPlaylistErrorMessageDialog();
                }
                else
                {
                    // Create playlist succeeded
                    await ViewModel.LoadAsync();
                }
            }
        }

        private async Task ShowAddPlaylistErrorMessageDialog()
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = Strings.Resources.AddPlaylistErrorDialogTitle,
                Content = Strings.Resources.AddPlaylistErrorDialogContent,
                CloseButtonText = Strings.Resources.AddPlaylistErrorDialogCloseButtonText
            };

            await dialog.ShowAsync();
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
            if (sender is MenuFlyout flyout && flyout.Target is GridViewItem item)
            {
                var index = PlaylistsView.IndexFromContainer(item);
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
            await ViewModel.LoadAsync();
        }

        private void PlayPlaylistContextMenuItem_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel.Play.Execute(currentContextMenuSelection);
        }

        private void PlayPlaylistNextContextMenuItem_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel.PlayNext.Execute(currentContextMenuSelection);
        }

        private async void DeletePlaylistContextMenuItem_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var contextMenuItem = currentContextMenuSelection;
            var result = await ShowDeleteConfirmation();

            if (result == ContentDialogResult.Primary)
            {
                ViewModel.Delete.Execute(contextMenuItem);
                await ViewModel.LoadAsync();
            }
        }

        private async void DeletePlaylistsButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var result = await ShowDeleteConfirmation();

            if (result == ContentDialogResult.Primary)
            {
                ViewModel.DeleteMultiple.Execute(PlaylistsView.SelectedItems);
                await ViewModel.LoadAsync();
            }
        }

        private async Task<ContentDialogResult> ShowDeleteConfirmation()
        {
            var dialog = new DeletePlaylistConfirmationDialog();
            return await dialog.ShowAsync();
        }
    }
}

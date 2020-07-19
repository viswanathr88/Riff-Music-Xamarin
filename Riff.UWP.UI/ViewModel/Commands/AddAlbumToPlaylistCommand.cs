using Mirage.ViewModel.Commands;
using Riff.Data;
using Riff.UWP.Controls;
using System;
using System.Threading.Tasks;

namespace Riff.UWP.ViewModel.Commands
{
    public sealed class AddAlbumToPlaylistCommand : AsyncCommand<AlbumItemViewModel>
    {
        private readonly IMusicLibrary library;
        private readonly PlaylistsViewModel playlistsVM;

        public AddAlbumToPlaylistCommand(IMusicLibrary library, PlaylistsViewModel playlistsVM)
        {
            this.library = library;
            this.playlistsVM = playlistsVM;
        }

        public override bool CanExecute(AlbumItemViewModel album)
        {
            return album.Item.Id.HasValue;
        }

        protected override async Task RunAsync(AlbumItemViewModel album)
        {
            AddToPlaylistDialog dialog = new AddToPlaylistDialog();
            var result = await dialog.ShowAsync();

            if (result == Windows.UI.Xaml.Controls.ContentDialogResult.Primary)
            {
                Playlist playlist;
                if (!string.IsNullOrEmpty(dialog.NewPlaylistName))
                {
                    playlist = new Playlist()
                    {
                        Name = dialog.NewPlaylistName
                    };

                    using (var session = library.Edit())
                    {
                        session.Playlists.Add(playlist);
                        session.Save();
                    }
                }
                else
                {
                    playlist = dialog.SelectedPlaylist;
                }

                await playlistsVM.AddToPlaylist(album.Item, playlist);
            }
        }
    }
}

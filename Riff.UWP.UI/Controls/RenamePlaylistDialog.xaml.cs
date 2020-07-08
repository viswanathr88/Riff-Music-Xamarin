using GalaSoft.MvvmLight.Ioc;
using Riff.Data;
using Riff.UWP.ViewModel;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Riff.UWP.Controls
{
    public sealed partial class RenamePlaylistDialog : ContentDialog
    {
        public RenamePlaylistDialog(Playlist playlist)
        {
            this.InitializeComponent();
            CurrentPlaylist = playlist;
            ViewModel.Rename.PlaylistName = CurrentPlaylist.Name;
            Loaded += RenamePlaylistDialog_Loaded;
        }

        private void RenamePlaylistDialog_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            RenamePlaylistTextBox.SelectAll();
        }

        public PlaylistsViewModel ViewModel => SimpleIoc.Default.GetInstance<PlaylistsViewModel>();

        public Playlist CurrentPlaylist { get; }
    }
}

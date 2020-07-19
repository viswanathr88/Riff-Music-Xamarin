using CommonServiceLocator;
using Riff.Data;
using Riff.UWP.ViewModel;
using System.ComponentModel;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Riff.UWP.Controls
{
    public sealed partial class AddToPlaylistDialog : ContentDialog, INotifyPropertyChanged
    {
        private Playlist selectedPlaylist;
        private string newPlaylistName;

        public AddToPlaylistDialog()
        {
            this.InitializeComponent();
        }

        public Playlist SelectedPlaylist
        {
            get => selectedPlaylist;
            set
            {
                if (this.selectedPlaylist != value)
                {
                    this.selectedPlaylist = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedPlaylist)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsOKButtonEnabled)));
                }
            }
        }

        public string NewPlaylistName
        {
            get => newPlaylistName;
            set
            {
                if (this.newPlaylistName != value)
                {
                    this.newPlaylistName = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NewPlaylistName)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsOKButtonEnabled)));
                }
            }
        }

        public bool IsOKButtonEnabled
        {
            get => SelectedPlaylist != null || !string.IsNullOrEmpty(NewPlaylistName);
        }

        private PlaylistsViewModel Playlists => ServiceLocator.Current.GetInstance<PlaylistsViewModel>();

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

using GalaSoft.MvvmLight.Ioc;
using Riff.UWP.ViewModel;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Riff.UWP.Controls
{
    public sealed partial class AddPlaylistDialog : ContentDialog
    {
        public AddPlaylistDialog()
        {
            this.InitializeComponent();
        }

        public PlaylistsViewModel ViewModel => SimpleIoc.Default.GetInstance<PlaylistsViewModel>();

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}

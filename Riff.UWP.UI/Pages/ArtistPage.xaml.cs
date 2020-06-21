using CommonServiceLocator;
using Riff.UWP.ViewModel;
using Windows.ApplicationModel.Resources;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Riff.UWP.Pages
{
    public class ArtistPageBase : NavViewPageBase<ArtistViewModel>
    {
    }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ArtistPage : ArtistPageBase
    {
        public ArtistPage()
        {
            this.InitializeComponent();
            HeaderText = ResourceLoader.GetForCurrentView().GetString("ArtistPageHeader");
            Loaded += ArtistPage_Loaded;
        }

        private void ArtistPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ArtistToolbarPlayButton.Focus(Windows.UI.Xaml.FocusState.Programmatic);
        }

        private async void ArtistToolbarPlayButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await AlbumList.Play();
        }
    }
}

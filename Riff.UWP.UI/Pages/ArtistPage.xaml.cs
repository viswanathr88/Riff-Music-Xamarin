using CommonServiceLocator;
using Riff.Data;
using Riff.UWP.ViewModel;
using System.Threading.Tasks;
using Windows.UI.Xaml;

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
            HeaderText = Strings.Resources.ArtistPageHeader;
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

        private async void AddToNowPlayingListMenuItem_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await HandlePlayClick(sender, true);
        }

        public async static Task HandlePlayClick(object sender, bool addToCurrentList)
        {
            if (sender is FrameworkElement element && element.Tag is Artist artist)
            {
                var player = ServiceLocator.Current.GetInstance<IPlayer>();
                await player.PlayAsync(artist, addToCurrentList);
            }
        }
    }
}

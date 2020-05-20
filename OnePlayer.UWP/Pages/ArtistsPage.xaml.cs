using OnePlayer.UWP.ViewModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace OnePlayer.UWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ArtistsPage : LibraryPageBase, ISupportViewModel<ArtistsViewModel>
    {
        public ArtistsPage()
        {
            this.InitializeComponent();
        }

        public ArtistsViewModel ViewModel => (Application.Current.Resources["VMLocator"] as Locator).MusicLibrary.Artists;

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (!ViewModel.IsLoaded)
            {
                await ViewModel.LoadAsync(VoidType.Empty);
            }
        }
    }
}

using OnePlayer.Data;
using OnePlayer.UWP.ViewModel;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace OnePlayer.UWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ArtistPage : NavViewPageBase, ISupportViewModel<ArtistViewModel>
    {
        public ArtistPage()
        {
            this.InitializeComponent();
            HeaderText = ResourceLoader.GetForCurrentView().GetString("ArtistPageHeader");
        }

        private static Locator Locator => Application.Current.Resources["VMLocator"] as Locator;
        public ArtistViewModel ViewModel { get; } = new ArtistViewModel(Locator.Library);

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (!ViewModel.IsLoaded)
            {
                await ViewModel.LoadAsync(e.Parameter as Artist);
            }
        }
    }
}

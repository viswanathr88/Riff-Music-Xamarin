using OnePlayer.UWP.ViewModel;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
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
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;

            if (!ViewModel.IsLoaded)
            {
                await ViewModel.LoadAsync();
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        private async void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.IsLoaded) && !ViewModel.IsLoaded)
            {
                await ViewModel.LoadAsync();
            }
        }

        public static string FormatAlbumsCount(int albumsCount)
        {
            string resourceKey = albumsCount == 1 ? "ArtistsPageOneAlbumFormat" : "ArtistsPageMultipleAlbumFormat";
            return string.Format(ResourceLoader.GetForCurrentView().GetString(resourceKey), albumsCount);
        }

        private void ArtistList_ItemClick(object sender, Windows.UI.Xaml.Controls.ItemClickEventArgs e)
        {
            if (e.ClickedItem != null)
            {
                var item = e.ClickedItem as ArtistItemViewModel;
                var parentFrame = VisualTreeHelperExtensions.FindParent<Frame>(Frame, "ContentFrame");
                parentFrame.Navigate(typeof(ArtistPage), item.Model, new EntranceNavigationTransitionInfo());
            }
        }
    }
}

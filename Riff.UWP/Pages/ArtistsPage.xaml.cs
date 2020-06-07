using Riff.UWP.ViewModel;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Riff.UWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ArtistsPage : LibraryPageBase, ISupportViewModel<ArtistsViewModel>
    {
        public ArtistsPage()
        {
            this.InitializeComponent();
            RegisterForChanges = true;
        }

        public ArtistsViewModel ViewModel => Locator.MusicLibrary.Artists;

        public override IDataViewModel DataViewModel => ViewModel;

        protected async override void HandleViewModelPropertyChanged(string propertyName)
        {
            base.HandleViewModelPropertyChanged(propertyName);

            if (propertyName == nameof(ViewModel.IsLoaded) && !ViewModel.IsLoaded)
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

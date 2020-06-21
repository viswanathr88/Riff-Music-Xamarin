using Riff.UWP.ViewModel;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Riff.UWP.Pages
{
    public class ArtistsPageBase : LibraryPageBase<ArtistsViewModel>
    {

    }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ArtistsPage : ArtistsPageBase
    {
        public ArtistsPage()
        {
            this.InitializeComponent();
            RegisterForChanges = true;
        }

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
            return string.Format(albumsCount == 1 ? Strings.Resources.ArtistsPageOneAlbumFormat : Strings.Resources.ArtistsPageMultipleAlbumFormat, albumsCount);
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

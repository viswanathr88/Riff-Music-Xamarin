using Riff.UWP.ViewModel;
using Windows.ApplicationModel.Resources;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Riff.UWP.Pages
{
    public class PlaylistsPageBase : NavViewPageBase<PlaylistsViewModel>
    {
    }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlaylistsPage : PlaylistsPageBase
    {
        public PlaylistsPage()
        {
            this.InitializeComponent();
            HeaderText = Strings.Resources.PlaylistPageHeader;
        }
    }
}

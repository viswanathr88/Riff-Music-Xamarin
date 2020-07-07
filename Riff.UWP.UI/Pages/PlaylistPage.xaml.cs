using CommonServiceLocator;
using Riff.UWP.Controls;
using Riff.UWP.ViewModel;
using System;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Riff.UWP.Pages
{
    public class PlaylistPageBase : NavViewPageBase<PlaylistViewModel>
    {

    }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlaylistPage : PlaylistPageBase
    {
        public PlaylistPage()
        {
            this.InitializeComponent();
            HeaderText = Strings.Resources.PlaylistPageHeader;
        }

        private PlaylistsViewModel PlaylistsVM => ServiceLocator.Current.GetInstance<PlaylistsViewModel>();
    }
}

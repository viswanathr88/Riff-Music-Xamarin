using CommonServiceLocator;
using Riff.Data;
using Riff.UWP.ViewModel;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Riff.UWP.Pages
{
    public class ArtistPageBase : NavViewPageBase<ArtistViewModel>
    {
    }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ArtistPage : ArtistPageBase, ISupportPlaying
    {
        public ArtistPage()
        {
            this.InitializeComponent();
            HeaderText = ResourceLoader.GetForCurrentView().GetString("ArtistPageHeader");
        }

        public PlayerViewModel Player => ServiceLocator.Current.GetInstance<PlayerViewModel>();

        private async void ArtistToolbarPlayButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await Player.PlayAsync(ViewModel.PlayableTracks, 0);
        }

        private async void AlbumList_ItemClick(object sender, Windows.UI.Xaml.Controls.ItemClickEventArgs e)
        {
            if (e.ClickedItem != null)
            {
                var index = (sender as ListView).Items.IndexOf(e.ClickedItem);
                await Player.PlayAsync(ViewModel.PlayableTracks, Convert.ToUInt32(index));
            }
        }
    }
}

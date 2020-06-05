using OnePlayer.UWP.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace OnePlayer.UWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NowPlayingPage : NavViewPageBase, ISupportViewModel<PlayerViewModel>
    {
        public NowPlayingPage()
        {
            this.InitializeComponent();
            HeaderText = ResourceLoader.GetForCurrentView().GetString("NowPlayingPageHeader");
        }

        public override IDataViewModel DataViewModel => ViewModel;

        public PlayerViewModel ViewModel => Locator.Player;

        private void Titlebar_BackRequested(object sender, EventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private void NowPlayingList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem != null)
            {
            }
        }

        private void NowPlayingList_Loaded(object sender, RoutedEventArgs e)
        {
            MediaPlayerControl.SetMediaPlayer(ViewModel.MediaPlayer);
            if (ViewModel.PlaybackList != null && ViewModel.PlaybackList.CurrentItem != null)
            {
                NowPlayingList.ScrollIntoView(ViewModel.PlaybackList.CurrentItem, ScrollIntoViewAlignment.Leading);
            }
        }
    }
}

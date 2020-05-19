using OnePlayer.Data;
using OnePlayer.UWP.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace OnePlayer.UWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AlbumPage : NavViewPageBase, ISupportViewModel<AlbumViewModel>
    {
        public AlbumPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            HeaderText = ResourceLoader.GetForCurrentView().GetString("AlbumPageHeader");
        }

        public AlbumViewModel ViewModel { get; } = new AlbumViewModel(Locator.MusicMetadata);

        private static Locator Locator => Application.Current.Resources["VMLocator"] as Locator;

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ConnectedAnimation imageAnimation = ConnectedAnimationService.GetForCurrentView().GetAnimation("ca1");
            if (imageAnimation != null)
            {
                imageAnimation.TryStart(AlbumArt);
            }

            if (!ViewModel.IsLoaded || ViewModel.AlbumInfo != e.Parameter as Album)
            {
                await ViewModel.LoadAsync(e.Parameter as Album);
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            if (e.NavigationMode == NavigationMode.Back)
            {
                var service = ConnectedAnimationService.GetForCurrentView();
                var animation = service.PrepareToAnimate("backAnimation", AlbumArt);
            }
        }

        private async Task LoadArtAsync(Image image, Album album)
        {
            ThumbnailSize artSize = ThumbnailSize.Medium;
            if (Locator.Library.AlbumArts.Exists(album.Id.Value, artSize))
            {
                using (var stream = Locator.Library.AlbumArts.Get(album.Id.Value, artSize))
                {
                    using (var rtStream = stream.AsRandomAccessStream())
                    {
                        var bm = new BitmapImage();
                        await bm.SetSourceAsync(rtStream);
                        image.Source = bm;
                    }
                }
            }
        }

        private async void AlbumArt_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadArtAsync(AlbumArt, ViewModel.AlbumInfo);
        }

        private void AlbumTrackList_ItemClick(object sender, ItemClickEventArgs e)
        {

        }
    }
}

using OnePlayer.Data;
using OnePlayer.UWP.ViewModel;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace OnePlayer.UWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AlbumPage : NavViewPageBase, ISupportViewModel<AlbumViewModel>, ISupportPlaying
    {
        bool animateBack = false;
        public AlbumPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            HeaderText = ResourceLoader.GetForCurrentView().GetString("AlbumPageHeader");
        }

        public AlbumViewModel ViewModel { get; } = new AlbumViewModel(Locator.MusicMetadata);

        public override IDataViewModel DataViewModel => ViewModel;

        public PlayerViewModel Player => Locator.Player;

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // base.OnNavigatedTo(e);

            ConnectedAnimation imageAnimation = ConnectedAnimationService.GetForCurrentView().GetAnimation("ca1");
            if (imageAnimation != null)
            {
                imageAnimation.TryStart(AlbumArt);
                animateBack = true;
            }

            if (!ViewModel.IsLoaded || ViewModel.AlbumInfo != e.Parameter as Album)
            {
                await ViewModel.LoadAsync(e.Parameter as Album);
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            if (animateBack && e.NavigationMode == NavigationMode.Back)
            {
                var service = ConnectedAnimationService.GetForCurrentView();
                var animation = service.PrepareToAnimate("backAnimation", AlbumArt);
                animateBack = false;
            }
        }

        private async Task LoadArtAsync(Image image, Album album)
        {
            if (Locator.Library.AlbumArts.Exists(album.Id.Value))
            {
                using (var stream = Locator.Library.AlbumArts.Get(album.Id.Value))
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

        private async void AlbumTrackList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem != null)
            {
                var index = Convert.ToUInt32(AlbumTrackList.Items.IndexOf(e.ClickedItem));
                await Player.PlayAsync(ViewModel.AlbumInfo, index);
            }
        }

        private void AlbumTrackList_Loaded(object sender, RoutedEventArgs e)
        {
            /*var listScrollViewer = VisualTreeHelperExtensions.FindVisualChild<ScrollViewer>(sender as DependencyObject);
            if (listScrollViewer == null)
            {
                return;
            }

            // Update the ZIndex of the header container so that the header is above the items when scrolling
            var headerPresenter = (UIElement)VisualTreeHelper.GetParent((UIElement)AlbumHeader);
            var headerContainer = (UIElement)VisualTreeHelper.GetParent(headerPresenter);
            Canvas.SetZIndex((UIElement)headerContainer, 1);

            var scrollerPropertySet = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(listScrollViewer);
            var scrollingProperties = scrollerPropertySet.GetSpecializedReference<ManipulationPropertySetReferenceNode>();
            var compositor = scrollerPropertySet.Compositor;

            // Create a PropertySet that has values to be referenced in the ExpressionAnimations below
            var _props = compositor.CreatePropertySet();
            _props.InsertScalar("progress", 0);
            _props.InsertScalar("clampSize", 150);
            _props.InsertScalar("scaleFactor", 0.4f);

            // Access nodes
            var props = _props.GetReference();
            var progressNode = props.GetScalarProperty("progress");
            var clampSizeNode = props.GetScalarProperty("clampSize");
            var scaleFactorNode = props.GetScalarProperty("scaleFactor");

            // Create and start an ExpressionAnimation to track scroll progress over the desired distance
            ExpressionNode progressAnimation = ExpressionFunctions.Clamp(-scrollingProperties.Translation.Y / clampSizeNode, 0, 1);
            _props.StartAnimation("progress", progressAnimation);

            // Create an expression animation for clamping the header to the top while scrolling
            var headerVisual = ElementCompositionPreview.GetElementVisual(AlbumHeader);
            ExpressionNode headerTranslationAnimation = ExpressionFunctions.Conditional(progressNode < 1, 0, -scrollingProperties.Translation.Y - clampSizeNode);
            headerVisual.StartAnimation("Offset.Y", headerTranslationAnimation);

            // Get the backing visual for the photo in the header so that its properties can be animated
            Windows.UI.Composition.Visual photoVisual = ElementCompositionPreview.GetElementVisual(AlbumArt);
            ExpressionNode scaleAnimation = ExpressionFunctions.Lerp(1, scaleFactorNode, progressNode);
            photoVisual.StartAnimation("Scale.X", scaleAnimation);
            photoVisual.StartAnimation("Scale.Y", scaleAnimation);


            // headerVisual.StartAnimation("Offset.Y", offsetExpression);*/
        }

        private async void AlbumToolbarPlayButton_Click(object sender, RoutedEventArgs e)
        {
            await Player.PlayAsync(ViewModel.AlbumInfo);
        }
    }
}

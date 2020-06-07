using ExpressionBuilder;
using OnePlayer.UWP.ViewModel;
using System;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;
using EF = ExpressionBuilder.ExpressionFunctions;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace OnePlayer.UWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NowPlayingPage : ShellPageBase, ISupportViewModel<PlayerViewModel>
    {
        CompositionPropertySet _props;
        CompositionPropertySet _scrollerPropertySet;
        Compositor _compositor;

        public NowPlayingPage()
        {
            this.InitializeComponent();
        }

        public override IDataViewModel DataViewModel => ViewModel;

        public PlayerViewModel ViewModel => Locator.Player;

        public override bool CanGoBack => Frame.CanGoBack;

        private void Titlebar_BackRequested(object sender, EventArgs e)
        {
            GoBack();
        }

        public override void GoBack()
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
            // Set the media player
            MediaPlayerControl.SetMediaPlayer(ViewModel.MediaPlayer);
            
            // Make header sticky
            MakeHeaderSticky();

            // Scroll to selected item
            ScrollToSelectedItem();
        }

        private void MakeHeaderSticky()
        {
            // Retrieve the scroll viewer of the list view
            var scrollViewer = VisualTreeHelperExtensions.FindVisualChild<ScrollViewer>(NowPlayingList);

            // Set the z-Index on the header so that it is above the content
            var headerPresenter = (UIElement)VisualTreeHelper.GetParent((UIElement)NowPlayingList.Header);
            var headerContainer = (UIElement)VisualTreeHelper.GetParent(headerPresenter);
            Canvas.SetZIndex(headerContainer, 1);

            // Get the PropertySet that contains the scroll values from the ScrollViewer
            _scrollerPropertySet = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(scrollViewer);
            _compositor = _scrollerPropertySet.Compositor;

            // Create a PropertySet that has values to be referenced in the ExpressionAnimations below
            _props = _compositor.CreatePropertySet();
            _props.InsertScalar("progress", 0);
            _props.InsertScalar("clampSize", 175);
            _props.InsertScalar("scaleFactor", 0.5f);

            // Get references to our property sets for use with ExpressionNodes
            var scrollingProperties = _scrollerPropertySet.GetSpecializedReference<ManipulationPropertySetReferenceNode>();
            var props = _props.GetReference();
            var progressNode = props.GetScalarProperty("progress");
            var clampSizeNode = props.GetScalarProperty("clampSize");
            var scaleFactorNode = props.GetScalarProperty("scaleFactor");

            // Create and start an ExpressionAnimation to track scroll progress over the desired distance
            ExpressionNode progressAnimation = EF.Clamp(-scrollingProperties.Translation.Y / clampSizeNode, 0, 1);
            _props.StartAnimation("progress", progressAnimation);

            // Get the backing visual for the header so that its properties can be animated
            Visual headerVisual = ElementCompositionPreview.GetElementVisual(Header);

            // Create and start an ExpressionAnimation to clamp the header's offset to keep it onscreen
            ExpressionNode headerTranslationAnimation = EF.Conditional(progressNode < 1, 0, -scrollingProperties.Translation.Y - clampSizeNode);
            headerVisual.StartAnimation("Offset.Y", headerTranslationAnimation);

            // Get backing visuals for the text blocks so that their properties can be animated
            Visual albumVisual = ElementCompositionPreview.GetElementVisual(TrackAlbum);
            Visual albumReleaseYearVisual = ElementCompositionPreview.GetElementVisual(TrackAlbumYear);

            // Create an ExpressionAnimation that moves between 1 and 0 with scroll progress, to be used for text block opacity
            ExpressionNode textOpacityAnimation = EF.Clamp(1 - (progressNode), 0, 1);
            albumVisual.StartAnimation("Opacity", textOpacityAnimation);
            albumReleaseYearVisual.StartAnimation("Opacity", textOpacityAnimation);

            // Get the backing visual for the photo in the header so that its properties can be animated
            /*Windows.UI.Composition.Visual albumArtVisual = ElementCompositionPreview.GetElementVisual(AlbumArtContainer);
            ExpressionNode scaleAnimation = EF.Lerp(1, scaleFactorNode, progressNode);
            albumArtVisual.StartAnimation("Scale.X", scaleAnimation);
            albumArtVisual.StartAnimation("Scale.Y", scaleAnimation);

            ExpressionNode albumArtOffsetAnimation = progressNode * 75;
            albumArtVisual.StartAnimation("Offset.X", albumArtOffsetAnimation);*/

            Visual detailsVisual = ElementCompositionPreview.GetElementVisual(TrackDetailsContainer);
            // ExpressionNode contentOffsetAnimation = progressNode * 175;
            // detailsVisual.StartAnimation("Offset.Y", contentOffsetAnimation);
            detailsVisual.StartAnimation("Opacity", textOpacityAnimation);

            Visual playerVisual = ElementCompositionPreview.GetElementVisual(PlayerContainer);
            ExpressionNode playerOffsetAnimation = progressNode * -75;
            playerVisual.StartAnimation("Offset.Y", playerOffsetAnimation);
        }

        private void ScrollToSelectedItem()
        {
            if (ViewModel.PlaybackList != null && ViewModel.PlaybackList.CurrentIndex != -1)
            {
                var index = Math.Min(ViewModel.PlaybackList.CurrentIndex + 5, ViewModel.PlaybackList.Count - 1);
                NowPlayingList.ScrollIntoView(ViewModel.PlaybackList[index]);
            }
        }
    }
}

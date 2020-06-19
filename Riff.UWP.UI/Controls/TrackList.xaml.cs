using CommonServiceLocator;
using Riff.Data;
using Riff.UWP.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Riff.UWP.Controls
{
    public sealed partial class TrackList : UserControl
    {
        public TrackList()
        {
            this.InitializeComponent();

            EvaluateTrackAlbumVisibility();
            EvaluateTrackArtVisibility();
            EvaluateTrackGenreVisibility();
            EvaluateTrackNumberVisibility();
            EvaluateTrackReleaseYearVisibility();
        }

        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(TrackList), new PropertyMetadata(null));

        public IList<DriveItem> Items
        {
            get { return (IList<DriveItem>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Items.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(IList<DriveItem>), typeof(TrackList), new PropertyMetadata(null));

        #region Public Column Visibility Methods

        public bool EnableTrackNumbers
        {
            get { return (bool)GetValue(EnableTrackNumbersProperty); }
            set { SetValue(EnableTrackNumbersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableTrackNumbers.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableTrackNumbersProperty =
            DependencyProperty.Register("EnableTrackNumbers", typeof(bool), typeof(TrackList), new PropertyMetadata(false, OnEnableTrackNumbersChanged));

        public bool EnableArt
        {
            get { return (bool)GetValue(EnableArtProperty); }
            set { SetValue(EnableArtProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableArt.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableArtProperty =
            DependencyProperty.Register("EnableArt", typeof(bool), typeof(TrackList), new PropertyMetadata(false, OnEnableTrackArtChanged));

        public bool EnableAlbum
        {
            get { return (bool)GetValue(EnableAlbumProperty); }
            set { SetValue(EnableAlbumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableAlbum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableAlbumProperty =
            DependencyProperty.Register("EnableAlbum", typeof(bool), typeof(TrackList), new PropertyMetadata(false, OnEnableAlbumChanged));

        public bool EnableGenre
        {
            get { return (bool)GetValue(EnableGenreProperty); }
            set { SetValue(EnableGenreProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableGenre.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableGenreProperty =
            DependencyProperty.Register("EnableGenre", typeof(bool), typeof(TrackList), new PropertyMetadata(false, OnEnableGenreChanged));


        public bool EnableReleaseYear
        {
            get { return (bool)GetValue(EnableReleaseYearProperty); }
            set { SetValue(EnableReleaseYearProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableReleaseYear.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableReleaseYearProperty =
            DependencyProperty.Register("EnableReleaseYear", typeof(bool), typeof(TrackList), new PropertyMetadata(false, OnEnableReleaseYearChanged));

        private static void OnEnableTrackNumbersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TrackList).EvaluateTrackNumberVisibility();
        }

        private static void OnEnableTrackArtChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TrackList).EvaluateTrackArtVisibility();
        }

        private static void OnEnableAlbumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TrackList).EvaluateTrackAlbumVisibility();
        }

        private static void OnEnableGenreChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TrackList).EvaluateTrackGenreVisibility();
        }

        private static void OnEnableReleaseYearChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TrackList).EvaluateTrackReleaseYearVisibility();
        }

        #endregion

        #region Private Adaptive Trigger Column Visibility Properties

        private bool ShowTrackNumberGridItem
        {
            get { return (bool)GetValue(ShowTrackNumberGridItemProperty); }
            set { SetValue(ShowTrackNumberGridItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowTrackNumber GridItem.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty ShowTrackNumberGridItemProperty =
            DependencyProperty.Register("ShowTrackNumberGridItem", typeof(bool), typeof(TrackList), new PropertyMetadata(true, OnShowTrackNumberGridItemChanged));

        private bool ShowTrackArtGridItem
        {
            get { return (bool)GetValue(ShowTrackArtGridItemProperty); }
            set { SetValue(ShowTrackArtGridItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowTrackArtGridItem.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty ShowTrackArtGridItemProperty =
            DependencyProperty.Register("ShowTrackArtGridItem", typeof(bool), typeof(TrackList), new PropertyMetadata(true, OnShowTrackArtGridItemChanged));

        private bool ShowTrackAlbumGridItem
        {
            get { return (bool)GetValue(ShowTrackAlbumGridItemProperty); }
            set { SetValue(ShowTrackAlbumGridItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowTrackAlbumGridItem.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty ShowTrackAlbumGridItemProperty =
            DependencyProperty.Register("ShowTrackAlbumGridItem", typeof(bool), typeof(TrackList), new PropertyMetadata(false, OnShowTrackAlbumGridItemChanged));

        private bool ShowTrackGenreGridItem
        {
            get { return (bool)GetValue(ShowTrackGenreGridItemProperty); }
            set { SetValue(ShowTrackGenreGridItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowTrackGenreGridItemProperty.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty ShowTrackGenreGridItemProperty =
            DependencyProperty.Register("ShowTrackGenreGridItem", typeof(bool), typeof(TrackList), new PropertyMetadata(false, OnShowTrackGenreGridItemChanged));

        private bool ShowTrackReleaseYearGridItem
        {
            get { return (bool)GetValue(ShowTrackReleaseYearGridItemProperty); }
            set { SetValue(ShowTrackReleaseYearGridItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowTrackReleaseYearGridItemProperty.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty ShowTrackReleaseYearGridItemProperty =
            DependencyProperty.Register("ShowTrackReleaseYearGridItem", typeof(bool), typeof(TrackList), new PropertyMetadata(false, OnShowTrackReleaseYearGridItemChanged));

        private static void OnShowTrackReleaseYearGridItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TrackList).EvaluateTrackReleaseYearVisibility();
        }

        private static void OnShowTrackGenreGridItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TrackList).EvaluateTrackGenreVisibility();
        }

        private static void OnShowTrackNumberGridItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TrackList).EvaluateTrackNumberVisibility();
        }

        private static void OnShowTrackArtGridItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TrackList).EvaluateTrackArtVisibility();
        }

        private static void OnShowTrackAlbumGridItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TrackList).EvaluateTrackAlbumVisibility();
        }

        #endregion

        #region Grid Bound Column Visibility Properties

        private bool ShowArt
        {
            get { return (bool)GetValue(ShowArtProperty); }
            set { SetValue(ShowArtProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowArt.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty ShowArtProperty =
            DependencyProperty.Register("ShowArt", typeof(bool), typeof(TrackList), new PropertyMetadata(true));

        private bool ShowTrackNumbers
        {
            get { return (bool)GetValue(ShowTrackNumbersProperty); }
            set { SetValue(ShowTrackNumbersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowTrackNumbers.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty ShowTrackNumbersProperty =
            DependencyProperty.Register("ShowTrackNumbers", typeof(bool), typeof(TrackList), new PropertyMetadata(false));

        private bool ShowGenre
        {
            get { return (bool)GetValue(ShowGenreProperty); }
            set { SetValue(ShowGenreProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowGenre.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty ShowGenreProperty =
            DependencyProperty.Register("ShowGenre", typeof(bool), typeof(TrackList), new PropertyMetadata(false));

        private bool ShowReleaseYear
        {
            get { return (bool)GetValue(ShowReleaseYearProperty); }
            set { SetValue(ShowReleaseYearProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowReleaseYear.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty ShowReleaseYearProperty =
            DependencyProperty.Register("ShowReleaseYear", typeof(bool), typeof(TrackList), new PropertyMetadata(false));

        private bool ShowAlbum
        {
            get { return (bool)GetValue(ShowAlbumProperty); }
            set { SetValue(ShowAlbumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowAlbum.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty ShowAlbumProperty =
            DependencyProperty.Register("ShowAlbum", typeof(bool), typeof(TrackList), new PropertyMetadata(false));

        #endregion

        private void EvaluateTrackArtVisibility()
        {
            ShowArt = EnableArt && ShowTrackArtGridItem;
        }

        private void EvaluateTrackNumberVisibility()
        {
            ShowTrackNumbers = EnableTrackNumbers && ShowTrackNumberGridItem;
        }

        private void EvaluateTrackAlbumVisibility()
        {
            ShowAlbum = EnableAlbum && ShowTrackAlbumGridItem;
        }

        private void EvaluateTrackGenreVisibility()
        {
            ShowGenre = EnableGenre && ShowTrackGenreGridItem;
        }

        private void EvaluateTrackReleaseYearVisibility()
        {
            ShowReleaseYear = EnableReleaseYear && ShowTrackReleaseYearGridItem;
        }

        private async void TrackListView_ItemClick(object sender, ItemClickEventArgs args)
        {
            if (args.ClickedItem != null)
            {
                var index = Convert.ToUInt32(Items.IndexOf(args.ClickedItem as DriveItem));
                await Player.PlayAsync(Items, index);
            }
        }

        public async Task Play()
        {
            if (Items.Count > 0)
            {
                await Player.PlayAsync(Items, 0);
            }
        }

        private PlayerViewModel Player => ServiceLocator.Current.GetInstance<PlayerViewModel>();

        private MusicLibrary Library => ServiceLocator.Current.GetInstance<MusicLibrary>();

        private void TracksList_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.InRecycleQueue)
            {
                var templateRoot = args.ItemContainer.ContentTemplateRoot as FrameworkElement;
                var image = (Image)templateRoot.FindName("TrackArt");

                if (image != null)
                {
                    image.Source = null;
                }
            }

            if (args.Phase == 0)
            {
                args.RegisterUpdateCallback(2, LoadImageAsync);
                args.Handled = true;
            }
        }

        private async void LoadImageAsync(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.Phase == 2)
            {
                var templateRoot = args.ItemContainer.ContentTemplateRoot as FrameworkElement;
                var image = (Image)templateRoot.FindName("TrackArt");

                if (image != null)
                {
                    image.Opacity = 1;

                    var item = Items[args.ItemIndex] as DriveItem;
                    if (item != null)
                    {
                        await LoadArtAsync(image, item.Track);
                    }
                }
            }
        }

        private async Task<bool> LoadArtAsync(Image image, Track track)
        {
            bool loaded = false;
            if (Library.AlbumArts.Exists(track.Album.Id.Value))
            {
                using (var stream = Library.AlbumArts.Get(track.Album.Id.Value))
                {
                    using (var rtStream = stream.AsRandomAccessStream())
                    {
                        var bm = new BitmapImage();
                        await bm.SetSourceAsync(rtStream);
                        image.Source = bm;
                        loaded = true;
                    }
                }
            }

            return loaded;
        }
    }
}

using OnePlayer.Data;
using OnePlayer.Data.Access;
using OnePlayer.UWP.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace OnePlayer.UWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : NavViewPageBase, ISupportViewModel<MainViewModel>
    {
        private readonly IDictionary<string, Type> pages = new Dictionary<string, Type>()
        {
            { "home", typeof(HomePage) },
            { "library", typeof(MusicLibraryPage) },
            { "nowplaying", typeof(NowPlayingPage) },
            { "playlists", typeof(PlaylistsPage) },
            { "settings", typeof(SettingsPage) },
            { "syncstatus", typeof(SyncStatusPage) }
        };

        private static Dictionary<SearchItemType, string> searchItemDescriptionFormatMap = new Dictionary<SearchItemType, string>()
        {
            { SearchItemType.Album, "SearchItemAlbumDescriptionFormat" },
            { SearchItemType.Artist, "SearchItemArtistDescriptionFormat" },
            { SearchItemType.Genre, "SearchItemGenreDescriptionFormat" },
            { SearchItemType.Track, "SearchItemTrackDescriptionFormat" },
            { SearchItemType.TrackArtist, "SearchItemTrackDescriptionFormat" }
        };

        private static Dictionary<SearchItemType, string> searchItemIconMap = new Dictionary<SearchItemType, string>()
        {
            { SearchItemType.Album, "\uE93C" },
            { SearchItemType.Artist, "\uE8D4" },
            { SearchItemType.Genre, "\uE8D6" },
            { SearchItemType.Track, "\uEC4F" },
            { SearchItemType.TrackArtist, "\uEC4F" }
        };

        private Locator Locator => App.Current.Resources["VMLocator"] as Locator;
        public MainViewModel ViewModel => Locator.Main;

        public MainPage()
        {
            this.InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () => RootGrid.Opacity = 1);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            
            UpdateSyncStateIcon();
            await ViewModel.LoadAsync(VoidType.Empty);
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        private async void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.State))
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, UpdateSyncStateIcon);
            }
        }

        private void UpdateSyncStateIcon()
        {
            string glyph = string.Empty;
            switch (ViewModel.State)
            {
                case Sync.SyncState.NotStarted:
                    glyph = "\uEA6A";
                    break;
                case Sync.SyncState.Started:
                    glyph = "\uE895";
                    break;
                case Sync.SyncState.Syncing:
                    glyph = "\uE895";
                    break;
                case Sync.SyncState.NotSyncing:
                    glyph = "\uEA6A";
                    break;
                case Sync.SyncState.Uptodate:
                    glyph = "\uE930";
                    break;
                case Sync.SyncState.Stopped:
                    glyph = "\uE769";
                    break;
                default:
                    glyph = "\uE895";
                    break;
            }

            SyncStatusCommandBarIcon.Glyph = glyph;

            if (ViewModel.State == Sync.SyncState.Syncing)
            {
                SyncStatusCommandBarIconRotation.Begin();
            }
            else
            {
                SyncStatusCommandBarIconRotation.Stop();
            }
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            NavView.SelectedItem = NavView.MenuItems[1];
            NavView_Navigate(pages.First().Key, new EntranceNavigationTransitionInfo());
        }

        private void NavView_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            string tag = args.IsSettingsInvoked ? "settings" : (string)args.InvokedItemContainer?.Tag;
            NavView_Navigate(tag, args.RecommendedNavigationTransitionInfo);
        }

        private void NavView_Navigate(string tag, NavigationTransitionInfo transitionInfo)
        {
            if (string.IsNullOrEmpty(tag))
            {
                // Do nothing;
                return;
            }

            var item = pages.FirstOrDefault(p => p.Key.Equals(tag));
            var page = item.Value;

            // Get the page type before navigation so you can prevent duplicate
            // entries in the backstack.
            var currentPageType = ContentFrame.CurrentSourcePageType;
            if (page != currentPageType)
            {
                ContentFrame.Navigate(page, null, transitionInfo);
            }
        }

        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            var pageItem = pages.FirstOrDefault(p => p.Value == e.SourcePageType);

            if (e.SourcePageType == typeof(SettingsPage))
            {
                // Set name for the settings item
                var settingsMenuItem = (Microsoft.UI.Xaml.Controls.NavigationViewItem)NavView.SettingsItem;
                settingsMenuItem.Content = ResourceLoader.GetForCurrentView().GetString("NavViewSettingsContent");
                NavView.SelectedItem = settingsMenuItem;
            }
            else
            {
                if (pageItem.Key == null)
                {
                    pageItem = new KeyValuePair<string, Type>("library", typeof(MusicLibraryPage));
                }
                NavView.SelectedItem = NavView.MenuItems
                    .OfType<Microsoft.UI.Xaml.Controls.NavigationViewItem>()
                    .FirstOrDefault(n => n.Tag.Equals(pageItem.Key));
            }

            var headerText = ((Microsoft.UI.Xaml.Controls.NavigationViewItem)NavView.SelectedItem)?.Content?.ToString();
            // NavViewHeaderText.Text = headerText ?? string.Empty;
        }

        private void Titlebar_BackRequested(object sender, EventArgs e)
        {
            if (ContentFrame.CanGoBack)
            {
                ContentFrame.GoBack();
            }
        }

        private async void NavViewSearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                await ViewModel.SearchSuggestions.Load(sender.Text);
            }
        }

        private void ImageArt_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Debug.WriteLine("Failed");
        }

        public static string FormatSearchItemDescription(string description, SearchItemType type)
        {
            string format = searchItemDescriptionFormatMap[type];
            return string.Format(ResourceLoader.GetForCurrentView().GetString(format), description);
        }

        public static string FormatSearchItemIcon(SearchItemType type)
        {
            return searchItemIconMap[type];
        }

        private void NavViewSearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            sender.Text = "";
        }

        private void NavViewSearchBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            sender.Text = "";

            if (args.SelectedItem != null)
            {
                var item = args.SelectedItem as SearchItemViewModel;
                if (item.Type == SearchItemType.Album)
                {
                    var options = new AlbumAccessOptions()
                    {
                        IncludeArtist = true,
                        IncludeGenre = true,
                        AlbumFilter = item.Id
                    };
                    var album = Locator.MusicMetadata.Albums.Get(options).First();
                    ContentFrame.Navigate(typeof(AlbumPage), album, new EntranceNavigationTransitionInfo());
                }
            }
        }
    }
}

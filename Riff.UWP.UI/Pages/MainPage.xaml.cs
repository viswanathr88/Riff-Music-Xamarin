using CommonServiceLocator;
using Riff.Data;
using Riff.Data.Access;
using Riff.Sync;
using Riff.UWP.Storage;
using Riff.UWP.ViewModel;
using Riff.Extensions;
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
using System.Security.Cryptography;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Riff.UWP.Pages
{
    public class MainPageBase : PageBase<MainViewModel>
    {
    }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : MainPageBase, IShellPage, ISupportPlaying
    {
        private readonly IDictionary<string, Type> pages = new Dictionary<string, Type>()
        {
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

        public PlayerViewModel Player => ServiceLocator.Current.GetInstance<PlayerViewModel>();

        private IAppPreferences Preferences => ServiceLocator.Current.GetInstance<IAppPreferences>();

        private IMusicMetadata MusicMetadata => ServiceLocator.Current.GetInstance<IMusicMetadata>();

        public bool CanGoBack => ContentFrame.CanGoBack;

        public MainPage()
        {
            this.InitializeComponent();
            ViewModel.Sync.PropertyChanged += Sync_PropertyChanged;
            Preferences.Changed += Preferences_Changed;
            NavigationCacheMode = NavigationCacheMode.Required;
        }

        private async void Sync_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.Sync.State))
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, UpdateSyncStateIcon);
            }
        }

        private string GetSyncStatusText(SyncState state)
        {
            switch (state)
            {
                case SyncState.NotSyncing:
                    return Strings.Resources.SyncStatusNotSyncingText;
                case SyncState.Started:
                    return Strings.Resources.SyncStatusStartedText;
                case SyncState.Stopped:
                    return Strings.Resources.SyncStatusStoppedText;
                case SyncState.Syncing:
                    return Strings.Resources.SyncStatusSyncingText;
                case SyncState.Uptodate:
                    return Strings.Resources.SyncStatusUptodateText;
            }

            return "";
        }

        private void UpdateSyncStateIcon()
        {
            if (ViewModel.Sync.State == Sync.SyncState.Syncing)
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
            UpdateSyncStateIcon();
            NavView.SelectedItem = NavView.MenuItems[0];
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
                if (page == typeof(NowPlayingPage))
                {
                    Frame.Navigate(page, null, transitionInfo);
                }
                else
                {
                    ContentFrame.Navigate(page, null, transitionInfo);
                }
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
                settingsMenuItem.Content = Strings.Resources.NavViewSettingsContent;
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
            GoBack();
        }

        private async void NavViewSearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput && !string.IsNullOrEmpty(sender.Text))
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
            string value = string.Empty;
            switch (type)
            {
                case SearchItemType.Album:
                    value = string.Format(Strings.Resources.SearchItemAlbumDescriptionFormat, description ?? Strings.Resources.UnknownArtistText);
                    break;
                case SearchItemType.Artist:
                    value = string.Format(Strings.Resources.SearchItemArtistDescriptionFormat, description);
                    break;
                case SearchItemType.Genre:
                    value = string.Format(Strings.Resources.SearchItemGenreDescriptionFormat, description);
                    break;
                case SearchItemType.Track:
                case SearchItemType.TrackArtist:
                    value = string.Format(Strings.Resources.SearchItemTrackDescriptionFormat, description ?? Strings.Resources.UnknownArtistText);
                    break;
                default:
                    break;
            }
            return value;
        }

        public static string FormatSearchItemIcon(SearchItemType type)
        {
            return searchItemIconMap[type];
        }

        private async void NavViewSearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            NavView.IsPaneOpen = false;

            if (args.ChosenSuggestion != null)
            {
                var item = args.ChosenSuggestion as SearchItemViewModel;
                if (item.Type == SearchItemType.Album)
                {
                    var options = new AlbumAccessOptions()
                    {
                        IncludeArtist = true,
                        IncludeGenre = true,
                        AlbumFilter = item.Id
                    };
                    var album = MusicMetadata.Albums.Get(options).First();
                    ContentFrame.Navigate(typeof(AlbumPage), album, new EntranceNavigationTransitionInfo());
                }
                else if (item.Type == SearchItemType.Artist)
                {
                    var artist = MusicMetadata.Artists.Get(item.Id);
                    ContentFrame.Navigate(typeof(ArtistPage), artist, new EntranceNavigationTransitionInfo());
                }
                else if (item.Type == SearchItemType.Track || item.Type == SearchItemType.TrackArtist)
                {
                    var options = new DriveItemAccessOptions()
                    {
                        SortType = TrackSortType.Number,
                        SortOrder = SortOrder.Ascending,
                        IncludeTrack = true,
                        IncludeTrackAlbum = true,
                        AlbumFilter = item.ParentId
                    };

                    var items = MusicMetadata.DriveItems.Get(options);
                    var index = items.IndexOf(driveItem => driveItem.Track.Id == item.Id);
                    if (index >= 0)
                    {
                        await Player.PlayAsync(items, Convert.ToUInt32(index));
                    }
                    sender.Text = "";
                }
            }
        }

        private void Preferences_Changed(object sender, string preference)
        {
            if (preference == nameof(IAppPreferences.AppTheme))
            {
                Device.UpdateTheme(Preferences.AppTheme);
            }
        }

        private void NavViewSyncStatusFooter_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Flyout.ShowAttachedFlyout(sender as FrameworkElement);
        }

        private void SyncStatusFlyout_Opening(object sender, object e)
        {
            UpdateSyncStatusFlyout();
        }

        private void UpdateSyncStatusFlyout()
        {
            SyncStatusIcon.Glyph = SyncStatusCommandBarIcon.Glyph;
        }

        private void MediaPlayerControl_Loaded(object sender, RoutedEventArgs e)
        {
            MediaPlayerControl.SetMediaPlayer(Player.MediaPlayer);
        }

        public void GoBack()
        {
            if (ContentFrame.CanGoBack)
            {
                ContentFrame.GoBack();
            }
        }
    }
}

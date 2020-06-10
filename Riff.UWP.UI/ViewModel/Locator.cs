using Riff.Authentication;
using Riff.Data;
using Riff.Data.Sqlite;
using Riff.Sync;
using Riff.UWP.Authentication;
using Riff.UWP.Storage;
using System;
using System.IO;
using System.Net.Http;
using Windows.ApplicationModel.Resources;
using Windows.Storage;

namespace Riff.UWP.ViewModel
{
    public sealed class Locator
    {
        private readonly Lazy<IAppPreferences> preferences;

        private readonly Lazy<MusicLibraryViewModel> musicLibraryVM;
        private readonly Lazy<FirstRunExperienceViewModel> firstRunExperienceVM;
        private readonly Lazy<SettingsViewModel> settingsVM;
        private readonly Lazy<MainViewModel> mainVM;
        private readonly Lazy<PlayerViewModel> playerVM;
        
        private readonly Lazy<ILoginManager> loginManager;
        private readonly Lazy<HttpClient> httpClient;
        private readonly Lazy<SyncEngine> syncEngine;
        private readonly Lazy<MusicLibrary> musicLibrary;
        private readonly Lazy<IMusicMetadata> musicMetadata;

        private readonly string DefaultPath = ApplicationData.Current.LocalCacheFolder.Path;

        public Locator()
        {
            preferences = new Lazy<IAppPreferences>(() => new AppPreferences());
            httpClient = new Lazy<HttpClient>(() => new HttpClient(new HttpClientHandler() { AllowAutoRedirect = false }));
            loginManager = new Lazy<ILoginManager>(() => new CacheReadyLoginManager(new WindowsLoginManager(WebClient, ResourceLoader.GetForCurrentView().GetString("LoginDescription")), DefaultPath));
            syncEngine = new Lazy<SyncEngine>(() => new SyncEngine(new AppPreferences(), LoginManager, WebClient, Library));
            musicMetadata = new Lazy<IMusicMetadata>(() => new MusicMetadata(Path.Combine(DefaultPath, "Riff.db")));
            musicLibrary = new Lazy<MusicLibrary>(() => new MusicLibrary(DefaultPath, MusicMetadata, WebClient));

            mainVM = new Lazy<MainViewModel>(() => new MainViewModel(Library, SyncEngine, Preferences));
            musicLibraryVM = new Lazy<MusicLibraryViewModel>(() => new MusicLibraryViewModel(Library));
            settingsVM = new Lazy<SettingsViewModel>(() => new SettingsViewModel(LoginManager, Preferences));
            firstRunExperienceVM = new Lazy<FirstRunExperienceViewModel>(() => new FirstRunExperienceViewModel(loginManager.Value));
            playerVM = new Lazy<PlayerViewModel>(() => new PlayerViewModel(Library, SyncEngine));
        }

        public MainViewModel Main => mainVM.Value;
        
        public MusicLibraryViewModel MusicLibrary => musicLibraryVM.Value;

        public FirstRunExperienceViewModel FirstRunExperience => firstRunExperienceVM.Value;

        public SettingsViewModel Settings => settingsVM.Value;

        public PlayerViewModel Player => playerVM.Value;

        public ILoginManager LoginManager => loginManager.Value;

        private HttpClient WebClient => httpClient.Value;

        public IMusicMetadata MusicMetadata => musicMetadata.Value;

        public IAppPreferences Preferences => preferences.Value;

        public MusicLibrary Library => musicLibrary.Value;

        private SyncEngine SyncEngine => syncEngine.Value;

    }
}

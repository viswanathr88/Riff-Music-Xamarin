using OnePlayer.Authentication;
using OnePlayer.Data;
using OnePlayer.Sync;
using OnePlayer.UWP.Storage;
using System;
using System.Net.Http;
using Windows.Storage;

namespace OnePlayer.UWP.ViewModel
{
    public sealed class Locator
    {
        private readonly Lazy<MusicLibraryViewModel> musicLibraryVM;
        private readonly Lazy<FirstRunExperienceViewModel> firstRunExperienceVM;
        private readonly Lazy<SettingsViewModel> settingsVM;
        private readonly Lazy<MainViewModel> mainVM;
        
        private readonly Lazy<ILoginManager> loginManager;
        private readonly Lazy<HttpClient> httpClient;
        private readonly Lazy<SyncEngine> syncEngine;
        private readonly Lazy<MusicLibrary> musicLibrary;

        private readonly string DefaultPath = ApplicationData.Current.LocalCacheFolder.Path;

        public Locator()
        {
            httpClient = new Lazy<HttpClient>(() => new HttpClient());
            loginManager = new Lazy<ILoginManager>(() => new CacheReadyLoginManager(new OnePlayer.UWP.Authentication.WindowsLoginManager(WebClient), DefaultPath));
            syncEngine = new Lazy<SyncEngine>(() => new SyncEngine(new AppPreferences(), LoginManager, WebClient, Library));
            musicLibrary = new Lazy<MusicLibrary>(() => new MusicLibrary(new MusicDataStore(DefaultPath), WebClient));

            mainVM = new Lazy<MainViewModel>(() => new MainViewModel(Library, SyncEngine));
            musicLibraryVM = new Lazy<MusicLibraryViewModel>();
            settingsVM = new Lazy<SettingsViewModel>(() => new SettingsViewModel(loginManager.Value));
            firstRunExperienceVM = new Lazy<FirstRunExperienceViewModel>(() => new FirstRunExperienceViewModel(loginManager.Value));
        }

        public MainViewModel Main => mainVM.Value;
        
        public MusicLibraryViewModel MusicLibrary => musicLibraryVM.Value;

        public FirstRunExperienceViewModel FirstRunExperience => firstRunExperienceVM.Value;

        public SettingsViewModel Settings => settingsVM.Value;

        private HttpClient WebClient => httpClient.Value;

        private MusicLibrary Library => musicLibrary.Value;

        private SyncEngine SyncEngine => syncEngine.Value;

        public ILoginManager LoginManager => loginManager.Value;
    }
}

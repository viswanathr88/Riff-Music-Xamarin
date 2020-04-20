using OnePlayer.Authentication;
using System;
using System.Net.Http;
using Windows.Storage;

namespace OnePlayer.UWP.ViewModel
{
    public sealed class Locator
    {
        private readonly Lazy<MusicLibraryViewModel> musicLibraryVM;
        private readonly Lazy<FirstRunExperienceViewModel> firstRunExperienceVM;
        
        private readonly Lazy<ILoginManager> loginManager;
        private readonly Lazy<HttpClient> httpClient;

        private readonly string DefaultPath = ApplicationData.Current.LocalCacheFolder.Path;

        public Locator()
        {
            httpClient = new Lazy<HttpClient>(() => new HttpClient());
            musicLibraryVM = new Lazy<MusicLibraryViewModel>();
            loginManager = new Lazy<ILoginManager>(() => new CacheReadyLoginManager(new OnePlayer.UWP.Authentication.LoginManager(WebClient), DefaultPath));
            firstRunExperienceVM = new Lazy<FirstRunExperienceViewModel>(() => new FirstRunExperienceViewModel(loginManager.Value));
        }
        
        public MusicLibraryViewModel MusicLibrary => musicLibraryVM.Value;

        public FirstRunExperienceViewModel FirstRunExperience => firstRunExperienceVM.Value;

        private HttpClient WebClient => httpClient.Value;

        public ILoginManager LoginManager => loginManager.Value;
    }
}

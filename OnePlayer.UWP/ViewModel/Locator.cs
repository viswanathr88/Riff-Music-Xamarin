using OnePlayer.Authentication;
using OnePlayer.Data;
using System;
using System.Net.Http;

namespace OnePlayer.UWP.ViewModel
{
    public sealed class Locator
    {
        private Lazy<MusicLibraryViewModel> musicLibraryVM;
        private Lazy<FirstRunExperienceViewModel> firstRunExperienceVM;
        
        private Lazy<ILoginManager> loginManager;
        private Lazy<HttpClient> httpClient;

        public Locator()
        {
            httpClient = new Lazy<HttpClient>(() => new HttpClient());
            musicLibraryVM = new Lazy<MusicLibraryViewModel>();
            loginManager = new Lazy<ILoginManager>(() => new OnePlayer.UWP.Authentication.LoginManager(WebClient));
            firstRunExperienceVM = new Lazy<FirstRunExperienceViewModel>(() => new FirstRunExperienceViewModel(loginManager.Value));
        }
        
        public MusicLibraryViewModel MusicLibrary => musicLibraryVM.Value;

        public FirstRunExperienceViewModel FirstRunExperience => firstRunExperienceVM.Value;

        private HttpClient WebClient => httpClient.Value;
    }
}

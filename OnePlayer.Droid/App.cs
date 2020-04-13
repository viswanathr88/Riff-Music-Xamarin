using Android.App;
using Android.Content;
using OnePlayer.Authentication;
using OnePlayer.Data;
using OnePlayer.Droid.Storage;
using OnePlayer.Sync;
using System;
using System.Net.Http;

namespace OnePlayer.Droid
{
    [Application]
    public class App : Android.App.Application, IOnePlayerApp
    {
        private LoginManager loginManager;
        private ITokenCache tokenCache;
        private IPreferences appPreferences;
        private MusicLibrary musicLibrary;
        private SyncEngine syncEngine;
        private const string tokenCachePreferenceFile = "com.oneplayer.droid.tokencache.preferences";
        private const string appPreferenceFile = "com.oneplayer.droid.app.preferences";
        private readonly static HttpClient httpClient = new HttpClient();

        public App(IntPtr javaReference, Android.Runtime.JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
        }


        public LoginManager LoginManager
        {
            get
            {
                if (this.loginManager == null)
                {
                    this.loginManager = new LoginManager(httpClient, TokenCache);
                }

                return this.loginManager;
            }
        }

        public HttpClient WebClient => httpClient;

        private ITokenCache TokenCache
        {
            get
            {
                if (tokenCache == null)
                {
                    this.tokenCache = new TokenCache(GetSharedPreferences(tokenCachePreferenceFile, FileCreationMode.Private));
                }

                return this.tokenCache;
            }
        }

        public IPreferences Preferences
        {
            get
            {
                if (appPreferences == null)
                {
                    appPreferences = new AppPreferences(GetSharedPreferences(appPreferenceFile, FileCreationMode.Private));
                }

                return appPreferences;
            }
        }

        public MusicLibrary MusicLibrary
        {
            get
            {
                if (musicLibrary == null)
                {
                    musicLibrary = new MusicLibrary(WebClient);
                }

                return musicLibrary;
            }
        }

        public SyncEngine SyncEngine
        {
            get
            {
                if (syncEngine == null)
                {
                    syncEngine = new SyncEngine(Preferences, LoginManager, WebClient, MusicLibrary);
                }

                return syncEngine;
            }
        }
    }
}
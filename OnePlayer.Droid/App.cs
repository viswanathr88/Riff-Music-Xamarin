using Android.App;
using Android.Content;
using OnePlayer.Authentication;
using OnePlayer.Data;
using OnePlayer.Droid.Storage;
using OnePlayer.Sync;
using System;
using System.IO;
using System.Net.Http;

namespace OnePlayer.Droid
{
    [Application]
    public class App : Android.App.Application, IOnePlayerApp
    {
        private ILoginManager loginManager;
        private ITokenCache tokenCache;
        private IProfileCache profileCache;
        private IPreferences appPreferences;
        private MusicLibrary musicLibrary;
        private SyncEngine syncEngine;
        private const string tokenCachePreferenceFile = "com.oneplayer.droid.tokencache.preferences";
        private const string appPreferenceFile = "com.oneplayer.droid.app.preferences";
        private readonly static HttpClient httpClient = new HttpClient();
        private static readonly string DefaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal));

        public App(IntPtr javaReference, Android.Runtime.JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

        }


        public ILoginManager LoginManager
        {
            get
            {
                if (this.loginManager == null)
                {
                    this.loginManager = new CacheReadyLoginManager(httpClient, TokenCache, ProfileCache);
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

        private IProfileCache ProfileCache
        {
            get
            {
                if (profileCache == null)
                {
                    profileCache = new ProfileCache(DefaultPath);
                }

                return profileCache;
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
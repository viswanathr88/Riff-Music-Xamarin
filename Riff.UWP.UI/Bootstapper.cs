﻿using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using Riff.Authentication;
using Riff.Data;
using Riff.Data.Sqlite;
using Riff.Sync;
using Riff.UWP.Authentication;
using Riff.UWP.Storage;
using Riff.UWP.ViewModel;
using System.IO;
using System.Net.Http;
using Windows.ApplicationModel.Resources;
using Windows.Storage;

namespace Riff.UWP
{
    public class Bootstapper
    {
        private static readonly string DefaultPath = ApplicationData.Current.LocalCacheFolder.Path;
        private static readonly string DatabaseName = "Riff.db";
        private static readonly string LoginDescriptionKey = "LoginDescription";

        public Bootstapper()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            // Setup core objects in IOC first
            SimpleIoc.Default.Register<AppPreferences>();
            SimpleIoc.Default.Register<IPreferences>(() => SimpleIoc.Default.GetInstance<AppPreferences>());
            SimpleIoc.Default.Register<IAppPreferences>(() => SimpleIoc.Default.GetInstance<AppPreferences>());
            SimpleIoc.Default.Register(() => new HttpClient(new HttpClientHandler() { AllowAutoRedirect = false }));

            var loginManager = new WindowsLoginManager(SimpleIoc.Default.GetInstance<HttpClient>(), ResourceLoader.GetForCurrentView().GetString(LoginDescriptionKey));
            SimpleIoc.Default.Register<ILoginManager>(() => new CacheReadyLoginManager(loginManager, DefaultPath));

            SimpleIoc.Default.Register<IMusicMetadata>(() => new MusicMetadata(Path.Combine(DefaultPath, DatabaseName)));
            SimpleIoc.Default.Register(() => new MusicLibrary(DefaultPath, SimpleIoc.Default.GetInstance<IMusicMetadata>()));
            SimpleIoc.Default.Register<SyncEngine>();

            // Setup all ViewModels
            SimpleIoc.Default.Register<FirstRunExperienceViewModel>();
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<MusicLibraryViewModel>();
            SimpleIoc.Default.Register<AlbumsViewModel>();
            SimpleIoc.Default.Register<ArtistsViewModel>();
            SimpleIoc.Default.Register<TracksViewModel>();
            SimpleIoc.Default.Register<PlaylistsViewModel>();
            SimpleIoc.Default.Register<PlayerViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<AlbumViewModel>();
            SimpleIoc.Default.Register<ArtistViewModel>();
        }
    }
}
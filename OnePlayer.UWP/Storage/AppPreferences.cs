using OnePlayer.Data;
using Windows.Storage;

namespace OnePlayer.UWP.Storage
{
    sealed class AppPreferences : IPreferences
    {
        private readonly string deltaUrlKey = "DeltaUrl";
        private readonly string lastSyncTimeKey = "LastSyncTime";
        private readonly string isSyncPausedKey = "IsSyncPaused";

        public string DeltaUrl 
        {
            get => ApplicationData.Current.LocalSettings.Values[deltaUrlKey]?.ToString();
            set => ApplicationData.Current.LocalSettings.Values[deltaUrlKey] = value;
        }
        
        public string LastSyncTime 
        {
            get => ApplicationData.Current.LocalSettings.Values[lastSyncTimeKey]?.ToString();
            set => ApplicationData.Current.LocalSettings.Values[lastSyncTimeKey] = value;
        }
        public bool IsSyncPaused 
        {
            get => bool.Parse(ApplicationData.Current.LocalSettings.Values[isSyncPausedKey]?.ToString() ?? "false");
            set => ApplicationData.Current.LocalSettings.Values[isSyncPausedKey] = value;
        }
    }
}

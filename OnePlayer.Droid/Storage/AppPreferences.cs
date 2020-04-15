using Android.Content;
using Java.Interop;
using System;

namespace OnePlayer.Droid.Storage
{
    sealed class AppPreferences : Data.IPreferences
    {
        private readonly ISharedPreferences sharedPreferences;
        private const string deltaUrlKey = "DeltaUrl";
        private const string isSyncPausedKey = "IsSyncPaused";
        private const string lastSyncTimeKey = "LastSyncTime";


        public AppPreferences(ISharedPreferences sharedPreferences)
        {
            this.sharedPreferences = sharedPreferences;
        }

        public string DeltaUrl
        {
            get => sharedPreferences.GetString(deltaUrlKey, null);
            set => sharedPreferences.Edit().PutString(deltaUrlKey, value).Apply();
        }
        public string LastSyncTime 
        {
            get => sharedPreferences.GetString(lastSyncTimeKey, DateTime.MinValue.ToString());
            set => sharedPreferences.Edit().PutString(lastSyncTimeKey, value).Apply();
        }
        public bool IsSyncPaused 
        {
            get => sharedPreferences.GetBoolean(isSyncPausedKey, false);
            set => sharedPreferences.Edit().PutBoolean(isSyncPausedKey, value).Apply();
        }
    }
}
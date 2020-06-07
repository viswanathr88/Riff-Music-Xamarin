using Android.Content;
using System;

namespace Riff.Droid.Storage
{
    sealed class AppPreferences : Data.IPreferences
    {
        private readonly ISharedPreferences sharedPreferences;
        private const string deltaUrlKey = "DeltaUrl";
        private const string isSyncPausedKey = "IsSyncPaused";
        private const string lastSyncTimeKey = "LastSyncTime";

        public event EventHandler<string> Changed;

        public AppPreferences(ISharedPreferences sharedPreferences)
        {
            this.sharedPreferences = sharedPreferences;
        }

        public string DeltaUrl
        {
            get => sharedPreferences.GetString(deltaUrlKey, null);
            set
            {
                sharedPreferences.Edit().PutString(deltaUrlKey, value).Apply();
                Changed?.Invoke(this, nameof(DeltaUrl));
            }
        }
        public string LastSyncTime 
        {
            get => sharedPreferences.GetString(lastSyncTimeKey, DateTime.MinValue.ToString());
            set
            {
                sharedPreferences.Edit().PutString(lastSyncTimeKey, value).Apply();
                Changed?.Invoke(this, nameof(LastSyncTime));
            }
        }
        public bool IsSyncPaused 
        {
            get => sharedPreferences.GetBoolean(isSyncPausedKey, false);
            set
            {
                sharedPreferences.Edit().PutBoolean(isSyncPausedKey, value).Apply();
                Changed?.Invoke(this, nameof(IsSyncPaused));
            }
        }
    }
}
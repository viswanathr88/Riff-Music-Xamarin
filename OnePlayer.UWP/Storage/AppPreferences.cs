using OnePlayer.Data;
using System;
using System.Runtime.CompilerServices;
using Windows.Storage;
using Windows.UI.Text.Core;

namespace OnePlayer.UWP.Storage
{
    sealed class AppPreferences : IAppPreferences
    {
        private readonly string deltaUrlKey = "DeltaUrl";
        private readonly string lastSyncTimeKey = "LastSyncTime";
        private readonly string isSyncPausedKey = "IsSyncPaused";
        private readonly string themeKey = "AppTheme";

        public string DeltaUrl 
        {
            get => GetPreference(deltaUrlKey);
            set => SetPreference(deltaUrlKey, value);
        }
        
        public string LastSyncTime 
        {
            get => GetPreference(lastSyncTimeKey);
            set => SetPreference(lastSyncTimeKey, value);
        }
        public bool IsSyncPaused 
        {
            get => bool.Parse(GetPreference(isSyncPausedKey) ?? "false");
            set => SetPreference(isSyncPausedKey, value.ToString());
        }
        public Theme AppTheme 
        {
            get => (Theme)System.Enum.Parse(typeof(Theme), GetPreference(themeKey) ?? Theme.Default.ToString());
            set => SetPreference(themeKey, value.ToString());
        }

        public event EventHandler<string> Changed;

        private string GetPreference(string key)
        {
            return ApplicationData.Current.LocalSettings.Values[key]?.ToString();
        }

        private bool SetPreference(string key, string value, [CallerMemberName]string member = "")
        {
            bool changed = false;
            if (ApplicationData.Current.LocalSettings.Values[key]?.ToString() != value)
            {
                ApplicationData.Current.LocalSettings.Values[key] = value.ToString();
                changed = true;
                Changed?.Invoke(this, member);
            }

            return changed;
        }
    }
}

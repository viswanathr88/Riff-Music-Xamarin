using System;

namespace Riff.Data
{
    public interface IPreferences
    {
        string DeltaUrl { get; set; }

        string LastSyncTime { get; set; }

        bool IsSyncPaused { get; set; }

        event EventHandler<string> Changed;
    }
}
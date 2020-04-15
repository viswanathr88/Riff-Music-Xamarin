using OnePlayer.Authentication;
using OnePlayer.Data;
using OnePlayer.Sync;
using System.Net.Http;

namespace OnePlayer.Droid
{
    public interface IOnePlayerApp
    {
        ILoginManager LoginManager { get; }

        HttpClient WebClient { get; }

        MusicLibrary MusicLibrary { get; }

        Data.IPreferences Preferences { get; }

        SyncEngine SyncEngine { get; }

    }
}
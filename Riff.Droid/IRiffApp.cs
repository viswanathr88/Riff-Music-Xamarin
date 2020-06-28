using Riff.Authentication;
using Riff.Data;
using Riff.Sync;
using System.Net.Http;

namespace Riff.Droid
{
    public interface IRiffApp
    {
        ILoginManager LoginManager { get; }

        HttpClient WebClient { get; }

        IMusicLibrary MusicLibrary { get; }

        Data.IPreferences Preferences { get; }

        SyncEngine SyncEngine { get; }

    }
}
using OnePlayer.Authentication;
using OnePlayer.Data;
using System.Net.Http;

namespace OnePlayer.Droid
{
    public interface IOnePlayerApp
    {
        LoginManager LoginManager { get; }

        HttpClient WebClient { get; }

        MusicLibrary MusicLibrary { get; }

        Data.IPreferences Preferences { get; }

    }
}
using Newtonsoft.Json;
using OnePlayer.Authentication;
using OnePlayer.Data;
using OnePlayer.Data.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace OnePlayer.Sync
{
    public enum SyncJobState
    {
        Running,
        NotRunning
    }
    public sealed class OneDriveMusicSyncJob
    {
        private readonly string baseUrl = "https://graph.microsoft.com/v1.0/drive/special/music/delta";
        private SyncJobState state = SyncJobState.NotRunning;
        private readonly IPreferences preferences;
        private readonly LoginManager loginManager;
        private readonly HttpClient webClient;
        private readonly MusicLibrary library;

        public OneDriveMusicSyncJob(IPreferences preferences, LoginManager loginManager, HttpClient webClient, MusicLibrary library)
        {
            this.preferences = preferences;
            this.loginManager = loginManager;
            this.webClient = webClient;
            this.library = library;
        }
        public async Task RunAsync()
        {
            if (IsSyncing)
            {
                // We are already syncing.
                return;
            }

            state = SyncJobState.Running;

            string deltaUrl = string.Empty;
            var token = await loginManager.AcquireTokenSilentAsync();
            bool completed = false;
            var editor = library.Edit();

            do
            {
                if (deltaUrl == string.Empty)
                {
                    // Read delta url from a previous sync
                    deltaUrl = preferences.DeltaUrl ?? baseUrl;
                }

                var request = new HttpRequestMessage(HttpMethod.Get, deltaUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

                var response = await webClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                if (response.IsSuccessStatusCode)
                {
                    DeltaQueryResponse deltaQueryResponse = null;
                    using (var responseStream = await response.Content.ReadAsStreamAsync())
                    {
                        using (var streamReader = new System.IO.StreamReader(responseStream))
                        {
                            using (var jsonReader = new JsonTextReader(streamReader))
                            {
                                var serializer = new JsonSerializer();
                                deltaQueryResponse = serializer.Deserialize<DeltaQueryResponse>(jsonReader);
                            }
                        }
                    }

                    editor.AddRange(deltaQueryResponse.value);

                    if (!string.IsNullOrEmpty(deltaQueryResponse.nextLink))
                    {
                        deltaUrl = deltaQueryResponse.nextLink;
                    }
                    else if (!string.IsNullOrEmpty(deltaQueryResponse.deltaLink))
                    {
                        deltaUrl = deltaQueryResponse.deltaLink;
                        completed = true;
                    }

                    preferences.DeltaUrl = deltaUrl;
                }
                else
                {
                    // Read the error message
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == System.Net.HttpStatusCode.Gone)
                    {
                        // Look for the location header
                        deltaUrl = response.Headers.Location.ToString();
                    }
                }
            }
            while (!completed);

            state = SyncJobState.NotRunning;
        }

        public bool IsSyncing => state == SyncJobState.Running;

    }
}

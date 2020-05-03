using Newtonsoft.Json;
using OnePlayer.Authentication;
using OnePlayer.Data;
using OnePlayer.Data.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace OnePlayer.Sync
{
    public enum SyncState
    {
        NotStarted,
        Started,
        Syncing,
        NotSyncing,
        Uptodate,
        Stopped
    }

    public sealed class SyncStatus
    {
        public DateTime SuccessfulSyncTime
        {
            get;
            set;
        }

        public DateTime SyncTime
        {
            get;
            set;
        }

        public Exception Error
        {
            get;
            set;
        }

        public SyncState State
        {
            get;
            set;
        }

        public int ItemsAdded
        {
            get;
            set;
        } = 0;
    }

    public sealed class SyncEngine
    {
        public event EventHandler<SyncState> StateChanged;
        public event EventHandler<SyncStatus> Checkpoint;
        private SyncState state = SyncState.NotStarted;
        private readonly string baseUrl = "https://graph.microsoft.com/v1.0/drive/special/music/delta?$expand=thumbnails";
        private readonly IPreferences preferences;
        private readonly ILoginManager loginManager;
        private readonly HttpClient webClient;
        private readonly MusicLibrary library;

        public SyncEngine(IPreferences preferences, ILoginManager loginManager, HttpClient webClient, MusicLibrary library)
        {
            this.preferences = preferences;
            this.loginManager = loginManager;
            this.webClient = webClient;
            this.library = library;
            this.library.ItemAdded += Library_ItemAdded;

            if (!string.IsNullOrEmpty(preferences.LastSyncTime))
            {
                Status.SuccessfulSyncTime = DateTime.Parse(preferences.LastSyncTime);
            }

            Start();
        }

        private void Library_ItemAdded(object sender, Data.DriveItem e)
        {
            Status.ItemsAdded++;
        }

        public SyncState State
        {
            get
            {
                return this.state;
            }
            private set
            {
                if (this.state != value)
                {
                    this.state = value;
                    Status.State = this.state;
                    StateChanged?.Invoke(this, this.state);
                }
            }
        }

        public SyncStatus Status
        {
            get;
            private set;
        } = new SyncStatus();

        public void Stop()
        {
            if (State != SyncState.Stopped)
            {
                // TODO: Kill any sync sessions

                State = SyncState.Stopped;
                preferences.IsSyncPaused = true;
            }
        }

        public void Start()
        {
            if (preferences.IsSyncPaused)
            {
                State = SyncState.Stopped;
            } 
            else if (State == SyncState.NotStarted || State == SyncState.Stopped)
            {
                State = SyncState.Started;
            }
        }

        public async Task RunAsync()
        {
            try
            {
                Status.SyncTime = DateTime.Now;
                Status.ItemsAdded = 0;
                Status.Error = null;

                await RunInternalAsync();
            }
            catch (Exception ex)
            {
                Status.Error = ex;
                State = SyncState.NotSyncing;
            }
        }

        private async Task RunInternalAsync()
        {
            if (State == SyncState.Syncing || State == SyncState.Stopped)
            {
                // We are already syncing.
                return;
            }

            State = SyncState.Syncing;

            // Fire a checkpoint event
            Checkpoint?.Invoke(this, Status);

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

                    editor.Add(deltaQueryResponse.value);

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

                    // Download thumbnails
                    await editor.DownloadThumbnailsAsync();

                    if (completed)
                    {
                        DateTime successTime = DateTime.Now;
                        Status.SuccessfulSyncTime = successTime;
                        preferences.LastSyncTime = successTime.ToString();
                        State = SyncState.Uptodate;
                    }

                    // Fire a checkpoint event
                    Checkpoint?.Invoke(this, Status);
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
        }
    }
}

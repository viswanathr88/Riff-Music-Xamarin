using Newtonsoft.Json;
using OnePlayer.Authentication;
using OnePlayer.Data;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web.Core;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.UI.ApplicationSettings;

namespace OnePlayer.UWP.Authentication
{
    public sealed class LoginProfile
    {
        public LoginProfile(WebAccountProvider provider, WebAccount account)
        {
            Provider = provider;
            Account = account;
        }

        public WebAccountProvider Provider 
        {
            get;
            private set;
        }

        public WebAccount Account
        {
            get;
            private set;
        }
    }

    public sealed class WindowsLoginManager : ILoginManager
    {
        private const string appId = "19b11b92-7fc8-44c9-b794-8ae1d41cebed";
        private static readonly string[] scopes = new string[] { "User.Read", "files.read", "offline_access" };
        private const string authorizeUri = @"https://login.microsoft.com";
        private const string profileUri = @"https://graph.microsoft.com/V1.0/me/";
        private const string profilePhotoUri = @"https://graph.microsoft.com/beta/me/photo/$value";
        private const string currentProviderIdKey = "CurrentUserProviderId";
        private const string currentUserIdKey = "CurrentUserId";
        private bool webCommandInvoked = false;

        private readonly HttpClient webClient;

        private event EventHandler<Token> LoginCompleted;

        public WindowsLoginManager(HttpClient client, string headerText)
        {
            this.webClient = client ?? throw new ArgumentNullException(nameof(client));
            HeaderText = headerText;
        }

        public string HeaderText { get; private set; }

        public async Task<Token> AcquireTokenSilentAsync()
        {
            var profile = await GetLoginProfileAsync();

            var request = new WebTokenRequest(profile.Provider, string.Join(" ", scopes));
            var result = await WebAuthenticationCoreManager.GetTokenSilentlyAsync(request, profile.Account);

            if (result.ResponseStatus == WebTokenRequestStatus.Success)
            {
                return new Token() { AccessToken = result.ResponseData[0].Token };
            }
            else
            {
                return null;
            }
        }

        public async Task<Token> LoginAsync(object data)
        {
            AccountsSettingsPane.GetForCurrentView().AccountCommandsRequested += BuildPaneAsync;
            await AccountsSettingsPane.ShowAddAccountAsync();

            if (webCommandInvoked)
            {
                // Reset flag
                webCommandInvoked = false;

                TaskCompletionSource<Token> tcs = new TaskCompletionSource<Token>();

                void callback(object sender, Token token)
                {
                    LoginCompleted -= callback;
                    AccountsSettingsPane.GetForCurrentView().AccountCommandsRequested -= BuildPaneAsync;
                    tcs.SetResult(token);
                }

                LoginCompleted += callback;
                return await tcs.Task;
            }
            else
            {
                AccountsSettingsPane.GetForCurrentView().AccountCommandsRequested -= BuildPaneAsync;
            }

            return null;
        }

        public string GetAuthorizeUrl()
        {
            return authorizeUri;
        }

        public string GetRedirectUrl()
        {
            throw new NotSupportedException("Redirect is not support by Windows Login Manager");
        }

        public async Task<UserProfile> GetUserAsync()
        {
            var token = await AcquireTokenSilentAsync();

            var request = new HttpRequestMessage(HttpMethod.Get, profileUri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

            var response = await this.webClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            string body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserProfile>(body);
        }

        public async Task<Stream> GetUserPhotoAsync()
        {
            var token = await AcquireTokenSilentAsync();

            var request = new HttpRequestMessage(HttpMethod.Get, profilePhotoUri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

            var response = await this.webClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStreamAsync();
        }

        public async Task<bool> LoginExistsAsync()
        {
            bool loginExists = false;
            try
            {
                var loginProfile = await GetLoginProfileAsync();
                loginExists = (loginProfile != null);
            }
            catch (Exception)
            {
            }

            return loginExists;
        }

        private async Task<LoginProfile> GetLoginProfileAsync()
        {
            string providerId = ApplicationData.Current.LocalSettings.Values["CurrentUserProviderId"]?.ToString();
            string accountId = ApplicationData.Current.LocalSettings.Values["CurrentUserId"]?.ToString();

            if (string.IsNullOrEmpty(providerId) || string.IsNullOrEmpty(accountId))
            {
                throw new NotSupportedException("Account is not available");
            }

            var provider = await WebAuthenticationCoreManager.FindAccountProviderAsync(providerId);
            var account = await WebAuthenticationCoreManager.FindAccountAsync(provider, accountId);
            return new LoginProfile(provider, account);
        }

        private void StoreLoginProfile(LoginProfile profile)
        {
            ApplicationData.Current.LocalSettings.Values[currentProviderIdKey] = profile.Provider.Id;
            ApplicationData.Current.LocalSettings.Values[currentUserIdKey] = profile.Account.Id;
        }

        private async void BuildPaneAsync(AccountsSettingsPane sender, AccountsSettingsPaneCommandsRequestedEventArgs args)
        {
            var deferral = args.GetDeferral();

            if (!string.IsNullOrEmpty(HeaderText))
            {
                args.HeaderText = HeaderText;
            }

            if (App.DeviceFamily == DeviceFamily.Xbox)
            {
                var provider = await WebAuthenticationCoreManager.FindAccountProviderAsync(GetAuthorizeUrl());
                await GetMsaTokenFromProviderAsync(provider);
            }
            else
            {
                // Add consumer account provider
                var msaProvider = await WebAuthenticationCoreManager.FindAccountProviderAsync(GetAuthorizeUrl(), "consumers");
                args.WebAccountProviderCommands.Add(new WebAccountProviderCommand(msaProvider, GetMsaTokenAsync));
            }

            deferral.Complete();
        }

        private async void GetMsaTokenAsync(WebAccountProviderCommand providerCommand)
        {
            webCommandInvoked = true;
            await GetMsaTokenFromProviderAsync(providerCommand.WebAccountProvider);
        }

        private async Task GetMsaTokenFromProviderAsync(WebAccountProvider provider)
        {
            try
            {
                WebTokenRequest request = new WebTokenRequest(provider, string.Join(" ", scopes), appId);
                request.Properties.Add("resource", "https://graph.microsoft.com");
                WebTokenRequestResult result = await WebAuthenticationCoreManager.RequestTokenAsync(request);

                if (result.ResponseStatus == WebTokenRequestStatus.Success)
                {
                    StoreLoginProfile(new LoginProfile(provider, result.ResponseData[0].WebAccount));
                    LoginCompleted?.Invoke(this, new Token() { AccessToken = result.ResponseData[0].Token });
                }

                throw new Exception($"Login failed with status {result.ResponseStatus}, Error Code: {result.ResponseError?.ErrorCode}, ErrorMessage: {result.ResponseError?.ErrorMessage}");
            }
            catch (Exception)
            {
            }
        }

        public async Task SignOutAsync()
        {
            var profile = await GetLoginProfileAsync();

            await profile.Account.SignOutAsync();

            ApplicationData.Current.LocalSettings.Values.Remove(currentProviderIdKey);
            ApplicationData.Current.LocalSettings.Values.Remove(currentUserIdKey);
        }
    }
}

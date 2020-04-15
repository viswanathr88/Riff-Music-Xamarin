using Newtonsoft.Json;
using OnePlayer.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace OnePlayer.Authentication
{
    public sealed class LoginManager : ILoginManager
    {
        private const string appId = "19b11b92-7fc8-44c9-b794-8ae1d41cebed";
        private static readonly string[] scopes = new string[] { "User.Read", "files.read", "offline_access" };
        private const string authorizeUri = @"https://login.microsoftonline.com/common/oauth2/v2.0/authorize";
        private const string accessTokenUri = @"https://login.microsoftonline.com/common/oauth2/v2.0/token";
        private const string profileUri = @"https://graph.microsoft.com/V1.0/me/";
        private const string profilePhotoUri = @"https://graph.microsoft.com/beta/me/photo/$value";
        private const string redirectUri = "app://com.oneplayer.droid/oauth2redirect";

        private readonly HttpClient httpClient;
        private readonly ITokenCache tokenCache;
        private Token inMemoryToken = null;

        public LoginManager(HttpClient client, ITokenCache tokenCache)
        {
            this.httpClient = client;
            this.tokenCache = tokenCache;
        }

        public async Task<bool> LoginExistsAsync()
        {
            bool exists = false;
            try
            {
                var token = await GetCachedTokenAsync();
                if (token != null)
                {
                    exists = true;
                }
            }
            catch (Exception)
            {
                exists = false;
            }

            return exists;
        }

        public async Task<Token> AcquireTokenSilentAsync()
        {
            Token result = await GetCachedTokenAsync();

            if (result == null)
            {
                throw new Exception("No auth context available to fetch token silently. Login required");
            }

            if (result.HasExpired())
            {
                // Get a fresh access token
                string body = $"client_id={appId}&redirect_uri={redirectUri}&refresh_token={result.RefreshToken}&grant_type=refresh_token";

                HttpContent content = new StringContent(body);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                var response = await this.httpClient.PostAsync(accessTokenUri, content);
                response.EnsureSuccessStatusCode();

                string jsonToken = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<Token>(jsonToken);
                await this.tokenCache.SetAsync(result);
            }

            inMemoryToken = result;
            return result;
        }

        public string GetAuthorizeUrl()
        {
            return $"{authorizeUri}?client_id={appId}&scope={string.Join(" ", scopes)}&response_type=code&redirect_uri={redirectUri}";
        }

        public string GetRedirectUrl()
        {
            return redirectUri;
        }

        public async Task<Token> EndLoginAsync(string code)
        {
            IList<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("client_id", appId),
                new KeyValuePair<string, string>("redirect_uri", redirectUri),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("grant_type", "authorization_code")
            };

            var request = new HttpRequestMessage(HttpMethod.Post, accessTokenUri)
            {
                Content = new FormUrlEncodedContent(parameters)
            };
            var response = await this.httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            string jsonToken = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<Token>(jsonToken);
            
            await this.tokenCache.SetAsync(token);
            this.inMemoryToken = token;
            
            return token;
        }

        public async Task<UserProfile> GetUserAsync()
        {
            var token = await AcquireTokenSilentAsync();

            var request = new HttpRequestMessage(HttpMethod.Get, profileUri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

            var response = await this.httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            string body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserProfile>(body);
        }

        public async Task<Stream> GetUserPhotoAsync()
        {
            var token = await AcquireTokenSilentAsync();

            var request = new HttpRequestMessage(HttpMethod.Get, profilePhotoUri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

            var response = await this.httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStreamAsync();
        }

        private async Task<Token> GetCachedTokenAsync()
        {
            return inMemoryToken ?? await this.tokenCache.GetAsync();
        }
    }
}

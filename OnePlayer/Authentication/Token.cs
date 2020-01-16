using Newtonsoft.Json;
using System;

namespace OnePlayer.Authentication
{
    public class Token
    {
        public Token()
        {
            CreateTime = DateTime.Now;
        }

        [JsonProperty(PropertyName = "token_type")]
        public string TokenType { get; set; }

        [JsonProperty(PropertyName = "expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty(PropertyName = "scope")]
        public string Scope { get; set; }

        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }

        [JsonProperty(PropertyName = "refresh_token")]
        public string RefreshToken { get; set; }

        [JsonIgnore]
        public DateTime CreateTime { get; set; }

        public bool HasExpired()
        {
            return CreateTime.AddSeconds(ExpiresIn) < DateTime.Now;
        }

        public void EnsureValid()
        {
            if (string.IsNullOrEmpty(AccessToken) || string.IsNullOrEmpty(RefreshToken))
            {
                throw new Exception("Token is not valid");
            }
        }
    }

}

using Android.Content;
using Riff.Authentication;
using System;
using System.Threading.Tasks;

namespace Riff.Droid.Storage
{
    sealed class TokenCache : ITokenCache
    {
        private readonly ISharedPreferences preferences;
        public TokenCache(ISharedPreferences preferences)
        {
            this.preferences = preferences;
        }

        public Task<Token> GetAsync()
        {
            Token token = new Token()
            {
                AccessToken = this.preferences.GetString(nameof(Token.AccessToken), string.Empty),
                CreateTime = DateTime.Parse(this.preferences.GetString(nameof(Token.CreateTime), DateTime.MinValue.ToString())),
                ExpiresIn = this.preferences.GetInt(nameof(Token.ExpiresIn), 0),
                Scope = this.preferences.GetString(nameof(Token.Scope), string.Empty),
                RefreshToken = this.preferences.GetString(nameof(Token.RefreshToken), string.Empty),
                TokenType = this.preferences.GetString(nameof(Token.TokenType), string.Empty)
            };
            token.EnsureValid();
            return Task.FromResult(token);
        }

        public Task SetAsync(Token token)
        {
            var editor = this.preferences.Edit();
            editor.PutString(nameof(Token.AccessToken), token.AccessToken);
            editor.PutString(nameof(Token.CreateTime), token.CreateTime.ToString());
            editor.PutInt(nameof(Token.ExpiresIn), token.ExpiresIn);
            editor.PutString(nameof(Token.Scope), token.Scope);
            editor.PutString(nameof(Token.RefreshToken), token.RefreshToken);
            editor.PutString(nameof(Token.TokenType), token.TokenType);

            editor.Apply();
            return Task.CompletedTask;
        }
    }
}
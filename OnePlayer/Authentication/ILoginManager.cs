using OnePlayer.Data;
using System.IO;
using System.Threading.Tasks;

namespace OnePlayer.Authentication
{
    public interface ILoginManager
    {
        Task<Token> AcquireTokenSilentAsync();
        Task<Token> EndLoginAsync(string data);
        string GetAuthorizeUrl();
        string GetRedirectUrl();
        Task<UserProfile> GetUserAsync();
        Task<Stream> GetUserPhotoAsync();
        Task<bool> LoginExistsAsync();

        Task SignOutAsync();
    }
}
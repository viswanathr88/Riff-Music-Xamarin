using OnePlayer.Data;
using System.IO;
using System.Threading.Tasks;

namespace OnePlayer.Authentication
{
    public interface ILoginManager
    {
        Task<Token> AcquireTokenSilentAsync();
        Task<Token> EndLoginAsync(object data);
        string GetAuthorizeUrl();
        string GetRedirectUrl();
        Task<UserProfile> GetUserAsync();
        Task<Stream> GetUserPhotoAsync();
        Task<bool> LoginExistsAsync();

        Task SignOutAsync();
    }
}
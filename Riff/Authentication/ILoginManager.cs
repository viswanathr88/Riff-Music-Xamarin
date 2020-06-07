using Riff.Data;
using System.IO;
using System.Threading.Tasks;

namespace Riff.Authentication
{
    public interface ILoginManager
    {
        Task<Token> AcquireTokenSilentAsync();
        Task<Token> LoginAsync(object data);
        string GetAuthorizeUrl();
        string GetRedirectUrl();
        Task<UserProfile> GetUserAsync();
        Task<Stream> GetUserPhotoAsync();
        Task<bool> LoginExistsAsync();
        Task SignOutAsync();
    }
}
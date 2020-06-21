using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Riff.Authentication
{
    public interface ITokenCache
    {
        Task<Token> GetAsync();

        Task SetAsync(Token token);
    }
}

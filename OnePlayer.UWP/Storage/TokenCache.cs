using OnePlayer.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlayer.UWP.Storage
{
    class TokenCache : ITokenCache
    {
        public Task<Token> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task SetAsync(Token token)
        {
            throw new NotImplementedException();
        }
    }
}

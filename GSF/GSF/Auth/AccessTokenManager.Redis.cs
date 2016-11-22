using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSF.Auth
{
    class AccessTokenManagerRedis : IAccessTokenManager
    {
        public Task<string> CreateNew(string userType, string userId, string accessToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> Query(string accessToken)
        {
            throw new NotImplementedException();
        }
    }
}

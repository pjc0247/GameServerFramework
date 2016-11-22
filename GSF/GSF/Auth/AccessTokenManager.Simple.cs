using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSF.Auth
{
    class AccessTokenManagerSimple : IAccessTokenManager
    {
        private Dictionary<string, string> Storage { get; set; }

        public AccessTokenManagerSimple()
        {
            Storage = new Dictionary<string, string>();
        }

        public Task<string> CreateNew(string userType, string userId, string accessToken)
        {
            var token = Guid.NewGuid().ToString();
            Storage[token] = userId;

            return Task.FromResult(token);
        }

        public Task<string> Query(string accessToken)
        {
            if (Storage.ContainsKey(accessToken) == false)
                return Task.FromResult<string>(null);

            return Task.FromResult(Storage[accessToken]);
        }
    }
}

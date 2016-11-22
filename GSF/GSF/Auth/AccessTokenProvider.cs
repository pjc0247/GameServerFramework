using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSF.Auth
{
    class AccessTokenProvider
    {
        private static IAccessTokenManager Manager { get; set; }

        static AccessTokenProvider()
        {
            Manager = new AccessTokenManagerSimple();
        }

        public static Task<string> CreateNew(
            string userType, string userId, string accessToken)
        {
            return Manager.CreateNew(userType, userId, accessToken);
        }

        public static Task<string> Query(string accessToken)
        {
            return Manager.Query(accessToken);
        }
    }
}

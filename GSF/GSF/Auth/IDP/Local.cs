using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSF.Auth.IDP
{
    class Local : IDProvider
    {
        public async Task<bool> IsValidToken(string userId, string accessToken)
        {
            var got = await AccessTokenProvider.Query(accessToken);

            if (got == null)
                return false;

            return userId == got;
        }
    }
}

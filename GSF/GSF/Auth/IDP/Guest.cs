using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSF.Auth.IDP
{
    class Guest : IDProvider
    {
        public Task<bool> IsValidToken(string userId, string accessToken)
        {
            return Task.FromResult(true);
        }
    }
}

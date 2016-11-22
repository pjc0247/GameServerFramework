using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSF.Auth.IDP
{
    public interface IDProvider
    {
        Task<bool> IsValidToken(string userId, string accessToken);
    }

    public class IDProviderCodeAttribute : Attribute
    {
        public string Code;

        public IDProviderCodeAttribute(string code)
        {
            Code = code;
        }
    }
}

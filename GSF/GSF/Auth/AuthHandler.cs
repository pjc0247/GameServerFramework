using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSF.Auth
{
    using IDP;

    class AuthHandler
    {
        private Dictionary<string, IDProvider> IDProviders
            = new Dictionary<string, IDProvider>();

        public AuthHandler()
        {
            AddIDProvider<Guest>(UserType.Guest);
            AddIDProvider<Local>(UserType.Local);
        }

        public void AddIDProvider<T>(string userType)
            where T : IDProvider, new()
        {
            IDProviders.Add(userType, new T());
        }

        public async Task<bool> Login(
            string userType,
            string userId, string accessToken)
        {
            if (IDProviders.ContainsKey(userType) == false)
                throw new NotSupportedException("unknown userType");

            return await IDProviders[userType].IsValidToken(userId, accessToken);
        }
    }
}

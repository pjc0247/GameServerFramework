using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSF.Auth
{
    interface IAccessTokenManager
    {
        /// <summary>
        /// 지정된 정보를 저장한, 새 엑세스 토큰을 생성한다.
        /// </summary>
        /// <param name="userType"></param>
        /// <param name="userId"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        Task<string> CreateNew(string userType, string userId, string accessToken);

        Task<string> Query(string accessToken);
    }
}

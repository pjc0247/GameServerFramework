using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace GSF.Auth.IDP
{
    class Facebook : IDProvider
    {
        public async Task<bool> IsValidToken(string userId, string accessToken)
        {
            var http = new HttpClient();

            var response = await http.GetAsync(
                "https://graph.facebook.com/v2.8/debug_token?input_token=" + accessToken);

            if (response.IsSuccessStatusCode == false)
                return false;

            var json = await response.Content.ReadAsStringAsync();

            try
            {
                var data = JObject.Parse(json)["data"];
                var isValid = (bool)data["is_valid"];

                if (isValid == false)
                    return false;

                /* [ TODO ]
                 * APP ID 검사 
                 * 이 앱을 통해서 얻은 토큰인지를 알기 위해
                 */

                if (userId != (string)data["user_id"])
                    return false;

                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}

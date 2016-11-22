using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace GSF.Env
{
    class EnvEC2 : IEnv
    {
        public string PublicAddress
        {
            get
            {
                var url = "http://169.254.169.254/latest/meta-data/public-ipv4";
                var httpClient = new HttpClient();
                return httpClient.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
            }
        }

        public int Port => 9916;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSF.Env
{
    class EnvLocalWithoutDB : IEnv
    {
        public string PublicAddress => "localhost";
        public int Port => 9916;
    }
}

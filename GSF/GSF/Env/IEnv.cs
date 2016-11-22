using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSF.Env
{
    interface IEnv
    {
        string PublicAddress { get; }
        int Port { get; }

        //bool useSecureSocket { get; }
    }
}

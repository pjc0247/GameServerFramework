using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSF.Env
{
    class ServerEnv
    {
        public static IEnv CurrentEnv { get; private set; }

        static ServerEnv()
        {
#if BUILD_EC2
            selectedEnv = new EnvEC2();
#else
            CurrentEnv = new EnvLocalWithoutDB();
#endif

            Console.WriteLine("SelectedEnv : " + CurrentEnv);
        }
    }
}

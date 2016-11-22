using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GSF;
using GSF.Packet;
using GSF.Packet.Json;

namespace SampleServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hi");

            PacketSerializer.Protocol = new JsonProtocol();
        }
    }
}

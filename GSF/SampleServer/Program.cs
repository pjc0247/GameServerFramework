using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GSF;
using GSF.Packet;
using GSF.Packet.Json;

using SamplePacket;

namespace SampleServer
{
    public class EchoService : Service<EchoService>
    {
        public void OnEchoPacket(EchoPacket packet)
        {
            SendPacket(new EchoPacket()
            {
                Message = packet.Message
            });
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hi");

            PacketSerializer.Protocol = new JsonProtocol();

            Server.Create(9916)
                .WithService<EchoService>("/echo")
                .Run();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GSF;
using GSF.Packet;
using GSF.Packet.Json;
using GSF.MatchMaking;

using SamplePacket;

namespace SampleServer
{
    public class EchoService : Service<EchoService>
    {
        public void OnEchoPacket(EchoPacket packet)
        {
            SendReplyPacket(packet, new EchoPacket()
            {
                Message = packet.Message
            });

            Console.WriteLine(System.Threading.Thread.CurrentThread.ManagedThreadId);

            System.Threading.Thread.Sleep(100000);

            Console.WriteLine("END");
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            GSF.Ranking.RankingService.Test();

            Console.WriteLine("Hi");

            PacketSerializer.Protocol = new JsonProtocol();

            Server.Create(9916)
                //.WithService<EchoService>("/echo")
                .WithService<GSF.Ranking.RankingService>("/echo")
                .Run();

            Console.ReadLine();

            var q = new MatchQueueLocal();

            q.Reset(5);

            q.Enqueue(new MatchGroup()
            {
                Players = new MatchPlayer[]
                {
                    new MatchPlayer() { UserId = "1" },
                    new MatchPlayer() { UserId = "2" },
                    new MatchPlayer() { UserId = "3" },
                    new MatchPlayer() { UserId = "4" },
                }
            });

            q.Enqueue(new MatchGroup()
            {
                Players = new MatchPlayer[]
                {
                    new MatchPlayer() { UserId = "1" },
                    new MatchPlayer() { UserId = "2" },
                    new MatchPlayer() { UserId = "3" },
                }
            });

            q.Enqueue(new MatchGroup()
            {
                Players = new MatchPlayer[]
                {
                    new MatchPlayer() { UserId = "1" },
                    new MatchPlayer() { UserId = "2" },
                }
            });

            q.Enqueue(new MatchGroup()
            {
                Players = new MatchPlayer[]
                {
                    new MatchPlayer() { UserId = "1" },
                }
            });

            MatchData result;
            if (q.TryDequeue(5, out result))
            {
                Console.WriteLine("MatchCreated");
            }
            else
                Console.WriteLine("MatchFail");
        }
    }
}

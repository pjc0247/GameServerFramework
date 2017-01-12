using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebSocketSharp;

using GSF.Packet;
using GSF.Packet.Json;
using GSF.Ranking;
using GSF.Ez.Packet;

using SamplePacket;

namespace SampleClient
{

    class Foo : PacketBase
    {
        public string Value;
    }

    class Program
    {

        static void SendPacket(WebSocket ws, PacketBase packet)
        {
            var json = PacketSerializer.Serialize(packet);

            Console.WriteLine(json);

            ws.Send(json);
        }

        static void Main(string[] args)
        { 
            PacketSerializer.Protocol = new JsonProtocol();

            var ws = new WebSocket("ws://localhost:9916/echo?version=1.0.0&userType=guest&userId=1");
            ws.Connect();

            Console.WriteLine("IsAlive : " + ws.IsAlive);
            
            SendPacket(ws, new JoinPlayer()
            {
                Player = new EzPlayer()
                {
                    UserId = 1234,
                    Property = new Dictionary<string, object>()
                    {
                        {"nickname", "jwvg"}
                    }
                }
            });

            SendPacket(ws, new RequestBroadcast()
            {
                Type = 1,
                Data = new Dictionary<string, object>()
                {
                    {"a", "B"}
                }
            });

            /*
            SendPacket(ws, new AddScore()
            {
                RankingKey = "level",
                Score = 1000,
                Player = new GSF.Player() { PlayerId = "player1" }
            });
            SendPacket(ws, new AddScore()
            {
                RankingKey = "level",
                Score = 2000,
                Player = new GSF.Player() { PlayerId = "player2" }
            });
            SendPacket(ws, new AddScore()
            {
                RankingKey = "level",
                Score = 3000,
                Player = new GSF.Player() { PlayerId = "player3" }
            });

            SendPacket(ws, new QueryPlayerRank()
            {
                RankingKey = "level",
                Player = new GSF.Player() { PlayerId = "player3" }
            });
            */

            ws.OnMessage += Ws_OnMessage;

            Console.ReadLine();
        }

        private static void Ws_OnMessage(object sender, MessageEventArgs e)
        {
            Console.WriteLine("RECEIVD");
            Console.WriteLine(e.Data);
        }
    }
}

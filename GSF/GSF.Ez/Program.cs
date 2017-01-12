using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GSF;
using GSF.Packet;
using GSF.Packet.Json;
using GSF.Ez.Packet;

namespace GSF.Ez
{
    public class EzService : Service<EzService>
    {
        private static List<EzService> Sessions = new List<EzService>();
        private static Dictionary<string, object> WorldProperty = new Dictionary<string, object>();

        private EzPlayer Player;

        public void OnModifyWorldProperty(ModifyWorldProperty packet)
        {
            lock (WorldProperty)
            {
                foreach (var pair in packet.Property)
                    WorldProperty[pair.Key] = pair.Value;
            }

			lock (Sessions)
			{
				Sessions.Broadcast(packet);
			}
        }
        public void OnModifyPlayerProperty(ModifyPlayerProperty packet)
        {
            foreach (var pair in packet.Property)
                Player.Property[pair.Key] = pair.Value;
        }

        public void OnJoinPlayer(JoinPlayer packet)
        {
            lock (Sessions)
            {
                Sessions.Broadcast(packet);

                lock (WorldProperty)
                {
                    SendPacket(new WorldInfo()
                    {
                        Players = Sessions.Select(x => x.Player).ToArray(),
                        Property = WorldProperty
                    });
                }

                Sessions.Add(this);
            }

            Player = packet.Player;
        }
        public void OnLeavePlayer(LeavePlayer packet)
        {
            if (Player == null) return;

            lock (Sessions)
            {
                Sessions.Remove(this);
                Sessions.Broadcast(new LeavePlayer()
                {
                    Player = Player
                });
            }
            Player = null;
        }
        protected override void OnSessionClosed()
        {
            OnLeavePlayer(null);
        }

        public void OnRequestBroadcast(RequestBroadcast packet)
        {
            lock (Sessions)
            {
                Sessions.Broadcast(new BroadcastPacket()
                {
                    Sender = Player,

                    Type = packet.Type,
                    Data = packet.Data                    
                });
            }
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Ez");

            PacketSerializer.Protocol = new JsonProtocol();

            Server.Create(9916)
                .WithService<EzService>("/echo")
                //.WithService<GSF.Ranking.RankingService>("/echo")
                .Run();

            Console.ReadLine();
        }
    }
}

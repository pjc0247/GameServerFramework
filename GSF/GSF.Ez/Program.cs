using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

using GSF;
using GSF.Packet;
using GSF.Packet.Json;
using GSF.Ez.Packet;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GSF.Ez
{
	public class InitializationService
	{
		private static string DataSource = "https://jwvgtest.azurewebsites.net/api/GenerateMapData?code=ifMbPygn7iCbYduURc/zX/n2gJzD0lEqG1ej5apWKaacEhSC8DK6MA==";

		public static void Init()
		{
			var http = new HttpClient();
			var json = http.GetAsync(DataSource).Result.Content.ReadAsStringAsync().Result;

			var jobj = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
			var property = (JObject)jobj["worldProperty"];

			foreach (var pair in property)
			{
				EzService.WorldProperty[pair.Key] = pair.Value;
			}
		}
	}

    public class EzService : Service<EzService>
    {
        private static List<EzService> Sessions = new List<EzService>();
        public static Dictionary<string, object> WorldProperty = new Dictionary<string, object>();

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
			if (File.Exists("players\\" + packet.Player.PlayerId))
			{
				var json = File.ReadAllText("players\\" + packet.Player.PlayerId);
				packet.Player = JsonConvert.DeserializeObject<EzPlayer>(json);
			}

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

			var json = JsonConvert.SerializeObject(Player);
			File.WriteAllText("players\\" + Player.PlayerId, json);
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

			InitializationService.Init();

			if (Directory.Exists("players") == false)
				Directory.CreateDirectory("players");

            PacketSerializer.Protocol = new JsonProtocol();

            Server.Create(9916)
                .WithService<EzService>("/echo")
                //.WithService<GSF.Ranking.RankingService>("/echo")
                .Run();

            Console.ReadLine();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;

using GSF;
using GSF.Packet;
using GSF.Packet.Json;
using GSF.Ez.Packet;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using JsonFx.Json;

namespace GSF.Ez
{
	public class Config
	{
        public int Port;

		public string OptionalWorldPropertyDataSource;
		public string WorldPropertyDataSource;

		public Dictionary<string, object> WorldProperty;
		public Dictionary<string, object> OptionalWorldProperty;

        // default values
        public Config()
        {
            Port = 9916;
        }
	}

	public class InitializationService
	{
		public static Config Config;

		private static readonly string ConfigPath = "config.json";

		private static void LoadConfig()
		{
			if (File.Exists(ConfigPath) == false)
			{
				Config = new Config();
				return;
			}

			var json = File.ReadAllText(ConfigPath);
            Config = JsonFx.Json.JsonReader.Deserialize<Config>(json);

			Console.WriteLine("LoadConfig");
			Console.WriteLine(json);
		}

		private static dynamic LoadPropertyFromDataSource(string uri)
		{
			var http = new HttpClient();
			var json = http.GetAsync(uri).Result.Content.ReadAsStringAsync().Result;

            return JsonFx.Json.JsonReader.Deserialize<Dictionary<string, object>>(json)["worldProperty"];
		}
        private static string BindString(string src)
        {
            src = src.Replace("{rd}", (new Random()).Next(10000).ToString());
            return src;
        }

		public static void Init()
		{
			LoadConfig();

			// from DefaultValue
			if (Config.OptionalWorldProperty != null)
				EzService.OptionalWorldProperty = Config.OptionalWorldProperty;
			if (Config.WorldProperty != null)
				EzService.WorldProperty = Config.WorldProperty;

			// from DataSource
			if (string.IsNullOrEmpty(Config.OptionalWorldPropertyDataSource) == false) {
				Console.WriteLine("Load OptionalWorldProperty from DataSource");

                var data = LoadPropertyFromDataSource(BindString(Config.OptionalWorldPropertyDataSource));
                foreach (var pair in data)
					EzService.OptionalWorldProperty[pair.Key] = pair.Value;
                Console.WriteLine("Done (" + data.Count + " Key(s))");
            }
			if (string.IsNullOrEmpty(Config.WorldPropertyDataSource) == false)
			{
				Console.WriteLine("Load WorldProperty from DataSource");
                var data = LoadPropertyFromDataSource(BindString(Config.WorldPropertyDataSource));
                foreach (var pair in data)
					EzService.WorldProperty[pair.Key] = pair.Value;
                Console.WriteLine("Done (" + data.Count + " Key(s))");
			}
		}
	}

    public class EzService : Service<EzService>
    {
        private static List<EzService> Sessions = new List<EzService>();
        public static Dictionary<string, object> WorldProperty = new Dictionary<string, object>();
		public static Dictionary<string, object> OptionalWorldProperty = new Dictionary<string, object>();

        private EzPlayer Player;

		public void OnRequestOptionalWorldProperty(RequestOptionalWorldProperty packet)
		{
			var dic = new Dictionary<string, object>();

			lock (OptionalWorldProperty)
			{
                foreach (var key in packet.Keys)
                {
                    if (OptionalWorldProperty.ContainsKey(key))
                        dic[key] = OptionalWorldProperty[key];
                    else
                        dic[key] = null;
                }
			}

            SendReplyPacket(packet, new OptionalWorldProperty()
            {
                Property = dic
            });
		}

		public void OnModifyOptionalWorldProperty(ModifyOptionalWorldProperty packet)
		{
			lock (OptionalWorldProperty)
			{
				foreach (var pair in packet.Property)
					OptionalWorldProperty[pair.Key] = pair.Value;

                foreach (var key in packet.RemovedKeys)
                    OptionalWorldProperty.Remove(key);
			}

			lock (Sessions)
			{
				Sessions.Broadcast(packet);
			}
		}
        public void OnModifyWorldProperty(ModifyWorldProperty packet)
        {
            lock (WorldProperty)
            {
                foreach (var pair in packet.Property)
                    WorldProperty[pair.Key] = pair.Value;

                foreach (var key in packet.RemovedKeys)
                    WorldProperty.Remove(key);
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

            foreach (var key in packet.RemovedKeys)
                Player.Property.Remove(key);

            lock (Sessions)
            {
                packet.Player = Player;
                Sessions.Broadcast(packet);
            }
        }

        public void OnJoinPlayer(JoinPlayer packet)
        {
            var inputPlayer = packet.Player;

			if (File.Exists("players\\" + packet.Player.PlayerId))
			{
				var json = File.ReadAllText("players\\" + packet.Player.PlayerId);
				packet.Player = JsonConvert.DeserializeObject<EzPlayer>(json);

                foreach (var pair in inputPlayer.Property)
                    packet.Player.Property[pair.Key] = pair.Value;
			}

            lock (Sessions)
            {
                Sessions.Broadcast(packet);

                lock (WorldProperty)
                {
                    SendPacket(new WorldInfo()
                    {
                        Player = packet.Player,
                        OtherPlayers = Sessions.Select(x => x.Player).ToArray(),
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
            var config = InitializationService.Config;

            if (Directory.Exists("players") == false)
				Directory.CreateDirectory("players");

            PacketSerializer.Protocol = new JsonProtocol();

            Server.Create(config.Port)
                .WithService<EzService>("/ez")
                .Run();

            Console.ReadLine();
        }
    }
}

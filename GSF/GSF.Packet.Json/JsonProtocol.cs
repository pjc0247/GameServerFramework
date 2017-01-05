using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace GSF.Packet.Json
{
    public class JsonProtocol : IPacketProtocol
    {
        protected JsonSerializerSettings JsonSettings { get; set; }

        public JsonProtocol()
        {
            JsonSettings = new JsonSerializerSettings();
            JsonSettings.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full;
            JsonSettings.TypeNameHandling = TypeNameHandling.All;
        }

        public PacketBase Deserialize(byte[] data)
        {
            var json = Encoding.UTF8.GetString(data);

            return (PacketBase)JsonConvert.DeserializeObject(json, JsonSettings);
        }

        public byte[] Serialize<T>(T packet)
            where T : PacketBase
        {
            var json = JsonConvert.SerializeObject(packet, JsonSettings);

            return Encoding.UTF8.GetBytes(json);
        }
    }
}

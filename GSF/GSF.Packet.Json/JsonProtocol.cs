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
        public PacketBase Deserialize(string data)
        {
            return (PacketBase)JsonConvert.DeserializeObject(data);
        }

        public string Serialize<T>(T packet)
            where T : PacketBase
        {
            var setting = new JsonSerializerSettings()
            {
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full
            };
            var json = JsonConvert.SerializeObject(packet);

            return json;
        }
    }
}

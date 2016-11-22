using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GSF.Packet
{
    public class PacketSerializer
    {
        public static IPacketProtocol Protocol { get; set; }

        public static string Serialize<T>(T packet)
            where T : PacketBase
        {
            if (packet == null)
                throw new ArgumentNullException("packet");

            return Protocol.Serialize(packet);
        }

        public static PacketBase Deserialize(string data)
        {
            if (string.IsNullOrEmpty(data))
                throw new ArgumentException("data");

            return Protocol.Deserialize(data);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GSF.Packet
{
    public class PacketSerializer
    {
        public static IPacketProtocol Protocol { get; set; }

        public static byte[] Serialize<T>(T packet)
            where T : PacketBase
        {
            if (packet == null)
                throw new ArgumentNullException("packet");

            return Protocol.Serialize(packet);
        }

        public static PacketBase Deserialize(byte[] data)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentException("data");

            return Protocol.Deserialize(data);
        }
    }
}

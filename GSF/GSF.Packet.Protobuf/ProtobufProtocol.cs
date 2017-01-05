using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSF.Packet.Protobuf
{
    public class JsonProtocol : IPacketProtocol
    {
        public PacketBase Deserialize(byte[] data)
        {
        }

        public byte[] Serialize<T>(T packet)
            where T : PacketBase
        {
        }
    }
}

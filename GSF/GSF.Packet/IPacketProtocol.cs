using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GSF.Packet
{
    public interface IPacketProtocol
    {
        string Serialize<T>(T packet)
            where T : PacketBase;

        PacketBase Deserialize(string data);
    }
}

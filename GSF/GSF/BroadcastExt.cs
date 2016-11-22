using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSF
{
    using Packet;

    static class BroadcastExt
    {
        public static void Broadcast<T>(
            this IEnumerable<Service<T>> sessions,
            PacketBase packet)
        {
            var json = PacketSerializer.Serialize(packet);
            
            foreach(var session in sessions)
                session.SendRawPacket(json);
        }
    }
}

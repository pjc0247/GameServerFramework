using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GSF.Ez.Packet
{
    using GSF.Packet;

    public class EzPlayer
    {
        public long UserId;

        public Dictionary<string, object> Property;
    }

    public class WorldInfo : PacketBase
    {
        public EzPlayer[] Players;

        public Dictionary<string, object> Property;
    }

    public class JoinPlayer : PacketBase
    {
        public EzPlayer Player;
    } 
    public class LeavePlayer : PacketBase
    {
        public EzPlayer Player;
    }

    public class BroadcastPacket : PacketBase
    {
        public EzPlayer Sender;

        public int Type;
        public Dictionary<string, object> Data;
    }
    public class RequestBroadcast : PacketBase
    {
        public int Type;
        public Dictionary<string, object> Data;
    }
}

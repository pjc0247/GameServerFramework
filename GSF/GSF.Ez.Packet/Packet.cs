using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GSF.Ez.Packet
{
    using GSF.Packet;

    public class EzPlayer
    {
        public string PlayerId;

        public Dictionary<string, object> Property;
    }

    public class WorldInfo : PacketBase
    {
        public string RootPlayerId;

        public EzPlayer Player;
        public EzPlayer[] OtherPlayers;

        public Dictionary<string, object> Property;
    }

    public class ModifyPlayerProperty : PacketBase
    {
        public EzPlayer Player;

        public Dictionary<string, object> Property;
        public string[] RemovedKeys;
    }
    public class ModifyWorldProperty : PacketBase
    {
        public Dictionary<string, object> Property;
        public string[] RemovedKeys;
    }

	public class RequestOptionalWorldProperty : PacketBase
	{
		public string[] Keys;
	}
	public class OptionalWorldProperty : PacketBase
	{
		public Dictionary<string, object> Property;
	}
	public class ModifyOptionalWorldProperty : PacketBase
	{
		public Dictionary<string, object> Property;
        public string[] RemovedKeys;
    }
     
    public class JoinPlayer : PacketBase
    {
        public EzPlayer Player;
    } 
    public class LeavePlayer : PacketBase
    {
        public EzPlayer Player;

        public string RootPlayerId;
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

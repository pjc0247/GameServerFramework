﻿using System;
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

        public bool Slient;
    }
    public class ModifyWorldProperty : PacketBase
    {
        public Dictionary<string, object> Property;
        public string[] RemovedKeys;

        public bool Slient;
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

        public bool Slient;
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

        public string Tag;
    }
    public class RequestBroadcast : PacketBase
    {
        public int Type;
        public Dictionary<string, object> Data;

        public string Tag;
    }

    public class SubscribeTag : PacketBase
    {
        public string[] Tags;
    }
    public class UnsubscribeTag : PacketBase
    {
        public string[] Tags;
    }

    public class RequestRemoteCall : PacketBase
    {
        public EzPlayer Player;

        public string FunctionName;
        public object[] Args;
    }
    public class RespondRemoteCall : PacketBase
    {
        public EzPlayer RespondTo;

        public object Result;
        public Exception Exception;

        public bool IsSuccess;
    }
}

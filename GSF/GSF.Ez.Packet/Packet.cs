using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GSF.Ez.Packet
{
    using GSF.Packet;

    public class EzItem
    {
        public string ItemId;
        public long Quantity;

        public Dictionary<string, object> Property;
    }
    public class EzPlayer
    {
        public string PlayerId;

        public Dictionary<string, object> Property;
        public Dictionary<string, EzItem> PublicInventory;
    }

    public class WorldInfo : PacketBase
    {
        public string RootPlayerId;

        public EzPlayer Player;
        public EzPlayer[] OtherPlayers;

        public Dictionary<string, object> Property;
        public EzItem[] PrivateInventory;
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

    #region INVENTORY_PACKET
    public enum EzItemMergeType
    {
        Never,
        Allow
    }

    public class AddItemToPrivateInventory : PacketBase
    {
        public EzItem[] Item;

        public EzItemMergeType MergeType;
    }
    public class RemoveItemFromPrivateInventory : PacketBase
    {
        public EzItem[] Item;
    }
    public class ConsumeItemFromPrivateInventory : PacketBase
    {
        public EzItem[] Item;
    }
    public class MoveToPublicInventory : PacketBase
    {
        public EzItem Item;
        public string Key;
    }
    public class MoveToPrivateInventory : PacketBase
    {
        public string Key;
    }
    #endregion
}

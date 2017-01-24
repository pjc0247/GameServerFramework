using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSF.Ez
{
    using Packet;

    public partial class EzService
    {
        public void OnAddItemToPrivateInventory(AddItemToPrivateInventory packet)
        {
            foreach(var item in packet.Items)
            {

            }
        }
        public void OnRemoveItemFromPrivateInventory(RemoveItemFromPrivateInventory packet)
        {

        }
        public void OnConsumeItemFromPrivateInventory(ConsumeItemFromPrivateInventory packet)
        {

        }
    }
}

using ARPG.InventorySystem.Items;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ARPG.InventorySystem.Inventory
{
    [Serializable]
    public class Inventory
    {
        #region Variables

        public InventorySlot[] slots = new InventorySlot[24];

        #endregion Variables

        #region Properties
        #endregion Properties

        #region Methods

        public void Clear()
        {
            foreach (InventorySlot slot in slots)
            {
                slot.UpdateSlot(new Item(), 0);
            }
        }

        public bool IsContain(ItemObject itemObject)
        {
            return Array.Find(slots, i => i.item.id == itemObject.data.id) != null;
        }

        public bool IsContain(int id)
        {
            return slots.FirstOrDefault(i => i.item.id == id) != null;
        }


        [OnError]
        internal void OnError(StreamingContext context, ErrorContext errorContext)
        {
            UnityEngine.Debug.LogError("Serialization Error: " + errorContext.Error.Message);
            errorContext.Handled = true;
        }

        #endregion Methods
    }
}

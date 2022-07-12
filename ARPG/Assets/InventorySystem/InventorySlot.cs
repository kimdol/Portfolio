using ARPG.InventorySystem.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG.InventorySystem.Inventory
{
    [Serializable]
    public class InventorySlot
    {
        #region Variables

        public ItemType[] AllowedItems = new ItemType[0];

        [NonSerialized]
        public InventoryObject parent;
        [NonSerialized]
        public GameObject slotUI;

        [NonSerialized]
        public Action<InventorySlot> OnPreUpdate;
        [NonSerialized]
        public Action<InventorySlot> OnPostUpdate;

        public Item item;
        public int amount;

        [NonSerialized]
        public GameObject subSlotUI;
        public Vector3 subSlotUIPos;
        public bool haveSubSlotUI = false;

        #endregion Variables

        #region Properties
        public ItemObject ItemObject
        {
            get
            {
                return item.id >= 0 ? parent.database.itemObjects[item.id] : null;
            }
        }

        #endregion Properties

        #region Methods
        public InventorySlot() => UpdateSlot(new Item(), 0);

        public InventorySlot(Item item, int amount) => UpdateSlot(item, amount);

        public InventorySlot(Item item, int amount, bool haveSubSlotUI) => UpdateSlot(item, amount, haveSubSlotUI);

        public void RemoveItem() => UpdateSlot(new Item(), 0);

        public void AddAmount(int value) => UpdateSlot(item, amount += value);

        public void UpdateSlot(Item item, int amount)
        {
            if (amount <= 0)
            {
                subSlotUIPos = new Vector3();
                haveSubSlotUI = false;
                item = new Item();
            }

            OnPreUpdate?.Invoke(this);
            this.item = item;
            this.amount = amount;
            OnPostUpdate?.Invoke(this);
        }

        public void UpdateSlot(Item item, int amount, bool haveSubSlotUI)
        {
            if (amount <= 0)
            {
                subSlotUIPos = new Vector3();
                haveSubSlotUI = false;
                item = new Item();
            }

            OnPreUpdate?.Invoke(this);
            this.item = item;
            this.amount = amount;
            this.haveSubSlotUI = haveSubSlotUI;
            OnPostUpdate?.Invoke(this);
        }

        public bool CanPlaceInSlot(ItemObject itemObject)
        {
            if (AllowedItems.Length <= 0 || itemObject == null || itemObject.data.id < 0)
            {
                return true;
            }

            foreach (ItemType itemType in AllowedItems)
            {
                if (itemObject.type == itemType)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion Methods
    }
}

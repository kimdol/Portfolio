using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG.InventorySystem.Items
{
    [Serializable]
    public class Item
    {
        public int id = -1;
        public string name;

        public int width = 1;
        public int height = 1;

        public ItemBuff[] buffs;

        public Item()
        {
            id = -1;
            name = "";
        }

        public Item(ItemObject itemObject)
        {
            name = itemObject.name;
            id = itemObject.data.id;

            width = itemObject.data.width;
            height = itemObject.data.height;

            buffs = new ItemBuff[itemObject.data.buffs.Length];
            for (int i = 0; i < buffs.Length; i++)
            {
                buffs[i] = new ItemBuff(itemObject.data.buffs[i].Min, itemObject.data.buffs[i].Max)
                {
                    stat = itemObject.data.buffs[i].stat
                };
            }
        }
    }
}
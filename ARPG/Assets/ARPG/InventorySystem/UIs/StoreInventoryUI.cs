using ARPG.InventorySystem.Inventory;
using ARPG.InventorySystem.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace ARPG.InventorySystem.UIs
{
    public class StoreInventoryUI : DynamicInventoryUI
    {
        [SerializeField]
        public StatsObject playerStats;

        public Text goldText;

        protected override void Start()
        {
            base.Start();

            goldText.text = "Gold : " + playerStats.Gold.ToString();
        }

        public override void OnEndDrag(GameObject go)
        {
            Destroy(MouseData.tempItemBeingDragged);

            if (MouseData.interfaceMouseIsOver == null || MouseData.interfaceMouseIsOver is StoreInventoryUI)
            {
                Debug.Log("MouseData.interfaceMouseIsOver is StoreInventoryUI");
                return;
            }
            else if (MouseData.slotHoveredOver)
            {
                if (playerStats.Gold < slotUIs[go].ItemObject.data.gold)
                {
                    return;
                }

                InventorySlot mouseHoverSlotData = MouseData.interfaceMouseIsOver.slotUIs[MouseData.slotHoveredOver];
                if (mouseHoverSlotData.ItemObject)
                {
                    return;
                }

                if (inventoryObject.SwapItems(slotUIs[go], mouseHoverSlotData))
                {
                    int pay = mouseHoverSlotData.ItemObject.data.gold;
                    playerStats.AddGold(-pay);
                    goldText.text = "Gold : " + playerStats.Gold.ToString();
                }
                
            }
        }

    }
}

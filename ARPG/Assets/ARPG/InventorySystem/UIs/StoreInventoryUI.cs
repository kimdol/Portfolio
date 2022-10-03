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

        #region Unity Methods

        void Update()
        {
            goldText.text = "Gold : " + playerStats.gold.ToString();
        }

        #endregion Unity Methods

        public override void OnEndDrag(GameObject go)
        {
            if (MouseData.interfaceMouseIsOver)
            {
                MouseData.interfaceMouseIsOver.DestroyImage();
            }
            else
            {
                Destroy(MouseData.tempItemBeingDragged);
            }


            if (MouseData.interfaceMouseIsOver == null)
            {
                UnityEngine.Debug.Log("MouseData.interfaceMouseIsOver == null");
            }
            else if (MouseData.slotHoveredOver)
            {
                if (playerStats.Gold < slotUIs[go].ItemObject.data.gold)
                {
                    return;
                }

                InventorySlot locTrSlot = new InventorySlot();
                InventorySlot overlapItem = new InventorySlot();

                if (MouseData.interfaceMouseIsOver.PlaceItem(slotUIs[go], ref overlapItem, ref locTrSlot))
                {
                    if (overlapItem.ItemObject)
                    {
                        Debug.Log("사이즈에 맞는 빈 슬롯에 놓으세요");
                        return;
                    }
                    // 바꿔야하는 표적을 변경함(overlapItem -> locTrSlot)
                    MouseData.interfaceMouseIsOver.ReadySwap(ref overlapItem, ref locTrSlot);
                    int pay = slotUIs[go].ItemObject.data.gold;

                    inventoryObject.SwapItems(slotUIs[go], locTrSlot);
                    if (MouseData.interfaceMouseIsOver is DynamicInventoryUI)
                    {
                        playerStats.AddGold(-pay);
                    }
                }
            }
        }



    }
}
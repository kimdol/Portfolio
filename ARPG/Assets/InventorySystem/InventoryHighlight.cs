using ARPG.InventorySystem.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG.InventorySystem.UIs
{
    public class InventoryHighlight : MonoBehaviour
    {
        [SerializeField] RectTransform highlighter;

        public void Show(bool b)
        {
            highlighter.gameObject.SetActive(b);
        }

        public void SetSize(InventorySlot targetItem, Vector2 size, Vector2 space)
        {
            float slotWidth = targetItem.item.width;
            float slotHeight = targetItem.item.height;
            Vector2 sz = new Vector2();
            sz.x = slotWidth * size.x + space.x * (slotWidth - 1);
            sz.y = slotHeight * size.y + space.y * (slotHeight - 1);
            highlighter.sizeDelta = sz;
        }

        public void SetPosition(InventoryUI targetGrid, InventorySlot targetItem)
        {
            SetParent(targetGrid);

            Vector2Int onGridPosition = targetGrid.inventoryObject.CalculateTileGridPosition(
                targetItem.slotUI.GetComponent<RectTransform>().anchoredPosition
                );
            Vector2 pos = targetGrid.inventoryObject.CalculatePositionOnGrid(targetItem, onGridPosition.x, onGridPosition.y);

            highlighter.anchoredPosition = pos;
            highlighter.SetAsLastSibling();
        }

        public void SetParent(InventoryUI targetGrid)
        {
            if (targetGrid == null)
            {
                return;
            }
            highlighter.SetParent(targetGrid.GetComponent<RectTransform>());
        }

        public void SetPosition(InventoryUI targetGrid, InventorySlot targetItem, int posX, int posY)
        {
            Vector2 pos = targetGrid.inventoryObject.CalculatePositionOnGrid(targetItem, posX, posY);

            highlighter.anchoredPosition = pos;
            highlighter.SetAsLastSibling();
        }
    }
}


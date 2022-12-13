//using ARPG.InventorySystem.Inventory;
//using ARPG.InventorySystem.Items;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.UI;
//using TMPro;

//namespace ARPG.InventorySystem.UIs
//{
//    public class DynamicInventoryUI_Experiment : InventoryUI_Experiment
//    {
//        #region Variables
//        [SerializeField]
//        protected GameObject slotPrefab;

//        [SerializeField]
//        protected Vector2 start;

//        [SerializeField]
//        protected Vector2 size;

//        [SerializeField]
//        protected Vector2 space;


//        [Min(1), SerializeField]
//        protected int numberOfColumn = 4;


//        #endregion Variables

//        #region Unity Methods

//        #endregion Unity Methods

//        #region Methods

//        public override void CreateSlots()
//        {
//            slotUIs = new Dictionary<GameObject, Inventory.InventorySlot>();

//            inventoryObject.start = start;
//            inventoryObject.size = size;
//            inventoryObject.space = space;
//            inventoryObject.numberOfColumn = numberOfColumn;

//            for (int i = 0; i < inventoryObject.Slots.Length; ++i)
//            {
//                GameObject go = Instantiate(slotPrefab, Vector3.zero, Quaternion.identity, transform);
//                go.GetComponent<RectTransform>().anchoredPosition = CalculatePosition(i);

//                AddEvent(go, EventTriggerType.PointerEnter, delegate { OnEnter(go); });
//                AddEvent(go, EventTriggerType.PointerExit, delegate { OnExit(go); });
//                AddEvent(go, EventTriggerType.BeginDrag, delegate { OnStartDrag(go); });
//                AddEvent(go, EventTriggerType.EndDrag, delegate { OnEndDrag(go); });
//                AddEvent(go, EventTriggerType.Drag, delegate { OnDrag(go); });
//                AddEvent(go, EventTriggerType.PointerClick,
//                    (data) => { OnClick(go, (PointerEventData)data); });

//                inventoryObject.Slots[i].slotUI = go;
//                slotUIs.Add(go, inventoryObject.Slots[i]);
//                go.name += ": " + i;
//                go.GetComponent<RectTransform>().sizeDelta = size;
//            }
//        }

//        public override void OnPreUpdate(InventorySlot slot)
//        {
//            base.OnPreUpdate(slot);

//            Destroy(slot.subSlotUI);
//        }

//        public override void OnPostUpdate(InventorySlot slot)
//        {
//            base.OnPostUpdate(slot);

//            Vector3 pos = slot.slotUI.GetComponent<RectTransform>().anchoredPosition;
//            Vector2Int onGridPosition = inventoryObject.CalculateTileGridPosition(pos);

//            inventoryObject.FillItem(slot, onGridPosition.x, onGridPosition.y);

//            CreateSubSlotUI(slot, slot.subSlotUIPos);
//        }

//        private void CreateSubSlotUI(InventorySlot slot, Vector2 subSlotPos)
//        {
//            if (slot.ItemObject == null || (slot.item.width < 2 && slot.item.height < 2) || !slot.haveSubSlotUI)
//            {
//                return;
//            }
//            GameObject go = Instantiate(slotPrefab, Vector3.zero, Quaternion.identity, transform);
//            go.GetComponent<RectTransform>().anchoredPosition = subSlotPos;

//            AddEvent(go, EventTriggerType.PointerEnter, delegate { OnEnter(go); });
//            AddEvent(go, EventTriggerType.PointerExit, delegate { OnExit(go); });
//            AddEvent(go, EventTriggerType.BeginDrag, delegate { OnStartDrag(go); });
//            AddEvent(go, EventTriggerType.EndDrag, delegate { OnEndDrag(go); });
//            AddEvent(go, EventTriggerType.Drag, delegate { OnDrag(go); });
//            AddEvent(go, EventTriggerType.PointerClick,
//                (data) => { OnClick(go, (PointerEventData)data); });

//            slot.subSlotUI = go;
//            slotUIs.Add(go, slot);
//            go.name += " subUI ";

//            slot.subSlotUI.transform.GetChild(0).GetComponent<Image>().sprite =
//                slot.item.id < 0 ? null : slot.ItemObject.icon;
//            slot.subSlotUI.transform.GetChild(0).GetComponent<Image>().color =
//                slot.item.id < 0 ? new Color(1, 1, 1, 0) : new Color(1, 1, 1, 1);
//            slot.subSlotUI.GetComponentInChildren<TextMeshProUGUI>().text =
//                slot.item.id < 0 ?
//                string.Empty : (slot.amount == 1 ? string.Empty : slot.amount.ToString("n0"));

//            float slotWidth = slot.ItemObject.data.width;
//            float slotHeight = slot.ItemObject.data.height;
//            Vector2 uiSize = new Vector2();
//            uiSize.x = slotWidth * size.x + space.x * (slotWidth - 1);
//            uiSize.y = slotHeight * size.y + space.y * (slotHeight - 1);
//            go.GetComponent<RectTransform>().sizeDelta = uiSize;
//        }

//        public Vector3 CalculatePosition(int i)
//        {
//            float x = start.x + ((space.x + size.x) * (i % numberOfColumn));
//            float y = start.y + (-(space.y + size.y) * (i / numberOfColumn));

//            return new Vector3(x, y, 0f);
//        }

//        public override void OnStartDrag(GameObject go)
//        {
//            base.OnStartDrag(go);
//        }

//        public override void OnDrag(GameObject go)
//        {
//            base.OnDrag(go);
//        }

//        public override void CreateHighlighter(InventorySlot slot)
//        {
//            inventoryHighlight.Show(true);
//            HandleHighlight(slot);
//        }

//        public override void OnEndDrag(GameObject go)
//        {
//            base.OnEndDrag(go);
//        }

//        public override void DestroyImage()
//        {
//            Destroy(MouseData.tempItemBeingDragged);
//            inventoryHighlight.Show(false);
//        }

//        public override void RemoveItem(GameObject go)
//        {
//            inventoryObject.CleanGridReference(slotUIs[go], InterfaceType.Inventory);
//        }

//        public override bool PlaceItem(InventorySlot slot, ref InventorySlot overlapItem, ref InventorySlot locTrSlot)
//        {
//            InventorySlot mouseHoverSlotData =
//                    MouseData.interfaceMouseIsOver.slotUIs[MouseData.slotHoveredOver];

//            Vector3 pos;
//            if (mouseHoverSlotData.haveSubSlotUI)
//            {
//                pos = mouseHoverSlotData.subSlotUIPos;
//            }
//            else
//            {
//                pos = mouseHoverSlotData.slotUI.GetComponent<RectTransform>().anchoredPosition;
//            }

//            pos.x -= (slot.item.width - 1) * size.x / 2;
//            pos.y += (slot.item.height - 1) * size.y / 2;
//            Vector2Int onGridPosition = inventoryObject.CalculateTileGridPosition(pos);

//            locTrSlot = inventoryObject.Slots[inventoryObject.CalculateIndex(onGridPosition.x, onGridPosition.y)];
//            overlapItem = new InventorySlot();
//            // slot이 목표위치로 갔을 때 위험한지 체크함
//            bool complete = inventoryObject.PlaceItem(
//                slot,
//                onGridPosition.x,
//                onGridPosition.y,
//                ref overlapItem
//                );
//            // 반대로 overlapItem이 slot위치에 갔을 때 위험한지 체크함
//            if (complete && slot.parent.type == InterfaceType.Inventory)
//            {
//                Vector3 prePos = slot.slotUI.GetComponent<RectTransform>().anchoredPosition;
//                Vector2Int onGridPrePos = inventoryObject.CalculateTileGridPosition(prePos);
//                InventorySlot overlapItem2 = new InventorySlot();

//                complete = inventoryObject.PlaceItem(
//                overlapItem,
//                onGridPrePos.x,
//                onGridPrePos.y,
//                ref overlapItem2
//                );
//            }

//            return complete;
//        }

//        public override void ReadySwap(ref InventorySlot overlapItem, ref InventorySlot locTrSlot)
//        {
//            // Swap의 위험성은 없지만 overlapItem이 검출되지 않아 null(빈슬롯)일 때 처리함
//            if (overlapItem.ItemObject == null)
//            {
//                overlapItem = locTrSlot;
//            }
//            // 실제 Swap 대상인 locTrSlot가 overlapItem로 copy함
//            InventorySlot temp = new InventorySlot(
//                overlapItem.item,
//                overlapItem.amount,
//                overlapItem.haveSubSlotUI
//                );
//            inventoryObject.CleanGridReference(overlapItem, InterfaceType.Inventory);

//            locTrSlot.UpdateSlot(temp.item, temp.amount, temp.haveSubSlotUI);
//        }


//        InventorySlot itemToHighlight;
//        private void HandleHighlight(InventorySlot slot)
//        {
//            InventorySlot mouseHoverSlotData =
//                    MouseData.interfaceMouseIsOver.slotUIs[MouseData.slotHoveredOver];

//            Vector3 pos;
//            if (mouseHoverSlotData.haveSubSlotUI)
//            {
//                pos = mouseHoverSlotData.subSlotUIPos;
//            }
//            else
//            {
//                pos = mouseHoverSlotData.slotUI.GetComponent<RectTransform>().anchoredPosition;
//            }

//            pos.x -= (slot.item.width - 1) * size.x / 2;
//            pos.y += (slot.item.height - 1) * size.y / 2;
//            Vector2Int onGridPosition = inventoryObject.CalculateTileGridPosition(pos);

//            if (slot.ItemObject == null)
//            {
//                itemToHighlight = GetItem(onGridPosition.x, onGridPosition.y);

//                if (itemToHighlight != null)
//                {
//                    inventoryHighlight.Show(true);
//                    inventoryHighlight.SetSize(itemToHighlight, size, space);
//                    inventoryHighlight.SetParent(this);
//                    inventoryHighlight.SetPosition(this, itemToHighlight);
//                }
//                else
//                {
//                    inventoryHighlight.Show(false);
//                }
//            }
//            else
//            {
//                inventoryHighlight.Show(inventoryObject.BoundaryCheck(onGridPosition.x, onGridPosition.y, slot.item.width, slot.item.height));
//                inventoryHighlight.SetSize(slot, size, space);
//                inventoryHighlight.SetParent(this);
//                inventoryHighlight.SetPosition(this, slot, onGridPosition.x, onGridPosition.y);
//            }
//        }

//        InventorySlot GetItem(int x, int y)
//        {
//            return inventoryObject.Slots[inventoryObject.CalculateIndex(x, y)];
//        }

//        protected override void OnRightClick(InventorySlot slot)
//        {
//            inventoryObject.UseItem(slot);
//        }

//        #endregion Methods


//    }
//}
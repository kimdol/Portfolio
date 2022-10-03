using ARPG.InventorySystem.Inventory;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace ARPG.InventorySystem.UIs
{
    [RequireComponent(typeof(EventTrigger))]
    public abstract class InventoryUI : MonoBehaviour
    {
        #region Variables

        public InventoryObject inventoryObject;
        private InventoryObject previousInventory;

        public Dictionary<GameObject, InventorySlot> slotUIs = new Dictionary<GameObject, InventorySlot>();

        protected InventoryHighlight inventoryHighlight;

        #endregion Variables

        #region Unity Methods

        private void Awake()
        {
            CreateSlots();

            for (int i = 0; i < inventoryObject.Slots.Length; i++)
            {
                inventoryObject.Slots[i].parent = inventoryObject;
                inventoryObject.Slots[i].OnPostUpdate += OnPostUpdate;
                inventoryObject.Slots[i].OnPreUpdate += OnPreUpdate;
            }

            AddEvent(gameObject, 
                EventTriggerType.PointerEnter, 
                delegate { OnEnterInterface(gameObject); });
            AddEvent(gameObject, 
                EventTriggerType.PointerExit, 
                delegate { OnExitInterface(gameObject); });

        }

        protected virtual void Start()
        {
            inventoryHighlight = GetComponent<InventoryHighlight>();
            for (int i = 0; i < inventoryObject.Slots.Length; ++i)
            {
                inventoryObject.Slots[i].UpdateSlot(inventoryObject.Slots[i].item, 
                    inventoryObject.Slots[i].amount);
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < inventoryObject.Slots.Length; i++)
            {
                inventoryObject.Slots[i].OnPostUpdate -= OnPostUpdate;
                inventoryObject.Slots[i].OnPreUpdate -= OnPreUpdate;
            }
        }

        #endregion Unity Methods

        #region Methods

        public abstract void CreateSlots();

        public virtual void OnPreUpdate(InventorySlot slot)
        {

        }

        public virtual void OnPostUpdate(InventorySlot slot)
        {
            slot.slotUI.transform.GetChild(0).GetComponent<Image>().sprite = 
                slot.item.id < 0 ? null : slot.ItemObject.icon;
            slot.slotUI.transform.GetChild(0).GetComponent<Image>().color = 
                slot.item.id < 0 ? new Color(1, 1, 1, 0) : new Color(1, 1, 1, 1);
            slot.slotUI.GetComponentInChildren<TextMeshProUGUI>().text = 
                slot.item.id < 0 ? 
                string.Empty : (slot.amount == 1 ? string.Empty : slot.amount.ToString("n0"));
        }

        protected void AddEvent(GameObject go, EventTriggerType type, UnityAction<BaseEventData> action)
        {
            EventTrigger trigger = go.GetComponent<EventTrigger>();
            if (!trigger)
            {
                Debug.LogWarning("EventTrigger component를 찾을 수 없습니다!");
                return;
            }

            EventTrigger.Entry eventTrigger = new EventTrigger.Entry { eventID = type };
            eventTrigger.callback.AddListener(action);
            trigger.triggers.Add(eventTrigger);
        }

        public void OnEnterInterface(GameObject go)
        {
            MouseData.interfaceMouseIsOver = go.GetComponent<InventoryUI>();
        }
        public void OnExitInterface(GameObject go)
        {
            MouseData.interfaceMouseIsOver = null;
        }


        public void OnEnter(GameObject go)
        {
            MouseData.slotHoveredOver = go;
        }

        public void OnExit(GameObject go)
        {
            MouseData.slotHoveredOver = null;
        }


        public virtual void OnStartDrag(GameObject go)
        {
            MouseData.tempItemBeingDragged = CreateDragImage(go);
        }

        private GameObject CreateDragImage(GameObject go)
        {
            if (slotUIs[go].item.id < 0)
            {
                return null;
            }

            GameObject dragImage = new GameObject();

            RectTransform rectTransform = dragImage.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(100, 100);
            dragImage.transform.SetParent(transform.parent);
            Image image = dragImage.AddComponent<Image>();
            image.sprite = slotUIs[go].ItemObject.icon;
            image.raycastTarget = false;

            dragImage.name = "Drag Image";

            return dragImage;
        }

        public virtual void OnDrag(GameObject go)
        {
            if (MouseData.tempItemBeingDragged == null)
            {
                return;
            }

            MouseData.tempItemBeingDragged.GetComponent<RectTransform>().position = Input.mousePosition;

            if (MouseData.interfaceMouseIsOver == null)
            {
                inventoryHighlight.Show(false);
                return;
            }

            if (MouseData.slotHoveredOver == null)
            {
                inventoryHighlight.Show(false);
                return;
            }

            MouseData.interfaceMouseIsOver.CreateHighlighter(slotUIs[go]);
        }

        public virtual void CreateHighlighter(InventorySlot slot)
        {

        }

        public virtual void OnEndDrag(GameObject go)
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
                this.RemoveItem(go);
            }
            else if(MouseData.interfaceMouseIsOver is StoreInventoryUI)
            {
                UnityEngine.Debug.Log("마우스가 StoreInventoryUI에 올려져있습니다.");
            }
            else if (MouseData.slotHoveredOver)
            {
                InventorySlot locTrSlot = new InventorySlot();
                InventorySlot overlapItem = new InventorySlot();

                if (MouseData.interfaceMouseIsOver.PlaceItem(slotUIs[go], ref overlapItem, ref locTrSlot))
                {
                    MouseData.interfaceMouseIsOver.ReadySwap(ref overlapItem, ref locTrSlot);
                    inventoryObject.SwapItems(slotUIs[go], locTrSlot);
                }
            }
        }

        public virtual void DestroyImage()
        {
            Destroy(MouseData.tempItemBeingDragged);
        }

        public virtual void RemoveItem(GameObject go)
        {
            slotUIs[go].RemoveItem();
        }

        public virtual bool PlaceItem(InventorySlot slot, ref InventorySlot overlapItem, ref InventorySlot locTrSlot)
        {
            InventorySlot mouseHoverSlotData =
                    MouseData.interfaceMouseIsOver.slotUIs[MouseData.slotHoveredOver];

            locTrSlot = mouseHoverSlotData;

            return true;
        }

        public virtual void ReadySwap(ref InventorySlot overlapItem, ref InventorySlot locTrSlot)
        {

        }

        public void OnClick(GameObject go, PointerEventData data)
        {
            InventorySlot slot = slotUIs[go];
            if (slot == null)
            {
                return;
            }

            if (data.button == PointerEventData.InputButton.Left)
            {
                OnLeftClick(slot);
            }
            else if (data.button == PointerEventData.InputButton.Right)
            {
                OnRightClick(slot);
            }
        }

        protected virtual void OnRightClick(InventorySlot slot)
        {
        }

        protected virtual void OnLeftClick(InventorySlot slot)
        {
        }

        #endregion Methods
    }
}

using ARPG.InventorySystem.Inventory;
using ARPG.InventorySystem.Items;
using ARPG.QuestSystem;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Newtonsoft.Json;

namespace ARPG.InventorySystem.Inventory
{
    [CreateAssetMenu(fileName = "New Invnetory", menuName = "Inventory System/Inventory")]
    public class InventoryObject : ScriptableObject
    {
        #region Variables

        public ItemDatabaseObject database;
        public InterfaceType type;

        [SerializeField]
        private Inventory container = new Inventory();

        public Action<ItemObject> OnUseItem;

        [HideInInspector]
        public Vector2 start;
        [HideInInspector]
        public Vector2 size;
        [HideInInspector]
        public Vector2 space;
        [HideInInspector]
        public int numberOfColumn;

        #endregion Variables

        #region Properties

        public InventorySlot[] Slots => container.slots;

        public int EmptySlotCount
        {
            get
            {
                int counter = 0;
                foreach (InventorySlot slot in Slots)
                {
                    if (slot.item.id <= -1)
                    {
                        counter++;
                    }
                }

                return counter;
            }
        }

        #endregion Properties

        #region Unity Methods



        #endregion Unity Methods

        #region Methods
        public bool AddItem(Item item, int amount)
        {
            InventorySlot slot = FindItemInInventory(item);
            if (!database.itemObjects[item.id].stackable || slot == null)
            {
                if (EmptySlotCount <= 0)
                {
                    return false;
                }

                InventorySlot emptySlot = GetEmptySlot();
                if (item.width > 1 || item.height > 1)
                {
                    emptySlot.haveSubSlotUI = true;
                }
                // item을 add함
                emptySlot.UpdateSlot(item, amount);
            }
            else
            {
                slot.AddAmount(amount);
            }

            QuestManager.Instance.ProcessQuest(QuestType.AcquireItem, 1);

            return true;
        }

        public InventorySlot FindItemInInventory(Item item)
        {
            return Slots.FirstOrDefault(i => i.item.id == item.id);
        }

        public bool IsContainItem(ItemObject itemObject)
        {
            return Slots.FirstOrDefault(i => i.item.id == itemObject.data.id) != null;
        }

        public InventorySlot GetEmptySlot()
        {
            return Slots.FirstOrDefault(i => i.item.id <= -1);
        }

        public void SwapItems(InventorySlot itemA, InventorySlot itemB)
        {
            if (itemA == itemB)
            {
                return;
            }

            if (itemB.CanPlaceInSlot(itemA.ItemObject) && itemA.CanPlaceInSlot(itemB.ItemObject))
            {
                InventorySlot temp = new InventorySlot(itemB.item, itemB.amount, itemB.haveSubSlotUI);

                if (itemB != null)
                {
                    CleanGridReference(itemB, itemB.parent.type);
                }
                itemB.UpdateSlot(itemA.item, itemA.amount, itemA.haveSubSlotUI);
                
                CleanGridReference(itemA, itemA.parent.type);
                itemA.UpdateSlot(temp.item, temp.amount, temp.haveSubSlotUI);
            }
        }

        public bool PlaceItem(InventorySlot inventoryItem, int posX, int posY, ref InventorySlot overlapItem)
        {
            if (BoundaryCheck(posX, posY, inventoryItem.item.width, inventoryItem.item.height) == false)
            {
                return false;
            }

            if (OverlapCheck(posX, posY, inventoryItem.item.width, inventoryItem.item.height, ref overlapItem) == false)
            {
                overlapItem = null;
                return false;
            }

            Vector3 pos = inventoryItem.slotUI.GetComponent<RectTransform>().anchoredPosition;
            Vector2Int onGridPosition = CalculateTileGridPosition(pos);
            if (BoundaryCheck(onGridPosition.x, onGridPosition.y, overlapItem.item.width, overlapItem.item.height) == false)
            {
                return false;
            }

            if (SwapVirtualSimulation(inventoryItem, posX, posY, overlapItem) == false)
            {
                return false;
            }

            return true;
        }

        #endregion Methods

        #region Helper Methods

        public bool CanPlaceSubSlot(InventorySlot inventoryItem, int posX, int posY, ref InventorySlot overlapItem)
        {
            if (BoundaryCheck(posX, posY, inventoryItem.item.width, inventoryItem.item.height) == false)
            {
                return false;
            }

            if (OverlapCheck(posX, posY, inventoryItem.item.width, inventoryItem.item.height, ref overlapItem) == false)
            {
                overlapItem = null;
                return false;
            }

            return true;
        }

        public void PlaceItem(InventorySlot inventoryItem, int posX, int posY)
        {
            Vector2 position = CalculatePositionOnGrid(inventoryItem, posX, posY);

            for (int x = 0; x < inventoryItem.item.width; x++)
            {
                for (int y = 0; y < inventoryItem.item.height; y++)
                {
                    // slots에 "놓으려는 아이템 데이터"로 갈아치움
                    Slots[CalculateIndex(posX + x, posY + y)].haveSubSlotUI = false;
                    Slots[CalculateIndex(posX + x, posY + y)].subSlotUIPos = position;
                    Slots[CalculateIndex(posX + x, posY + y)].UpdateSlot(inventoryItem.item, inventoryItem.amount);
                }
            }

            InventorySlot targetSlot = Slots[CalculateIndex(posX, posY)];
            targetSlot.haveSubSlotUI = true;
            targetSlot.subSlotUIPos = position;
        }

        public Vector2Int CalculateTileGridPosition(Vector3 pos)
        {
            Vector2Int onGridPosition = new Vector2Int();

            onGridPosition.x = (int)((pos.x - start.x) / (space.x + size.x));
            onGridPosition.y = (int)((pos.y - start.y) / -(space.y + size.y));

            return onGridPosition;
        }

        public Vector2Int CalculateTileGridPosition(InventorySlot inventoryItem, Vector2 position)
        {
            float slotWidth = inventoryItem.ItemObject.data.width;
            float slotHeight = inventoryItem.ItemObject.data.height;

            Vector2Int pos = new Vector2Int();

            pos.x = (int)((position.x - (start.x - size.x / 2) - ((size.x * slotWidth) + (space.x * (slotWidth - 1))) / 2) / (size.x + space.x));
            pos.y = (int)(-((position.y - (start.y + size.y / 2) + ((size.y * slotHeight) + (space.y * (slotHeight - 1))) / 2) / (size.y + space.y)));

            return pos;
        }

        public int CalculateIndex(int posX, int posY)
        {
            return posX + posY * numberOfColumn;
        }

        public void CleanGridReference(InventorySlot slot, InterfaceType type)
        {
            if (type == InterfaceType.Inventory)
            {
                Vector3 pos = slot.slotUI.GetComponent<RectTransform>().anchoredPosition;
                Vector2Int onGridPosition = slot.parent.CalculateTileGridPosition(pos);

                int itemWidth = slot.item.id < 0 ? 0 : slot.ItemObject.data.width;
                int itemHeight = slot.item.id < 0 ? 0 : slot.ItemObject.data.height;

                for (int ix = 0; ix < itemWidth; ix++)
                {
                    for (int iy = 0; iy < itemHeight; iy++)
                    {
                        Slots[CalculateIndex(onGridPosition.x + ix, onGridPosition.y + iy)].RemoveItem();
                    }
                }
            }
            else
            {
                slot.RemoveItem();
            }
            
        }

        bool PositionCheck(int posX, int posY)
        {
            if (posX < 0 || posY < 0)
            {
                return false;
            }

            if (posX >= numberOfColumn || posY >= Slots.Length / numberOfColumn)
            {
                return false;
            }
            return true;
        }

        public bool BoundaryCheck(int posX, int posY, int width, int height)
        {
            if (PositionCheck(posX, posY) == false) { return false; }

            posX += width - 1;
            posY += height - 1;

            if (PositionCheck(posX, posY) == false) { return false; }

            return true;
        }

        private bool OverlapCheck(int posX, int posY, int width, int height, ref InventorySlot overlapItem)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (Slots[CalculateIndex(posX + x, posY + y)].item.id != -1)
                    {
                        if (overlapItem.item.id == -1)
                        {
                            overlapItem = Slots[CalculateIndex(posX + x, posY + y)];
                        }
                        else
                        {
                            if ((overlapItem.item.width > 1 || overlapItem.item.height > 1))
                            {
                                if (overlapItem.item.id != Slots[CalculateIndex(posX + x, posY + y)].item.id ||
                                overlapItem.subSlotUIPos != Slots[CalculateIndex(posX + x, posY + y)].subSlotUIPos)
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            if ((overlapItem.item.width > 1 || overlapItem.item.height > 1))
            {
                Vector2Int overlapItemPosition = CalculateTileGridPosition(overlapItem, overlapItem.subSlotUIPos);
                overlapItem = Slots[CalculateIndex(overlapItemPosition.x, overlapItemPosition.y)];
            }

            return true;
        }

        private bool SwapVirtualSimulation(InventorySlot inventoryItem, int posX, int posY, InventorySlot overlapItem)
        {
            int[,] inventoryItemSlot = new int[numberOfColumn, Slots.Length / numberOfColumn];
            for (int ix = 0; ix < numberOfColumn; ix++)
            {
                for (int iy = 0; iy < Slots.Length / numberOfColumn; iy++)
                {
                    inventoryItemSlot[ix, iy] = Slots[CalculateIndex(ix, iy)].item.id;
                }
            }

            Vector3 pos = inventoryItem.slotUI.GetComponent<RectTransform>().anchoredPosition;
            Vector2Int prePos = CalculateTileGridPosition(pos);

            for (int ix = 0; ix < inventoryItem.item.width; ix++)
            {
                for (int iy = 0; iy < inventoryItem.item.height; iy++)
                {
                    inventoryItemSlot[posX + ix, posY + iy] = inventoryItem.item.id;
                }
            }

            for (int ix = 0; ix < inventoryItem.item.width; ix++)
            {
                for (int iy = 0; iy < inventoryItem.item.height; iy++)
                {
                    inventoryItemSlot[prePos.x + ix, prePos.y + iy] = -1;
                }
            }

            for (int ix = 0; ix < overlapItem.item.width; ix++)
            {
                for (int iy = 0; iy < overlapItem.item.height; iy++)
                {
                    if (inventoryItemSlot[prePos.x + ix, prePos.y + iy] != -1)
                    {
                        return false;
                    }
                }
            }

            
            return true;
        }

        public Vector2 CalculatePositionOnGrid(InventorySlot inventoryItem, int posX, int posY)
        {
            float slotWidth = inventoryItem.item.width;
            float slotHeight = inventoryItem.item.height;

            Vector2 position = new Vector2();
            position.x = (start.x - size.x / 2) + (posX * (size.x + space.x)) + ((size.x * slotWidth) + (space.x * (slotWidth - 1))) / 2;
            position.y = (start.y + size.y / 2) - ((posY * (size.y + space.y)) + ((size.y * slotHeight) + (space.y * (slotHeight - 1))) / 2);


            return position;
        }

        #endregion Helper Methods

        #region Save/Load Methods
        public string ToJson()
        {
            string jsonString = JsonConvert.SerializeObject(container, Formatting.Indented);
            return jsonString;
        }

        public void FromJson(string jsonString)
        {
            Inventory newContainer = JsonConvert.DeserializeObject<Inventory>(jsonString);
            Debug.Log("from json: " + newContainer.slots.Length);

            for (int i = 0; i < Slots.Length; i++)
            {
                Slots[i].UpdateSlot(newContainer.slots[i].item, newContainer.slots[i].amount);
            }

        }

        public string savePath;

        [ContextMenu("Save")]
        public void Save()
        {
            #region Optional Save
            //string saveData = JsonUtility.ToJson(Container, true);
            //BinaryFormatter bf = new BinaryFormatter();
            //FileStream file = File.Create(string.Concat(Application.persistentDataPath, savePath));
            //bf.Serialize(file, saveData);
            //file.Close();
            #endregion

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), 
                FileMode.Create, 
                FileAccess.Write);
            formatter.Serialize(stream, container);
            stream.Close();
        }

        [ContextMenu("Load")]
        public void Load()
        {
            if (File.Exists(string.Concat(Application.persistentDataPath, savePath)))
            {
                #region Optional Load
                //BinaryFormatter bf = new BinaryFormatter();
                //FileStream file = File.Open(string.Concat(Application.persistentDataPath, savePath),
                //FileMode.Open,
                //FileAccess.Read);
                //JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), Container);
                //file.Close();
                #endregion

                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath),
                    FileMode.Open,
                    FileAccess.Read);
                Inventory newContainer = (Inventory)formatter.Deserialize(stream);
                for (int i = 0; i < Slots.Length; i++)
                {
                    Slots[i].UpdateSlot(newContainer.slots[i].item, newContainer.slots[i].amount);
                }
                stream.Close();
            }
        }

        [ContextMenu("Clear")]
        public void Clear()
        {
            container.Clear();
        }
        #endregion Save/Load Methods

        public void UseItem(InventorySlot slotToUse)
        {
            if (slotToUse.ItemObject == null || slotToUse.item.id < 0 || slotToUse.amount <= 0)
            {
                return;
            }

            ItemObject itemObject = slotToUse.ItemObject;
            slotToUse.UpdateSlot(slotToUse.item, slotToUse.amount - 1);

            OnUseItem.Invoke(itemObject);
        }
    }
}

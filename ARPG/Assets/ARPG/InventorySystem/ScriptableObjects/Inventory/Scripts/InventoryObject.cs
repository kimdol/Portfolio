using ARPG.InventorySystem.Items;
using ARPG.QuestSystem;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

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

        #endregion Variables

        #region Properties
        public Inventory Container => container;


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

                GetEmptySlot().UpdateSlot(item, amount);
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

        public bool SwapItems(InventorySlot itemA, InventorySlot itemB)
        {
            if (itemA == itemB)
            {
                return false;
            }

            if (itemB.CanPlaceInSlot(itemA.ItemObject) && itemA.CanPlaceInSlot(itemB.ItemObject))
            {
                InventorySlot temp = new InventorySlot(itemB.item, itemB.amount);
                itemB.UpdateSlot(itemA.item, itemA.amount);
                itemA.UpdateSlot(temp.item, temp.amount);

                return true;
            }

            return false;
        }

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

        #endregion Methods

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
            Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Create, FileAccess.Write);
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
                //FileStream file = File.Open(string.Concat(Application.persistentDataPath, savePath), FileMode.Open, FileAccess.Read);
                //JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), Container);
                //file.Close();
                #endregion

                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Open, FileAccess.Read);
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


    }
}
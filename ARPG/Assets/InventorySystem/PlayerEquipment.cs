using ARPG.InventorySystem.Inventory;
using ARPG.InventorySystem.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG.InventorySystem.Character
{
    public class PlayerEquipment : MonoBehaviour
    {
        public InventoryObject equipment;

        private EquipmentCombiner combiner;

        private ItemInstances[] itemInstances = new ItemInstances[8];

        [Header("Default Equipments: H = 0, C = 1, P = 2, B = 3, Pa = 4, G = 5, LW = 6, RW = 7")]
        public ItemObject[] defaultItemObjects = new ItemObject[8];

        // 초기화 함
        void Awake()
        {
            combiner = new EquipmentCombiner(gameObject);

            for (int i = 0; i < equipment.Slots.Length; ++i)
            {
                equipment.Slots[i].OnPreUpdate += OnRemoveItem;
                equipment.Slots[i].OnPostUpdate += OnEquipItem;
            }
        }

        private void Start()
        {
            foreach (InventorySlot slot in equipment.Slots)
            {
                OnEquipItem(slot);
            }
        }

        private void OnEquipItem(InventorySlot slot)
        {
            ItemObject itemObject = slot.ItemObject;
            if (itemObject == null)
            {
                EqupDefaultItemBy(slot.AllowedItems[0]);
                return;
            }

            int index = (int)slot.AllowedItems[0];

            switch (slot.AllowedItems[0])
            {
                case ItemType.Helmet:
                case ItemType.Chest:
                case ItemType.Pants:
                case ItemType.Boots:
                case ItemType.Gloves:
                    itemInstances[index] = EquipSkinnedItem(itemObject);
                    break;

                case ItemType.Pauldrons:
                case ItemType.LeftWeapon:
                case ItemType.RightWeapon:
                    itemInstances[index] = EquipMeshItem(itemObject);
                    break;
            }

            if (itemInstances[index] != null)
            {
                itemInstances[index].name = slot.AllowedItems[0].ToString();
            }
        }

        private ItemInstances EquipSkinnedItem(ItemObject itemObject)
        {
            if (itemObject == null)
            {
                return null;
            }

            Transform itemTransform = combiner.AddLimb(itemObject.modelPrefab, itemObject.boneNames);

            ItemInstances instances = itemTransform.gameObject.AddComponent<ItemInstances>();
            if (instances != null)
            {
                instances.items.Add(itemTransform);
            }

            return instances;
        }

        private ItemInstances EquipMeshItem(ItemObject itemObject)
        {
            if (itemObject == null)
            {
                return null;
            }

            Transform[] itemTransforms = combiner.AddMesh(itemObject.modelPrefab);
            if (itemTransforms.Length > 0)
            {
                ItemInstances instances = new GameObject().AddComponent<ItemInstances>();
                foreach (Transform t in itemTransforms)
                {
                    instances.items.Add(t);
                }

                instances.transform.parent = transform;

                return instances;
            }

            return null;
        }

        private void EqupDefaultItemBy(ItemType type)
        {
            int index = (int)type;

            ItemObject itemObject = defaultItemObjects[index];
            switch (type)
            {
                case ItemType.Helmet:
                case ItemType.Chest:
                case ItemType.Pants:
                case ItemType.Boots:
                case ItemType.Gloves:
                    itemInstances[index] = EquipSkinnedItem(itemObject);
                    break;

                case ItemType.Pauldrons:
                case ItemType.LeftWeapon:
                case ItemType.RightWeapon:
                    itemInstances[index] = EquipMeshItem(itemObject);
                    break;
            }
        }

        private void RemoveItemBy(ItemType type)
        {
            int index = (int)type;
            if (itemInstances[index] != null)
            {
                Destroy(itemInstances[index].gameObject);
                itemInstances[index] = null;
            }
        }

        private void OnRemoveItem(InventorySlot slot)
        {
            ItemObject itemObject = slot.ItemObject;
            if (itemObject == null)
            {
                // "deafult item"들을 destroy함
                RemoveItemBy(slot.AllowedItems[0]);
                return;
            }

            if (slot.ItemObject.modelPrefab != null)
            {
                RemoveItemBy(slot.AllowedItems[0]);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG.InventorySystem.Items
{
    [CreateAssetMenu(fileName = "New Item Database", menuName = "Inventory System/Items/Database")]
    public class ItemDatabaseObject : ScriptableObject
    {
        public ItemObject[] itemObjects;

        public void OnValidate()
        {
            for (int i = 0; i < itemObjects.Length; ++i)
            {
                itemObjects[i].data.id = i;
            }
        }
    }
}
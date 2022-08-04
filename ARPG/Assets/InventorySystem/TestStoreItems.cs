using ARPG.InventorySystem.Inventory;
using ARPG.InventorySystem.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestStoreItems : MonoBehaviour
{
    public InventoryObject inventoryObject;
    public ItemDatabaseObject databaseObject;
    public int StoreItemIndex = 5;
    private Queue<ItemObject> items;

    void Start()
    {
        items = new Queue<ItemObject>();
    }

    public void LoadItems()
    {
        if (items.Count == 0)
        {
            int k = StoreItemIndex;
            for (int i = 0; i < databaseObject.itemObjects.Length - StoreItemIndex; i++)
            {
                items.Enqueue(databaseObject.itemObjects[k++]);
            }
        }
    }

    public void AddNewItem()
    {
        if (items.Count > 0)
        {
            ItemObject newItemObject = items.Dequeue();
            //ItemObject newItemObject = databaseObject.itemObjects[databaseObject.itemObjects.Length - 1];
            Item newItem = new Item(newItemObject);
            inventoryObject.AddItem(newItem, 1);
        }
    }

    public void ClearInventory()
    {
        inventoryObject?.Clear();
    }
}

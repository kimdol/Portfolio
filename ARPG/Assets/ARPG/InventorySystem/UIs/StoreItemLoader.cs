using ARPG.InventorySystem.Inventory;
using ARPG.InventorySystem.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreItemLoader : MonoBehaviour
{
    public InventoryObject inventoryObject;
    public ItemDatabaseObject databaseObject;
    public ItemObject[] storeItems;
    private Queue<ItemObject> items;

    void Start()
    {
        items = new Queue<ItemObject>();
    }

    public void LoadItems()
    {
        if (items.Count == 0)
        {
            for (int i = 0; i < storeItems.Length; i++)
            {
                items.Enqueue(storeItems[i]);
            }
        }
    }

    public void AddNewItem()
    {
        if (items.Count > 0)
        {
            ItemObject newItemObject = items.Dequeue();
            Item newItem = new Item(newItemObject);
            inventoryObject.AddItem(newItem, 1);
        }
    }

    public void OnAddItem()
    {
        LoadItems();
        int itemCount = items.Count;
        for (int i = 0; i < itemCount; i++)
        {
            AddNewItem();
        }
    }

    public void ClearInventory()
    {
        inventoryObject?.Clear();
    }
}

using ARPG.InventorySystem.Inventory;
using ARPG.InventorySystem.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestItems : MonoBehaviour
{
    public InventoryObject equipmentObject;
    public InventoryObject inventoryObject;
    public ItemDatabaseObject databaseObject;

    public void AddNewItem()
    {
        if (databaseObject.itemObjects.Length > 0)
        {
            ItemObject newItemObject = databaseObject.itemObjects[Random.Range(0, databaseObject.itemObjects.Length - 1)];
            //ItemObject newItemObject = databaseObject.itemObjects[databaseObject.itemObjects.Length - 1];
            Item newItem = new Item(newItemObject);

            inventoryObject.AddItem(newItem, 1);
        }
    }

    public void ClearInventory()
    {
        equipmentObject?.Clear();
        inventoryObject?.Clear();
    }
}

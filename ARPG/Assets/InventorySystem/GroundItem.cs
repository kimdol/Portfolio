using ARPG.InventorySystem.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundItem : MonoBehaviour
{
    public ItemObject itemObject;

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (itemObject != null)
        {
            GetComponent<SpriteRenderer>().sprite = itemObject.icon;
        }
#endif
    }
}

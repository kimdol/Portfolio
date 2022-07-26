using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG.InventorySystem.Items
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Items/New Item")]
    public class ItemObject : ScriptableObject
    {
        #region Variables

        public ItemType type;
        public bool stackable;

        public Sprite icon;
        public GameObject modelPrefab;

        public Item data = new Item();

        public List<List<string>> boneNamesList = new List<List<string>>();
        public List<string> boneNames = new List<string>();

        [TextArea(15, 20)]
        public string description;

        #endregion Variables


        #region Unity Methods
        private void OnValidate()
        {
            boneNamesList.Clear();
            boneNames.Clear();
            if (modelPrefab == null || modelPrefab.GetComponentInChildren<SkinnedMeshRenderer>() == null)
            {
                return;
            }

            SkinnedMeshRenderer[] skinnedMeshRenderers = modelPrefab.GetComponentsInChildren<SkinnedMeshRenderer>();
            if (skinnedMeshRenderers.Length > 1)
            {
                foreach (SkinnedMeshRenderer renderer in skinnedMeshRenderers)
                {
                    var bones = renderer.bones;

                    List<string> boneNames = new List<string>();
                    foreach (var t in bones)
                    {
                        boneNames.Add(t.name);
                    }

                    boneNamesList.Add(boneNames);
                }
            }
            else
            {
                if (modelPrefab == null || modelPrefab.GetComponentInChildren<SkinnedMeshRenderer>() == null)
                {
                    return;
                }

                SkinnedMeshRenderer renderer = modelPrefab.GetComponentInChildren<SkinnedMeshRenderer>();
                var bones = renderer.bones;

                foreach (var t in bones)
                {
                    boneNames.Add(t.name);
                }
            }
        }

        #endregion Unity Methods

        #region Methods

        public Item CreateItem()
        {
            Item newItem = new Item(this);
            return newItem;
        }

        #endregion Methods
    }
}
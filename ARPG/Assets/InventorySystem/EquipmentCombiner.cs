using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG.InventorySystem.Character
{
    public class EquipmentCombiner
    {
        #region Variables
        private readonly Dictionary<int, Transform> rootBoneDictionary = new Dictionary<int, Transform>();
        private readonly Transform transform;

        private const string boneTag = "bone";

        #endregion Variables

        #region Methods
        public EquipmentCombiner(GameObject rootGO)
        {
            transform = rootGO.transform;
            TraverseHierarchy(transform);
        }

        public Transform AddLimb(GameObject boneGO, List<string> boneNames)
        {
            Transform limb = ProcessBoneObject(boneGO.GetComponentInChildren<SkinnedMeshRenderer>(), boneNames);
            limb.SetParent(transform);

            return limb;
        }

        private Transform ProcessBoneObject(SkinnedMeshRenderer renderer, List<string> boneNames)
        {
            // Sub-object ����
            Transform boneObject = new GameObject().transform;

            // Renderer �߰���
            SkinnedMeshRenderer meshRenderer = boneObject.gameObject.AddComponent<SkinnedMeshRenderer>();

            Transform[] tempBones = new Transform[boneNames.Count];

            // bone structure ����
            for (int i = 0; i < boneNames.Count; ++i)
            {
                tempBones[i] = rootBoneDictionary[boneNames[i].GetHashCode()];
            }

            // Renderer�� ���� ó����
            meshRenderer.bones = tempBones;
            meshRenderer.sharedMesh = renderer.sharedMesh;
            meshRenderer.materials = renderer.sharedMaterials;

            return boneObject;
        }

        public Transform[] AddMesh(GameObject meshGO)
        {
            Transform[] meshes = ProcessMeshObject(meshGO.GetComponentsInChildren<MeshRenderer>());
            return meshes;
        }

        private Transform[] ProcessMeshObject(MeshRenderer[] meshRenderers)
        {
            List<Transform> meshGOs = new List<Transform>();

            foreach (MeshRenderer renderer in meshRenderers)
            {

                if (renderer.transform.parent != null)
                {
                    Transform parent = rootBoneDictionary[renderer.transform.parent.name.GetHashCode()];
                    GameObject itemGO = GameObject.Instantiate(renderer.gameObject, parent);

                    meshGOs.Add(itemGO.transform);
                }
            }

            return meshGOs.ToArray();
        }

        private void TraverseHierarchy(Transform root)
        {
            foreach (Transform child in root)
            {
                rootBoneDictionary.Add(child.name.GetHashCode(), child);

                TraverseHierarchy(child);
            }
        }
        #endregion Methods
    }
}
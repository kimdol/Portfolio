using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG.SceneUtils
{
    public class PlaceTargetWithMouse : MonoBehaviour
    {
        #region Variables
        // ¶¥¹Ù´Ú¿¡¼­ ¾ó¸¶³ª ¶ß´ÂÁö ¼³Á¤ÇÔ
        public float surfaceOffset = 1.5f;
        public Transform target = null;

        #endregion Variables
        // Update is called once per frame
        void Update()
        {
            if (target)
            {
                transform.position = target.position + Vector3.up * surfaceOffset;
            }
            //if (!Input.GetMouseButtonDown(0))
            //{
            //    return;
            //}

            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //RaycastHit hit;
            //if (!Physics.Raycast(ray, out hit))
            //{
            //    return;
            //}

            //transform.position = hit.point + hit.normal * surfaceOffset;
        }

        public void SetPosition(RaycastHit hit)
        {
            target = null;
            transform.position = hit.point + hit.normal * surfaceOffset;
        }
    }
}
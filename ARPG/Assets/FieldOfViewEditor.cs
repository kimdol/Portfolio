using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ARPG.AI
{
    [CustomEditor(typeof(FieldOfView))]
    public class FieldOfViewEditor : Editor
    {
        void OnSceneGUI()
        {
            FieldOfView fov = (FieldOfView)target;
            // �þ� �Ÿ��� ���� �׷���
            Handles.color = Color.white;
            Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.viewRadius);
        }
    }
}

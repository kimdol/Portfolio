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
            // 시야 거리를 먼저 그려줌
            Handles.color = Color.white;
            Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.viewRadius);
            // 왼쪽 꼭지점과 오른쪽 꼭지점의 방향을 얻어옴
            Vector3 viewAngleA = fov.DirFromAngle(-fov.viewAngle / 2, false);
            Vector3 viewAngleB = fov.DirFromAngle(fov.viewAngle / 2, false);

            Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA * fov.viewRadius);
            Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB * fov.viewRadius);

            Handles.color = Color.red;
            foreach (Transform visibleTarget in fov.VisibleTargets)
            {
                if (fov.NearestTarget != visibleTarget)
                {
                    Handles.DrawLine(fov.transform.position, visibleTarget.position);
                }
            }

            Handles.color = Color.green;
            if (fov.NearestTarget)
            {
                Handles.DrawLine(fov.transform.position, fov.NearestTarget.position);
            }
        }
    }
}

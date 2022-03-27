using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ARPG.Cameras
{
    // 커스텀 에디터라는 것을 지정
    [CustomEditor(typeof(TopDownCamera))]
    public class TopDownCamera_SceneEditor : Editor
    {
        #region Variables
        private TopDownCamera targetCamera;
        #endregion

        #region Main Methods
        // targetCamera를 받아오기 위함
        public override void OnInspectorGUI()
        {
            targetCamera = (TopDownCamera)target;
            base.OnInspectorGUI();
        }
        // 받아온 camera에 대한 logic 구현
        private void OnSceneGUI()
        {
            // targetCamera를 가지고 있는지 검사
            if (!targetCamera || !targetCamera.target)
            {
                return;
            }

            // Target, position, lookAtHeight 받아오기
            Transform camTarget = targetCamera.target;
            Vector3 targetPosition = camTarget.position;
            targetPosition.y += targetCamera.lookAtHeight;

            // Distance 표시 그리기
            Handles.color = new Color(1f, 0f, 0f, 0.15f);
            Handles.DrawSolidDisc(targetPosition, Vector3.up, targetCamera.distance);

            Handles.color = new Color(0f, 1f, 0f, 0.75f);
            Handles.DrawWireDisc(targetPosition, Vector3.up, targetCamera.distance);

            // Camera properties을 조정할 슬라이더 핸들을 만들기
            Handles.color = new Color(1f, 0f, 0f, 0.5f);
            targetCamera.distance = Handles.ScaleSlider(targetCamera.distance, 
                targetPosition,
                -camTarget.forward, 
                Quaternion.identity, 
                targetCamera.distance, 0.1f);
            targetCamera.distance = Mathf.Clamp(targetCamera.distance, 2f, float.MaxValue);

            Handles.color = new Color(0f, 0f, 1f, 0.5f);
            targetCamera.height = Handles.ScaleSlider(targetCamera.height, 
                targetPosition, 
                Vector3.up, 
                Quaternion.identity, 
                targetCamera.height, 0.1f);
            targetCamera.height = Mathf.Clamp(targetCamera.height, 2f, float.MaxValue);

            // 라벨 만들기!
            GUIStyle labelStyle = new GUIStyle();
            labelStyle.fontSize = 15;
            labelStyle.normal.textColor = Color.white;
            labelStyle.alignment = TextAnchor.UpperCenter;

            Handles.Label(targetPosition + (-camTarget.forward * targetCamera.distance), "Distance", labelStyle);

            labelStyle.alignment = TextAnchor.MiddleRight;
            Handles.Label(targetPosition + (Vector3.up * targetCamera.height), "Height", labelStyle);

            targetCamera.HandleCamera();
        }

        #endregion
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace ARPG.Cameras
{
    public class TopDownCamera : MonoBehaviour
    {
        #region Variables

        public float height = 5f;
        public float distance = 10f;
        public float angle = 45f;
        public float lookAtHeight = 2f;
        public float smoothSpeed = 0.5f;


        private Vector3 refVelocity;
        public Transform target;
        #endregion

        #region Main Methods
        private void LateUpdate()
        {
            HandleCamera();
        }
        #endregion

        #region Helper Methods
        public void HandleCamera()
        {
            if (!target)
            {
                return;
            }

            // World Pos계산
            Vector3 worldPosition = (Vector3.forward * -distance) + (Vector3.up * height);
            //Debug.DrawLine(target.position, worldPosition, Color.red);

            // 카메라 회전 값 계산
            Vector3 rotatedVector = Quaternion.AngleAxis(angle, Vector3.up) * worldPosition;
            //Debug.DrawLine(target.position, rotatedVector, Color.green);

            // 카메라 위치 재설정
            Vector3 flatTargetPosition = target.position;
            flatTargetPosition.y += lookAtHeight;

            // 최종 카메라 위치 설정
            Vector3 finalPosition = flatTargetPosition + rotatedVector;
            //Debug.DrawLine(target.position, finalPosition, Color.blue);

            // 자연스럽게 이동을 처리
            transform.position = Vector3.SmoothDamp(transform.position, finalPosition, ref refVelocity, smoothSpeed);
            //Vector3 smoothedPosition = Vector3.Lerp(transform.position, finalPosition, m_SmoothSpeed);
            //transform.position = smoothedPosition;

            // 카메라가 타겟을 바라보도록 설정
            transform.LookAt(target.position);
        }

        // 카메라의 위치와 캐릭터가 바라보는 위치를 표시해주는 디버그 함수
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            if (target)
            {
                Vector3 lookAtPosition = target.position;
                lookAtPosition.y += lookAtHeight;
                Gizmos.DrawLine(transform.position, lookAtPosition);
                Gizmos.DrawSphere(lookAtPosition, 0.25f);
            }
            // 현재 카메라 위치 표시(구형태)
            Gizmos.DrawSphere(transform.position, 0.25f);
        }
        #endregion
    }
}


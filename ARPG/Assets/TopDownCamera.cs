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

            // World Pos���
            Vector3 worldPosition = (Vector3.forward * -distance) + (Vector3.up * height);
            //Debug.DrawLine(target.position, worldPosition, Color.red);

            // ī�޶� ȸ�� �� ���
            Vector3 rotatedVector = Quaternion.AngleAxis(angle, Vector3.up) * worldPosition;
            //Debug.DrawLine(target.position, rotatedVector, Color.green);

            // ī�޶� ��ġ �缳��
            Vector3 flatTargetPosition = target.position;
            flatTargetPosition.y += lookAtHeight;

            // ���� ī�޶� ��ġ ����
            Vector3 finalPosition = flatTargetPosition + rotatedVector;
            //Debug.DrawLine(target.position, finalPosition, Color.blue);

            // �ڿ������� �̵��� ó��
            transform.position = Vector3.SmoothDamp(transform.position, finalPosition, ref refVelocity, smoothSpeed);
            //Vector3 smoothedPosition = Vector3.Lerp(transform.position, finalPosition, m_SmoothSpeed);
            //transform.position = smoothedPosition;

            // ī�޶� Ÿ���� �ٶ󺸵��� ����
            transform.LookAt(target.position);
        }

        // ī�޶��� ��ġ�� ĳ���Ͱ� �ٶ󺸴� ��ġ�� ǥ�����ִ� ����� �Լ�
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
            // ���� ī�޶� ��ġ ǥ��(������)
            Gizmos.DrawSphere(transform.position, 0.25f);
        }
        #endregion
    }
}


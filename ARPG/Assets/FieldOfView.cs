using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG.AI
{
    public class FieldOfView : MonoBehaviour
    {
        #region Variables

        [Header("Sight Settings")]
        public float viewRadius = 5f;
        [Range(0, 360)]
        public float viewAngle = 90f;

        [Header("Find Settings")]
        public float delay = 0.2f;

        public LayerMask targetMask;
        public LayerMask obstacleMask;

        private List<Transform> visibleTargets = new List<Transform>();
        // 가장 가까이 있는 타겟에 대한 변수
        private Transform nearestTarget;
        private float distanceToTarget = 0.0f;
        #endregion Variables

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            FindVisibleTargets();
        }

        #region Logic Methods
        void FindVisibleTargets()
        {
            distanceToTarget = 0.0f;
            nearestTarget = null;
            visibleTargets.Clear();
            // 기본적인 타겟 검색함
            Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
            // 시야각과 장애물을 적용함
            for (int i = 0; i < targetsInViewRadius.Length; i++)
            {
                Transform target = targetsInViewRadius[i].transform;
                
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
                {
                    float dstToTarget = Vector3.Distance(transform.position, target.position);

                    if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                    {
                        visibleTargets.Add(target);

                        if (nearestTarget == null || (distanceToTarget > dstToTarget))
                        {
                            nearestTarget = target;
                        }

                        distanceToTarget = dstToTarget;
                        Debug.Log(distanceToTarget);
                    }
                }
            }
        }
        #endregion Logic Methods
    }
}


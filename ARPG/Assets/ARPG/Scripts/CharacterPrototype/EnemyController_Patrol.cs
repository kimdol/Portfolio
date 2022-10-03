//using ARPG.AI;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.AI;

//namespace ARPG.Characters
//{
//    public class EnemyController : MonoBehaviour
//    {
//        #region Variables
//        protected StateMachine<EnemyController> stateMachine;
//        public StateMachine<EnemyController> StateMachine => stateMachine;

//        private FieldOfView fov;
//        //public LayerMask targetMask;
//        //public Transform target;
//        //public float viewRadius;
//        public float attackRange;
//        #endregion Variables

//        #region Patrol Variables
//        public Transform[] waypoints;

//        [HideInInspector]
//        public Transform targetWaypoint = null;
//        private int waypointIndex = 0;
//        #endregion Patrol Variables


//        #region Properties
//        public Transform Target => fov?.NearestTarget;
//        #endregion Properties

//        #region Unity Methods
//        protected virtual void Start()
//        {
//            stateMachine = new StateMachine<EnemyController>(this, new MoveToWaypointState());
//            IdleState idleState = new IdleState();
//            idleState.isPatrol = true;

//            stateMachine.AddState(new IdleState());
//            stateMachine.AddState(new MoveState());
//            stateMachine.AddState(new AttackState());

//            fov = GetComponent<FieldOfView>();
//        }

//        // Update is called once per frame
//        void Update()
//        {
//            stateMachine.Update(Time.deltaTime);
//        }

//        #endregion Unity Methods

//        #region Helper Methods
//        public virtual bool IsAvailableAttack
//        {
//            get
//            {
//                if (!Target)
//                {
//                    return false;
//                }

//                float distance = Vector3.Distance(transform.position, Target.position);
//                return (distance <= attackRange);
//            }
//        }

//        public virtual Transform SearchEnemy()
//        {
//            return Target;

//            //target = null;

//            //Collider[] targetInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
//            //if (targetInViewRadius.Length > 0)
//            //{
//            //    target = targetInViewRadius[0].transform;
//            //}

//            //return target;
//        }

//        private void OnDrawGizmos()
//        {

//        }

//        public Transform FindNextWaypoint()
//        {
//            targetWaypoint = null;
//            // waypoints가 없는 경우 반환함(에러 방지)
//            if (waypoints.Length == 0)
//            {
//                return targetWaypoint;
//            }

//            // targetWaypoint를 waypoints의 첫번째 인덱스로 설정함
//            targetWaypoint = waypoints[waypointIndex];

//            // waypointIndex를 순환함
//            waypointIndex = (waypointIndex + 1) % waypoints.Length;

//            return targetWaypoint;
//        }

//        #endregion Helper Methods


//    }
//}

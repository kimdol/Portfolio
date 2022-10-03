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

//        #region Properties
//        public Transform Target => fov?.NearestTarget;
//        #endregion Properties

//        #region Unity Methods
//        protected virtual void Start()
//        {
//            stateMachine = new StateMachine<EnemyController>(this, new IdleState());
//            stateMachine.AddState(new MoveState());
//            stateMachine.AddState(new AttackState());

//            fov = GetComponent<FieldOfView>();
//        }

//        // Update is called once per frame
//        void Update()
//        {
//            stateMachine.Update(Time.deltaTime);
//            Debug.Log(stateMachine?.CurrentState);
//        }

//        #endregion Unity Methods

//        #region Other Methods

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

//        #endregion Other Methods

//        private void OnDrawGizmos()
//        {

//        }

//    }
//}

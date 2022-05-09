using ARPG.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ARPG.Characters
{
    [RequireComponent(typeof(FieldOfView)), RequireComponent(typeof(NavMeshAgent)), RequireComponent(typeof(CharacterController))]
    public abstract class EnemyController : MonoBehaviour
    {
        #region Variables
        protected StateMachine<EnemyController> stateMachine;
        public virtual float AttackRange => 3.0f;

        protected NavMeshAgent agent;
        protected Animator animator;

        private FieldOfView fieldOfView;
        #endregion Variables

        #region Properties
        public Transform Target => fieldOfView.NearestTarget;
        public LayerMask TargetMask => fieldOfView.targetMask;
        #endregion Properties

        #region Unity Methods
        protected virtual void Start()
        {
            stateMachine = new StateMachine<EnemyController>(this, new IdleState());

            agent = GetComponent<NavMeshAgent>();
            agent.updatePosition = false;
            agent.updateRotation = true;

            animator = GetComponent<Animator>();
            fieldOfView = GetComponent<FieldOfView>();
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            stateMachine.Update(Time.deltaTime);
            if (!(stateMachine.CurrentState is MoveState) && !(stateMachine.CurrentState is DeadState))
            {
                FaceTarget();
            }
        }

        void FaceTarget()
        {
            if (Target)
            {
                Vector3 direction = (Target.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, 
                    lookRotation, 
                    Time.deltaTime * 5f);
            }
        }

        private void OnAnimatorMove()
        {
            // NavMeshAgent
            //Vector3 position = agent.nextPosition;
            //animator.rootPosition = agent.nextPosition;
            //transform.position = position;

            // CharacterController
            Vector3 position = transform.position;
            position.y = agent.nextPosition.y;

            animator.rootPosition = position;
            agent.nextPosition = position;

            // RootAnimation
            //Vector3 position = animator.rootPosition;
            //position.y = agent.nextPosition.y;

            //agent.nextPosition = position;
            //transform.position = position;
        }

        #endregion Unity Methods

        #region Helper Methods
        public virtual bool IsAvailableAttack => false;

        public R ChangeState<R>() where R : State<EnemyController>
        {
            return stateMachine.ChangeState<R>();
        }

        #endregion Helper Methods


    }
}

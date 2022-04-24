using ARPG.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ARPG.Characters
{
    public class EnemyController_FOV : EnemyController
    {
        #region Variables
        protected StateMachine<EnemyController> stateMachine;
        public StateMachine<EnemyController> StateMachine => stateMachine;

        private FieldOfView fov;

        #endregion Variables

        #region Unity Methods
        protected override void Start()
        {
            base.Start();

            stateMachine = new StateMachine<EnemyController>(this, new IdleState());
            stateMachine.AddState(new MoveState());
            stateMachine.AddState(new AttackState());

            fov = GetComponent<FieldOfView>();
        }

        // Update is called once per frame
        void Update()
        {
            stateMachine.Update(Time.deltaTime);
            Debug.Log(stateMachine?.CurrentState);
        }

        #endregion Unity Methods

        #region Other Methods

        public override bool IsAvailableAttack
        {
            get
            {
                if (!target)
                {
                    return false;
                }

                float distance = Vector3.Distance(transform.position, target.position);
                return (distance <= attackRange);
            }
        }

        public override Transform SearchEnemy()
        {
            target = null;

            Collider[] targetInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
            if (targetInViewRadius.Length > 0)
            {
                target = targetInViewRadius[0].transform;
            }

            return target;
        }

        #endregion Other Methods

        private void OnDrawGizmos()
        {

        }

    }
}

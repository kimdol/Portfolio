using ARPG.Characters;
using ARPG.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG.Characters
{
    public class IdleState : State<EnemyController>
    {
        public bool isPatrol = false;
        private float minIdleTime = 0.0f;
        private float maxIdleTime = 3.0f;
        private float idleTime = 0.0f;

        private Animator animator;
        private CharacterController controller;

        protected int isMoveHash = Animator.StringToHash("IsMove");
        protected int moveSpeedHash = Animator.StringToHash("MoveSpeed");

        public override void OnInitialized()
        {
            animator = context.GetComponent<Animator>();
            controller = context.GetComponent<CharacterController>();
        }

        public override void OnEnter()
        {
            animator?.SetBool(isMoveHash, false);
            animator?.SetFloat(moveSpeedHash, 0);
            controller?.Move(Vector3.zero);

            if (context is EnemyController_Patrol)
            {
                isPatrol = true;
                idleTime = UnityEngine.Random.Range(minIdleTime, maxIdleTime);
            }
        }

        public override void Update(float deltaTime)
        {
            // 검색된 target이 있을 경우를 처리함
            if (context.Target)
            {
                if (context.IsAvailableAttack)
                {
                    // attack cool time을 체크하고, attack state로 전환함
                    stateMachine.ChangeState<AttackState>();
                }
                else
                {
                    stateMachine.ChangeState<MoveState>();
                }
            }
            else if (isPatrol && stateMachine.ElapsedTimeInState > idleTime)
            {
                stateMachine.ChangeState<MoveToWaypointState>();
            }
        }

        public override void OnExit()
        {
        }
    }
}


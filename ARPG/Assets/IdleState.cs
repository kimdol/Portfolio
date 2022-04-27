using ARPG.Characters;
using ARPG.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG.Characters
{
    public class IdleState : State<EnemyController>
    {
        public bool isPatrol = true;
        private float minIdleTime = 0.0f;
        private float maxIdleTime = 3.0f;
        private float idleTime = 0.0f;

        private Animator animator;
        private CharacterController controller;

        protected int hashMove = Animator.StringToHash("Move");
        protected int hashMoveSpeed = Animator.StringToHash("MoveSpeed");

        public override void OnInitialized()
        {
            animator = context.GetComponent<Animator>();
            controller = context.GetComponent<CharacterController>();
        }

        public override void OnEnter()
        {
            animator?.SetBool(hashMove, false);
            animator?.SetFloat(hashMoveSpeed, 0);
            controller?.Move(Vector3.zero);

            if (isPatrol)
            {
                idleTime = Random.Range(minIdleTime, maxIdleTime);
            }
        }

        public override void Update(float deltaTime)
        {
            // ���� Ÿ���� ã���� move state�� ��ȯ
            Transform enemy = context.SearchEnemy();
            if (enemy)
            {
                Debug.Log(context.IsAvailableAttack);
                if (context.IsAvailableAttack)
                {
                    // ������ �����ϸ� attack state�� ��ȯ
                    stateMachine.ChangeState<AttackState>();
                }
                else
                {
                    stateMachine.ChangeState<MoveState>();
                }
            }   // ���࿡ ���� �߰����� ���� Patrol�̶�� state ��ȯ�� ó����
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


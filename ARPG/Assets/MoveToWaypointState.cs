using ARPG.Characters;
using ARPG.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ARPG.Characters
{
    public class MoveToWaypointState : State<EnemyController>
    {
        private Animator animator;
        private CharacterController controller;
        private NavMeshAgent agent;

        private EnemyController patrolController;

        protected int hashMove = Animator.StringToHash("Move");
        protected int hashMoveSpeed = Animator.StringToHash("MoveSpeed");

        public override void OnInitialized()
        {
            animator = context.GetComponent<Animator>();
            controller = context.GetComponent<CharacterController>();
            agent = context.GetComponent<NavMeshAgent>();
        }

        public override void OnEnter()
        {
            animator?.SetBool(hashMove, false);
            animator?.SetFloat(hashMoveSpeed, 0);
            controller?.Move(Vector3.zero);
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
            }
        }

        public override void OnExit()
        {
        }
    }
}


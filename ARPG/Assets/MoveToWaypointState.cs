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
            if (context.targetWaypoint == null)
            {
                context.FindNextWaypoint();
            }

            if (context.targetWaypoint)
            {
                agent?.SetDestination(context.targetWaypoint.position);
                animator?.SetBool(hashMove, true);
            }
        }

        public override void Update(float deltaTime)
        {
            // ���� Ÿ���� ã���� move state�� ��ȯ��
            Transform enemy = context.SearchEnemy();

            if (enemy)
            {
                if (context.IsAvailableAttack)
                {
                    // ������ �����ϸ� attack state�� ��ȯ��
                    stateMachine.ChangeState<AttackState>();
                }
                else
                {
                    stateMachine.ChangeState<MoveState>();
                }
            }
            else
            {
                // way point ���̸� �ݺ������� �̵���
                if (!agent.pathPending && (agent.remainingDistance <= agent.stoppingDistance))
                {
                    // ���� ��ǥ ������ �˻���
                    Transform nextDest = context.FindNextWaypoint();
                    // nextDest�� ã�Ƴ´ٸ� ó����
                    if (nextDest)
                    {
                        agent.SetDestination(nextDest.position);
                    }
                    // IdleState�� ��ȯ��
                    stateMachine.ChangeState<IdleState>();
                }
                else
                {
                    // �̵��ؾ� �� way point�� �Ÿ��� ���Ҵٸ� �̵��� ó����
                    controller.Move(agent.velocity * Time.deltaTime);
                    animator.SetFloat(hashMoveSpeed, 
                        agent.velocity.magnitude / agent.speed, 
                        .1f, 
                        Time.deltaTime);
                }
            }
        }

        public override void OnExit()
        {
            animator?.SetBool(hashMove, false);
            agent.ResetPath();
        }
    }
}


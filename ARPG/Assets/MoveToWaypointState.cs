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
        #region Variables

        private Animator animator;
        private CharacterController controller;
        private NavMeshAgent agent;

        private EnemyController_Patrol patrolController;

        private Transform targetWaypoint = null;
        private int waypointIndex = 0;


        private int isMoveHash = Animator.StringToHash("IsMove");
        private int moveSpeedHash = Animator.StringToHash("MoveSpeed");

        #endregion Variables

        #region Properties

        private Transform[] Waypoints => ((EnemyController_Patrol)context)?.waypoints;

        #endregion Properties

        public override void OnInitialized()
        {
            animator = context.GetComponent<Animator>();
            controller = context.GetComponent<CharacterController>();
            agent = context.GetComponent<NavMeshAgent>();

            patrolController = context as EnemyController_Patrol;
        }

        public override void OnEnter()
        {
            agent.stoppingDistance = 0.0f;

            if (targetWaypoint == null)
            {
                FindNextWaypoint();
            }

            if (targetWaypoint)
            {
                animator?.SetBool(isMoveHash, true);
                agent.SetDestination(targetWaypoint.position);
            }
            else
            {
                stateMachine.ChangeState<IdleState>();
            }
        }

        public override void Update(float deltaTime)
        {
            if (context.Target)
            {
                if (context.IsAvailableAttack)
                {
                    // attack cool time�� üũ�ϰ�, attack state�� ��ȯ��
                    stateMachine.ChangeState<AttackState>();
                }
                else
                {
                    stateMachine.ChangeState<MoveState>();
                }
            }
            else
            {

                if (!agent.pathPending && (agent.remainingDistance <= agent.stoppingDistance))
                {
                    FindNextWaypoint();
                    stateMachine.ChangeState<IdleState>();
                }
                else
                {
                    controller.Move(agent.velocity * Time.deltaTime);
                    animator.SetFloat(moveSpeedHash, 
                        agent.velocity.magnitude / agent.speed, 
                        .1f, 
                        Time.deltaTime);
                }
            }
        }

        public override void OnExit()
        {
            agent.stoppingDistance = context.AttackRange;
            animator?.SetBool(isMoveHash, false);
            agent.ResetPath();
        }

        public Transform FindNextWaypoint()
        {
            targetWaypoint = null;

            // Waypoints�� �������� �ʾҴٸ� �׳� ��ȯ��
            if (Waypoints != null && Waypoints.Length > 0)
            {

                // Agent�� ���� ������ �������� �̵��ϵ��� Set��
                targetWaypoint = Waypoints[waypointIndex];

                // �迭���� ���� ������ �����ϰ�, �ʿ��ϴٸ� ���� �������� ��ȯ��
                waypointIndex = (waypointIndex + 1) % Waypoints.Length;
            }

            return targetWaypoint;
        }
    }
}


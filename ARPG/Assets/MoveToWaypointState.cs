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
                    // attack cool time을 체크하고, attack state로 전환함
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

            // Waypoints가 설정되지 않았다면 그냥 반환함
            if (Waypoints != null && Waypoints.Length > 0)
            {

                // Agent가 현재 선택한 목적지로 이동하도록 Set함
                targetWaypoint = Waypoints[waypointIndex];

                // 배열에서 다음 지점을 선택하고, 필요하다면 시작 지점까지 순환함
                waypointIndex = (waypointIndex + 1) % Waypoints.Length;
            }

            return targetWaypoint;
        }
    }
}


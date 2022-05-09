//using ARPG.Characters;
//using ARPG.AI;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.AI;

//namespace ARPG.Characters
//{
//    public class MoveToWaypointState : State<EnemyController>
//    {
//        private Animator animator;
//        private CharacterController controller;
//        private NavMeshAgent agent;

//        private EnemyController patrolController;

//        protected int isMoveHash = Animator.StringToHash("IsMove");
//        protected int moveSpeedHash = Animator.StringToHash("MoveSpeed");

//        public override void OnInitialized()
//        {
//            animator = context.GetComponent<Animator>();
//            controller = context.GetComponent<CharacterController>();
//            agent = context.GetComponent<NavMeshAgent>();
//        }

//        public override void OnEnter()
//        {
//            if (context.targetWaypoint == null)
//            {
//                context.FindNextWaypoint();
//            }

//            if (context.targetWaypoint)
//            {
//                agent?.SetDestination(context.targetWaypoint.position);
//                animator?.SetBool(isMoveHash, true);
//            }
//        }

//        public override void Update(float deltaTime)
//        {
//            // 만약 타겟을 찾으면 move state로 전환함
//            Transform enemy = context.SearchEnemy();

//            if (enemy)
//            {
//                if (context.IsAvailableAttack)
//                {
//                    // 공격이 가능하면 attack state로 전환함
//                    stateMachine.ChangeState<AttackState>();
//                }
//                else
//                {
//                    stateMachine.ChangeState<MoveState>();
//                }
//            }
//            else
//            {
//                // way point 사이를 반복적으로 이동함
//                if (!agent.pathPending && (agent.remainingDistance <= agent.stoppingDistance))
//                {
//                    // 다음 목표 지점을 검색함
//                    Transform nextDest = context.FindNextWaypoint();
//                    // nextDest을 찾아냈다면 처리함
//                    if (nextDest)
//                    {
//                        agent.SetDestination(nextDest.position);
//                    }
//                    // IdleState로 전환함
//                    stateMachine.ChangeState<IdleState>();
//                }
//                else
//                {
//                    // 이동해야 될 way point의 거리가 남았다면 이동을 처리함
//                    controller.Move(agent.velocity * Time.deltaTime);
//                    animator.SetFloat(moveSpeedHash, 
//                        agent.velocity.magnitude / agent.speed, 
//                        .1f, 
//                        Time.deltaTime);
//                }
//            }
//        }

//        public override void OnExit()
//        {
//            animator?.SetBool(isMoveHash, false);
//            agent.ResetPath();
//        }
//    }
//}


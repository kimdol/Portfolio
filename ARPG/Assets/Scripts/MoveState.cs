//using ARPG.AI;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.AI;

//namespace ARPG.Characters
//{
//    public class MoveState : State<EnemyController>
//    {
//        private Animator animator;
//        private CharacterController controller;
//        private NavMeshAgent agent;

//        private int isMoveHash = Animator.StringToHash("Move");
//        private int moveSpeedHash = Animator.StringToHash("MoveSpeed");

//        public override void OnInitialized()
//        {
//            base.OnInitialized();
//            animator = context.GetComponent<Animator>();
//            controller = context.GetComponent<CharacterController>();

//            agent = context.GetComponent<NavMeshAgent>();
//        }

//        public override void OnEnter()
//        {
//            base.OnEnter();
//            agent?.SetDestination(context.target.position);
//            animator?.SetBool(isMoveHash, true);
//        }

//        public override void Update(float deltaTime)
//        {
//            // ���� ���������� �˻��� ������
//            Transform enemy = context.SearchEnemy();
//            // ���� ���� �� ó����
//            if (enemy)
//            {
//                agent.SetDestination(context.target.position);
//                // ��ǥ������ �Ÿ��� ������ �� ó����
//                if (agent.remainingDistance > agent.stoppingDistance)
//                {
//                    controller.Move(agent.velocity * Time.deltaTime);
//                    animator.SetFloat(moveSpeedHash, 
//                        agent.velocity.magnitude / agent.speed, 
//                        .1f,
//                        deltaTime);
//                    return;
//                }
//            }
//            Debug.Log("Ż��");
//            stateMachine.ChangeState<IdleState>();
//        }

//        public override void OnExit()
//        {
//            animator?.SetBool(isMoveHash, false);
//            animator.SetFloat(moveSpeedHash, 0f);
//            // ��ã�� �ʱ�ȭ�Ͽ� ������ ������
//            agent.ResetPath();
//        }
//    }
//}


using ARPG.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ARPG.Characters
{
    public class MoveState : State<EnemyController>
    {
        private Animator animator;
        private CharacterController controller;
        private NavMeshAgent agent;

        private int hashMove = Animator.StringToHash("Move");
        private int hashMoveSpeed = Animator.StringToHash("MoveSpeed");

        public override void OnInitialized()
        {
            base.OnInitialized();
            animator = context.GetComponent<Animator>();
            controller = context.GetComponent<CharacterController>();

            agent = context.GetComponent<NavMeshAgent>();
        }

        public override void OnEnter()
        {
            base.OnEnter();
            agent?.SetDestination(context.target.position);
            animator?.SetBool(hashMove, true);
        }

        public override void Update(float deltaTime)
        {
            // 적을 지속적으로 검색을 수행함
            Transform enemy = context.SearchEnemy();
            // 적이 있을 때 처리함
            if (enemy)
            {
                agent.SetDestination(context.target.position);
                // 목표지점의 거리가 남았을 때 처리함
                if (agent.remainingDistance > agent.stoppingDistance)
                {
                    controller.Move(agent.velocity * Time.deltaTime);
                    animator.SetFloat(hashMoveSpeed, 
                        agent.velocity.magnitude / agent.speed, 
                        .1f,
                        deltaTime);
                    return;
                }
            }
            Debug.Log("탈출");
            stateMachine.ChangeState<IdleState>();
        }

        public override void OnExit()
        {
            animator?.SetBool(hashMove, false);
            animator.SetFloat(hashMoveSpeed, 0f);
            // 길찾기 초기화하여 완전히 종료함
            agent.ResetPath();
        }
    }
}


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

        private int isMoveHash = Animator.StringToHash("IsMove");
        private int moveSpeedHash = Animator.StringToHash("MoveSpeed");

        public override void OnInitialized()
        {
            base.OnInitialized();
            animator = context.GetComponent<Animator>();
            controller = context.GetComponent<CharacterController>();

            agent = context.GetComponent<NavMeshAgent>();
        }

        public override void OnEnter()
        {
            agent.stoppingDistance = context.AttackRange;
            agent?.SetDestination(context.Target.position);

            animator?.SetBool(isMoveHash, true);
        }

        public override void Update(float deltaTime)
        {
            if (context.Target)
            {
                agent.SetDestination(context.Target.position);
            }

            controller.Move(agent.velocity * Time.deltaTime);

            if (agent.remainingDistance > agent.stoppingDistance)
            {
                animator.SetFloat(moveSpeedHash, 
                    agent.velocity.magnitude / agent.speed, 
                    .1f, 
                    Time.deltaTime);
            }
            else
            {

                if (!agent.pathPending)
                {
                    animator.SetFloat(moveSpeedHash, 0);
                    animator.SetBool(isMoveHash, false);
                    agent.ResetPath();

                    stateMachine.ChangeState<IdleState>();
                }
            }
        }

        public override void OnExit()
        {
            agent.stoppingDistance = 0.0f;
            agent.ResetPath();

            animator?.SetBool(isMoveHash, false);
            animator?.SetFloat(moveSpeedHash, 0.0f);
        }
    }
}


//using ARPG.AI;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.AI;

//namespace ARPG.Characters
//{
//    public class AttackState : State<EnemyController>
//    {
//        private Animator animator;

//        protected int hashAttack = Animator.StringToHash("Attack");

//        public override void OnInitialized()
//        {
//            base.OnInitialized();
//            animator = context.GetComponent<Animator>();
//        }

//        public override void OnEnter()
//        {
//            base.OnEnter();
//            if (context.IsAvailableAttack)
//            {
//                animator?.SetTrigger(hashAttack);
//            }
//            else
//            {
//                stateMachine.ChangeState<IdleState>();
//            }
//        }

//        public override void Update(float deltaTime)
//        {
//        }
//    }
//}


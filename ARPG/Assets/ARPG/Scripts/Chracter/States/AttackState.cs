using ARPG.Characters;
using ARPG.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG.AI
{
    public class AttackState : State<EnemyController>
    {
        private Animator animator;
        private AttackStateController attackStateController;
        private IAttackable attackable;

        protected int attackTriggerHash = Animator.StringToHash("AttackTrigger");
        protected int attackIndexHash = Animator.StringToHash("AttackIndex");

        public override void OnInitialized()
        {
            base.OnInitialized();

            animator = context.GetComponent<Animator>();
            attackStateController = context.GetComponent<AttackStateController>();
            attackable = context.GetComponent<IAttackable>();
        }

        public override void OnEnter()
        {
            if (attackable == null || attackable.CurrentAttackBehaviour == null)
            {
                stateMachine.ChangeState<IdleState>();
                return;
            }

            attackStateController.enterAttackHandler += OnEnterAttackState;
            attackStateController.exitAttackHandler += OnExitAttackState;

            animator?.SetInteger(attackIndexHash, attackable.CurrentAttackBehaviour.animationIndex);
            animator?.SetTrigger(attackTriggerHash);
        }

        public override void Update(float deltaTime)
        {
        }

        public override void OnExit()
        {
            attackStateController.enterAttackHandler -= OnEnterAttackState;
            attackStateController.exitAttackHandler -= OnExitAttackState;
        }

        public void OnEnterAttackState()
        {
            //UnityEngine.Debug.Log("OnEnterAttackState()");
        }

        public void OnExitAttackState()
        {
            //UnityEngine.Debug.Log("OnExitAttackState()");
            stateMachine.ChangeState<IdleState>();
        }
    }
}


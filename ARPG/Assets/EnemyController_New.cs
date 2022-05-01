using ARPG.AI;
using ARPG.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ARPG.Characters
{
    public class EnemyController_New : EnemyController, IAttackable, IDamagable
    {
        #region Variables

        [SerializeField]
        public Transform hitPoint;
        // public Transform[] waypoints;

        // public override float AttackRange => CurrentAttackBehaviour?.range ?? 6.0f;

        //[SerializeField]
        //private NPCBattleUI battleUI;

        public float maxHealth => 100f;
        private float health;

        private int hitTriggerHash = Animator.StringToHash("HitTrigger");

        [SerializeField]
        private Transform projectilePoint;

        #endregion Variables

        #region Patrol Variables

        #endregion Patrol Variables

        #region Properties

        #endregion Properties

        #region Unity Methods
        protected override void Start()
        {
            base.Start();

            stateMachine.AddState(new MoveState());
            stateMachine.AddState(new AttackState());
            stateMachine.AddState(new DeadState());

            health = maxHealth;

            //if (battleUI)
            //{
            //    battleUI.MinimumValue = 0.0f;
            //    battleUI.MaximumValue = maxHealth;
            //    battleUI.Value = health;
            //}

            InitAttackBehaviour();
        }

        protected override void Update()
        {
            CheckAttackBehaviour();

            base.Update();
        }

        //private void OnAnimatorMove()
        //{
        //    // Follow NavMeshAgent
        //    //Vector3 position = agent.nextPosition;
        //    //animator.rootPosition = agent.nextPosition;
        //    //transform.position = position;

        //    // Follow CharacterController
        //    Vector3 position = transform.position;
        //    position.y = agent.nextPosition.y;

        //    animator.rootPosition = position;
        //    agent.nextPosition = position;

        //    // Follow RootAnimation
        //    //Vector3 position = animator.rootPosition;
        //    //position.y = agent.nextPosition.y;

        //    //agent.nextPosition = position;
        //    //transform.position = position;
        //}

        #endregion Unity Methods

        #region Helper Methods
        // 초기화 함수
        private void InitAttackBehaviour()
        {
            foreach (AttackBehaviour behaviour in attackBehaviours)
            {
                if (CurrentAttackBehaviour == null)
                {
                    // 제일 처음 공격 행동으로 설정함
                    CurrentAttackBehaviour = behaviour;
                }

                behaviour.targetMask = TargetMask;
            }
        }
        // 어떠한 공격 행동을 결정할지 검사하는 함수
        private void CheckAttackBehaviour()
        {
            // 만약 현재 공격 행동이 없거나, 허용되지 않더라면 새로운 공격 행동을 찾음
            if (CurrentAttackBehaviour == null || !CurrentAttackBehaviour.IsAvailable)
            {
                CurrentAttackBehaviour = null;

                foreach (AttackBehaviour behaviour in attackBehaviours)
                {
                    // 쿨타임이 다 차면을 처리함
                    if (behaviour.IsAvailable)
                    {
                        // 새로 찾은 공격 행동의 우선순위가 높다면을 처리함
                        if ((CurrentAttackBehaviour == null) || 
                            (CurrentAttackBehaviour.priority < behaviour.priority))
                        {
                            CurrentAttackBehaviour = behaviour;
                        }
                    }
                }
            }
        }

        #endregion Helper Methods

        #region IDamagable interfaces

        public bool IsAlive => (health > 0);

        public void TakeDamage(int damage, GameObject hitEffectPrefab)
        {
            if (!IsAlive)
            {
                return;
            }
            // 데미지 차감함
            health -= damage;

            //if (battleUI)
            //{
            //    battleUI.Value = health;
            //    battleUI.TakeDamage(damage);
            //}

            if (hitEffectPrefab)
            {
                Instantiate(hitEffectPrefab, hitPoint);
            }

            if (IsAlive)
            {
                animator?.SetTrigger(hitTriggerHash);
            }
            else
            {
                //if (battleUI != null)
                //{
                //    battleUI.enabled = false;
                //}

                stateMachine.ChangeState<DeadState>();
            }
        }

        #endregion IDamagable interfaces

        #region IAttackable Interfaces

        [SerializeField]
        private List<AttackBehaviour> attackBehaviours = new List<AttackBehaviour>();

        public AttackBehaviour CurrentAttackBehaviour
        {
            get;
            private set;
        }

        public void OnExecuteAttack(int attackIndex)
        {
            if (CurrentAttackBehaviour != null && Target != null)
            {
                CurrentAttackBehaviour.ExecuteAttack(Target.gameObject, projectilePoint);
            }
        }

        #endregion IAttackable Interfaces

    }
}

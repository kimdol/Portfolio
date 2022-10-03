using ARPG.AI;
using ARPG.Core;
using ARPG.UIs;
using ARPG.QuestSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG.Characters
{
    public class EnemyController_Patrol : EnemyController, IAttackable, IDamagable
    {
        #region Variables

        [SerializeField]
        public Transform hitPoint;
        public Transform[] waypoints;

        public override float AttackRange => CurrentAttackBehaviour?.range ?? 6.0f;

        [SerializeField]
        private NPCBattleUI battleUI;

        public float maxHealth => 100f;
        private float health;

        private int hitTriggerHash = Animator.StringToHash("HitTrigger");

        [SerializeField]
        private Transform projectilePoint;

        #endregion Variables

        #region Proeprties

        public override bool IsAvailableAttack
        {
            get
            {
                if (!Target)
                {
                    return false;
                }

                float distance = Vector3.Distance(transform.position, Target.position);
                return (distance <= AttackRange);
            }
        }

        #endregion Properties

        #region Unity Methods

        protected override void Start()
        {
            base.Start();

            stateMachine.AddState(new MoveState());
            stateMachine.AddState(new AttackState());
            stateMachine.AddState(new DeadState());
            stateMachine.AddState(new MoveToWaypointState());

            health = maxHealth;

            if (battleUI)
            {
                battleUI.MinimumValue = 0.0f;
                battleUI.MaximumValue = maxHealth;
                battleUI.Value = health;
            }

            InitAttackBehaviour();
        }

        protected override void Update()
        {
            CheckAttackBehaviour();

            base.Update();
        }

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

        public void CheckResurrection()
        {
            if (!isDead)
            {
                return;
            }

            animator?.SetBool("IsAlive", true);
            stateMachine.ChangeState<IdleState>();

            battleUI.enabled = true;

            health = maxHealth;

            if (battleUI)
            {
                battleUI.MinimumValue = 0.0f;
                battleUI.MaximumValue = maxHealth;
                battleUI.Value = health;
            }

            InitAttackBehaviour();
        }

        #endregion Helper Methods

        #region IDamagable interfaces

        private GameObject attacker = null;
        public GameObject Attacker
        {
            get;
            private set;
        }

        public bool IsAlive => (health > 0);

        public void TakeDamage(int damage, GameObject attacker, GameObject hitEffectPrefab)
        {
            if (!IsAlive)
            {
                return;
            }
            // 데미지 차감함
            health -= damage;

            if (battleUI)
            {
                battleUI.Value = health;
                battleUI.TakeDamage(damage);
            }

            if (hitEffectPrefab)
            {
                Instantiate(hitEffectPrefab, hitPoint);
            }

            if (IsAlive)
            {
                this.attacker = attacker;
                animator?.SetTrigger(hitTriggerHash);
            }
            else
            {
                if (battleUI != null)
                {
                    battleUI.enabled = false;
                }

                stateMachine.ChangeState<DeadState>();

                QuestManager.Instance.ProcessQuest(QuestType.DestroyEnemy, 1);
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

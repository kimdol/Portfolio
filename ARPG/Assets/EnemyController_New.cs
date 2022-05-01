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
        // �ʱ�ȭ �Լ�
        private void InitAttackBehaviour()
        {
            foreach (AttackBehaviour behaviour in attackBehaviours)
            {
                if (CurrentAttackBehaviour == null)
                {
                    // ���� ó�� ���� �ൿ���� ������
                    CurrentAttackBehaviour = behaviour;
                }

                behaviour.targetMask = TargetMask;
            }
        }
        // ��� ���� �ൿ�� �������� �˻��ϴ� �Լ�
        private void CheckAttackBehaviour()
        {
            // ���� ���� ���� �ൿ�� ���ų�, ������ �ʴ���� ���ο� ���� �ൿ�� ã��
            if (CurrentAttackBehaviour == null || !CurrentAttackBehaviour.IsAvailable)
            {
                CurrentAttackBehaviour = null;

                foreach (AttackBehaviour behaviour in attackBehaviours)
                {
                    // ��Ÿ���� �� ������ ó����
                    if (behaviour.IsAvailable)
                    {
                        // ���� ã�� ���� �ൿ�� �켱������ ���ٸ��� ó����
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
            // ������ ������
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

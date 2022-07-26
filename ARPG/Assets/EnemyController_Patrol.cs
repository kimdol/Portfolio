using ARPG.AI;
using ARPG.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG.Characters
{
    public class EnemyController_Patrol : EnemyController, IDamagable
    {
        #region Variables

        public Collider weaponCollider;
        public Transform hitPoint;
        public GameObject hitEffect = null;

        public Transform[] waypoints;

        //public float maxHealth = 100f;
        //public float currentHealth = 100f;

        //public NPCBattleUI healthBar;

        #endregion Variables

        #region Proeprties



        #endregion Properties

        #region Unity Methods

        protected override void Start()
        {
            base.Start();

            stateMachine.AddState(new MoveState());
            //stateMachine.AddState(new AttackState());
            stateMachine.AddState(new MoveToWaypointState());

            health = maxHealth;

            //if (healthBar)
            //{
            //    healthBar = GetComponent<NPCBattleUI>();
            //    healthBar.MinimumValue = 0.0f;
            //    healthBar.MaximumValue = maxHealth;
            //    healthBar.Value = health;
            //}
        }

        #endregion Unity Methods

        #region Helper Methods

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

        public void EnableAttackCollider()
        {
            Debug.Log("Attack Event!");
            if (weaponCollider)
            {
                weaponCollider.enabled = true;
            }


            StartCoroutine("DisableAttackCollider");
        }

        IEnumerator DisableAttackCollider()
        {
            yield return new WaitForFixedUpdate();

            if (weaponCollider)
            {
                weaponCollider.enabled = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other != weaponCollider)
            {
                return;
            }

            if (((1 << other.gameObject.layer) & TargetMask) != 0)
            {
                // 1번과 일치함
                Debug.Log("Attack trigger: " + other.name);
                PlayerCharacter playerCharacter = other.gameObject.GetComponent<PlayerCharacter>();
                playerCharacter?.TakeDamage(10, null, hitEffect);

            }

            if (((1 << other.gameObject.layer) & TargetMask) == 0)
            {
                // ignore layer에 있지 않음
            }
        }

        #endregion Helper Methods

        #region IDamagable interfaces

        public float maxHealth = 100f;

        private float health;

        private GameObject attacker = null;

        public bool IsAlive => (health > 0);

        private int hitTriggerHash = Animator.StringToHash("HitTrigger");
        private int isAliveHash = Animator.StringToHash("IsAlive");

        public void TakeDamage(int damage, GameObject attacker, GameObject hitEffectPrefab)
        {
            if (!IsAlive)
            {
                return;
            }

            health -= damage;

            //if (healthBar)
            //{
            //    healthBar.Value = health;
            //}

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
                // healthBar.enabled = false;
                animator?.SetBool(isAliveHash, false);

                Destroy(gameObject, 3.0f);
            }
        }
        #endregion IDamagable interfaces
    }
}

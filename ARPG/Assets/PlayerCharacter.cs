using ARPG.Core;
using ARPG.SceneUtils;
using ARPG.InventorySystem.Inventory;
using ARPG.InventorySystem.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace ARPG.Characters
{
    [RequireComponent(typeof(NavMeshAgent)), RequireComponent(typeof(CharacterController)), RequireComponent(typeof(Animator))]
    public class PlayerCharacter : MonoBehaviour, IAttackable, IDamagable
    {
        [SerializeField]
        private InventoryObject equipment;

        [SerializeField]
        private InventoryObject inventory;

        #region Variables
        public PlaceTargetWithMouse picker;


        private CharacterController controller;
        [SerializeField]
        private LayerMask groundLayerMask;

        private NavMeshAgent agent;
        private Camera camera;

        [SerializeField]
        private Animator animator;

        readonly int moveHash = Animator.StringToHash("Move");
        readonly int moveSpeedHash = Animator.StringToHash("MoveSpeed");
        readonly int fallingHash = Animator.StringToHash("Falling");
        readonly int attackTriggerHash = Animator.StringToHash("AttackTrigger");
        readonly int attackIndexHash = Animator.StringToHash("AttackIndex");
        readonly int hitTriggerHash = Animator.StringToHash("HitTrigger");
        readonly int isAliveHash = Animator.StringToHash("IsAlive");

        [SerializeField]
        private LayerMask targetMask;
        public Transform target;

        public bool IsInAttackState => GetComponent<AttackStateController>()?.IsInAttackState ?? false;

        [SerializeField]
        private Transform hitPoint;

        public float maxHealth = 1000f;
        protected float health;

        #endregion

        #region Main Methods

        void Start()
        {
            inventory.OnUseItem += OnUseItem;

            controller = GetComponent<CharacterController>();

            agent = GetComponent<NavMeshAgent>();
            agent.updatePosition = false;
            agent.updateRotation = true;

            camera = Camera.main;

            health = maxHealth;
        }


        void Update()
        {
            if (!IsAlive)
            {
                return;
            }

            // CheckAttackBehaviour();

            bool isOnUI = EventSystem.current.IsPointerOverGameObject();

            // 왼쪽 마우스 버튼 입력을 처리함
            if (!isOnUI && Input.GetMouseButtonDown(0) && !IsInAttackState)
            {
                // 스크린 화면에서 world로의 ray를 만듬
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);

                // Ray에서 hit을 체크함
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100, groundLayerMask))
                {
                    // RemoveTarget();

                    // Player를 hit한 곳으로 이동함
                    agent.SetDestination(hit.point);

                    if (picker)
                    {
                        picker.SetPosition(hit);
                    }
                }
            }
            else if (!isOnUI && Input.GetMouseButtonDown(1))
            {
                // 스크린 화면에서 world로의 ray를 만듬
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);

                // Ray에서 hit을 체크함
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100))
                {
                    IDamagable damagable = hit.collider.GetComponent<IDamagable>();
                    if (damagable != null && damagable.IsAlive)
                    {
                        // SetTarget(hit.collider.transform, CurrentAttackBehaviour?.range ?? 0);
                    }

                    IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                    if (interactable != null)
                    {
                        SetTarget(hit.collider.transform, interactable.Distance);
                    }
                }
            }

            if (target != null)
            {
                if (target.GetComponent<IDamagable>() != null && !target.GetComponent<IDamagable>().IsAlive)
                {
                    // RemoveTarget();
                }
                else
                {
                    agent.SetDestination(target.position);
                    // FaceToTarget();
                }
            }

            if ((agent.remainingDistance > agent.stoppingDistance))
            {
                controller.Move(agent.velocity * Time.deltaTime);
                animator.SetFloat(moveSpeedHash, agent.velocity.magnitude / agent.speed, .1f, Time.deltaTime);
                animator.SetBool(moveHash, true);
            }
            else
            {
                controller.Move(agent.velocity * Time.deltaTime);

                if (!agent.pathPending)
                {
                    animator.SetFloat(moveSpeedHash, 0);
                    animator.SetBool(moveHash, false);
                    agent.ResetPath();
                }

                if (target != null)
                {
                    if (target.GetComponent<IInteractable>() != null)
                    {
                        IInteractable interactable = target.GetComponent<IInteractable>();
                        if (interactable.Interact(this.gameObject))
                        {
                            RemoveTarget();
                        }
                    }
                    else if (target.GetComponent<IDamagable>() != null)
                    {
                        // AttackTarget();
                    }
                }
            }

        }

        private void OnAnimatorMove()
        {
            Vector3 position = agent.nextPosition;
            animator.rootPosition = agent.nextPosition;
            transform.position = position;
        }
        #endregion Main Methods

        #region Helper Methods

        private void InitAttackBehaviour()
        {
            foreach (AttackBehaviour behaviour in attackBehaviours)
            {
                behaviour.targetMask = targetMask;
            }
        }

        private void CheckAttackBehaviour()
        {
            if (CurrentAttackBehaviour == null || !CurrentAttackBehaviour.IsAvailable)
            {
                CurrentAttackBehaviour = null;

                foreach (AttackBehaviour behaviour in attackBehaviours)
                {
                    if (behaviour.IsAvailable)
                    {
                        if ((CurrentAttackBehaviour == null) || (CurrentAttackBehaviour.priority < behaviour.priority))
                        {
                            CurrentAttackBehaviour = behaviour;
                        }
                    }
                }
            }
        }

        void SetTarget(Transform newTarget, float stoppingDistance)
        {
            target = newTarget;

            agent.stoppingDistance = stoppingDistance;
            agent.updateRotation = false;
            agent.SetDestination(newTarget.transform.position);

            if (picker)
            {
                picker.target = newTarget.transform;
            }
        }

        void RemoveTarget()
        {
            target = null;
            agent.stoppingDistance = 0f;
            agent.updateRotation = true;

            agent.ResetPath();
        }

        void AttackTarget()
        {
            if (CurrentAttackBehaviour == null)
            {
                return;
            }

            if (target != null && !IsInAttackState && CurrentAttackBehaviour.IsAvailable)
            {
                float distance = Vector3.Distance(transform.position, target.transform.position);
                if (distance <= CurrentAttackBehaviour?.range)
                {
                    animator.SetInteger(attackIndexHash, CurrentAttackBehaviour.animationIndex);
                    animator.SetTrigger(attackTriggerHash);
                    //calcAttackCoolTime = 0.0f;
                }
            }
        }

        void FaceToTarget()
        {
            if (target)
            {
                Vector3 direction = (target.transform.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10.0f);
            }
        }

        #endregion Helper Methods

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
            if (CurrentAttackBehaviour != null)
            {
                CurrentAttackBehaviour.ExecuteAttack(target.gameObject);
            }
        }

        #endregion IAttackable Interfaces

        #region IDamagable Interfaces

        public bool IsAlive => health > 0;

        public void TakeDamage(int damage, GameObject damageEffectPrefab)
        {
            if (!IsAlive)
            {
                return;
            }

            health -= damage;

            if (damageEffectPrefab)
            {
                Instantiate<GameObject>(damageEffectPrefab, hitPoint);
            }

            if (IsAlive)
            {
                animator?.SetTrigger(hitTriggerHash);
            }
            else
            {
                animator?.SetBool(isAliveHash, false);
            }
        }

        #endregion IDamagable Interfaces

        #region Inventory
        private void OnUseItem(ItemObject itemObject)
        {
            foreach (ItemBuff buff in itemObject.data.buffs)
            {
                if (buff.stat == CharacterAttribute.Health)
                {
                    this.health += buff.value;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var item = other.GetComponent<GroundItem>();
            if (item)
            {
                if (inventory.AddItem(new Item(item.itemObject), 1))
                {
                    Destroy(other.gameObject);
                }
            }
        }

        public bool PickupItem(PickupItem pickupItem, int amount = 1)
        {

            if (pickupItem.itemObject != null && inventory.AddItem(new Item(pickupItem.itemObject), amount))
            {
                Destroy(pickupItem.gameObject);
                return true;
            }

            return false;
        }
        #endregion Inventory

    }
}

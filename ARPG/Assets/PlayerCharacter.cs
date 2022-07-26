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

        [SerializeField]
        private Transform projectilePoint;

        [SerializeField]
        public StatsObject playerStats;

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

            InitAttackBehaviour();
        }


        void Update()
        {
            if (!IsAlive)
            {
                return;
            }

            CheckAttackBehaviour();

            bool isOnUI = EventSystem.current.IsPointerOverGameObject();

            // 왼쪽 마우스 버튼 입력을 처리함
            if (!isOnUI && Input.GetMouseButtonDown(0))
            {
                // 스크린 화면에서 world로의 ray를 만듬
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);

                // Ray에서 hit을 체크함
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100, groundLayerMask))
                {
                    RemoveTarget();

                    // Player를 hit한 곳으로 이동함
                    agent.SetDestination(hit.point);

                    SetPicker(hit.point);
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
                        SetTarget(hit.collider.transform, CurrentAttackBehaviour?.range ?? 0);

                        SetPicker(hit.collider.transform.position);
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
                if (target.GetComponent<IInteractable>() != null)
                {
                    float calcDistance = Vector3.Distance(target.position, transform.position);
                    float range = target.GetComponent<IInteractable>().Distance;
                    if (calcDistance > range)
                    {
                        SetTarget(target, range);
                    }

                    FaceToTarget();
                }
                else if (!(target.GetComponent<IDamagable>()?.IsAlive ?? false))
                {
                    RemoveTarget();
                }
                else
                {
                    float calcDistance = Vector3.Distance(target.position, transform.position);
                    float range = CurrentAttackBehaviour?.range ?? 2.0f;
                    if (calcDistance > range)
                    {
                        SetTarget(target, range);
                    }

                    FaceToTarget();
                }
            }

            if (agent.pathPending || (agent.remainingDistance > agent.stoppingDistance))
            {
                controller.Move(agent.velocity * Time.deltaTime);
                animator.SetFloat(moveSpeedHash, agent.velocity.magnitude / agent.speed, .1f, Time.deltaTime);
                animator.SetBool(moveHash, true);
            }
            else
            {
                controller.Move(Vector3.zero);

                animator.SetFloat(moveSpeedHash, 0);
                animator.SetBool(moveHash, false);
                agent.ResetPath();

                if (target != null)
                {
                    if (target.GetComponent<IInteractable>() != null)
                    {
                        IInteractable interactable = target.GetComponent<IInteractable>();
                        interactable.Interact(this.gameObject);
                        target = null;
                    }
                    else if (target.GetComponent<IDamagable>() != null)
                    {
                        AttackTarget();
                    }
                }
                else
                {
                    RemovePicker();
                }
            }

        }

        private void OnAnimatorMove()
        {
            // NavMeshAgent
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

            GetComponent<AttackStateController>().enterAttackHandler += OnEnterAttackState;
            GetComponent<AttackStateController>().exitAttackHandler += OnEnterAttackState;
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

            agent.stoppingDistance = stoppingDistance - 0.05f;
            agent.updateRotation = false;
            agent.SetDestination(newTarget.transform.position);

            SetPicker(newTarget.transform.position);
        }

        public void RemoveTarget()
        {
            target = null;
            agent.stoppingDistance = 0.00f;
            agent.updateRotation = true;

            agent.ResetPath();

            RemovePicker();
        }

        private void SetPicker(Vector3 position)
        {
            if (!picker)
            {
                return;
            }

            Vector3 calcPosition = position;
            calcPosition.y += picker.surfaceOffset;
            picker.transform.position = calcPosition;
        }

        private void RemovePicker()
        {
            if (!picker)
            {
                return;
            }

            Vector3 calcPosition = picker.transform.position;
            calcPosition.y = -10;
            picker.transform.position = calcPosition;

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

        public void OnEnterAttackState()
        {
            UnityEngine.Debug.Log("OnEnterAttackState()");
            playerStats.AddMana(-3);
        }

        public void OnExitAttackState()
        {
            UnityEngine.Debug.Log("OnExitAttackState()");
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
                CurrentAttackBehaviour.ExecuteAttack(target?.gameObject, projectilePoint);
            }
        }

        #endregion IAttackable Interfaces

        #region IDamagable Interfaces

        private GameObject attacker = null;

        public bool IsAlive => playerStats.Health > 0;

        public void TakeDamage(int damage, GameObject attacker, GameObject damageEffectPrefab)
        {
            if (!IsAlive)
            {
                return;
            }

            playerStats.AddHealth(-damage);

            if (damageEffectPrefab)
            {
                Instantiate<GameObject>(damageEffectPrefab, hitPoint);
            }

            if (IsAlive)
            {
                this.attacker = attacker;
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
                if (buff.stat == AttributeType.Health)
                {
                    playerStats.AddHealth(buff.value);
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

        public bool PickupItem(ItemObject itemObject, int amount = 1)
        {
            if (itemObject != null)
            {
                return inventory.AddItem(new Item(itemObject), amount);
            }

            return false;
        }
        #endregion Inventory

    }
}

using ARPG.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ARPG.Characters
{
    [RequireComponent(typeof(FieldOfView)), RequireComponent(typeof(NavMeshAgent)), RequireComponent(typeof(CharacterController))]
    public abstract class EnemyController : Actor
    {
        #region Variables
        protected StateMachine<EnemyController> stateMachine;
        public virtual float AttackRange => 3.0f;

        protected NavMeshAgent agent;
        protected Animator animator;

        private FieldOfView fieldOfView;
        #endregion Variables

        #region Properties
        public Transform Target => fieldOfView.NearestTarget;
        public LayerMask TargetMask => fieldOfView.targetMask;
        #endregion Properties

        #region Unity Methods
        protected override void Start()
        {
            base.Start();

            stateMachine = new StateMachine<EnemyController>(this, new IdleState());

            agent = GetComponent<NavMeshAgent>();
            agent.updatePosition = false;
            agent.updateRotation = true;

            animator = GetComponent<Animator>();
            fieldOfView = GetComponent<FieldOfView>();
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();

            stateMachine.Update(Time.deltaTime);
            if (!(stateMachine.CurrentState is MoveState) && !(stateMachine.CurrentState is DeadState))
            {
                FaceTarget();
            }
        }

        void FaceTarget()
        {
            if (Target)
            {
                Vector3 direction = (Target.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    lookRotation,
                    Time.deltaTime * 5f);
            }
        }

        private void OnAnimatorMove()
        {
            // NavMeshAgent
            Vector3 position = agent.nextPosition;
            animator.rootPosition = agent.nextPosition;
            transform.position = position;

            // CharacterController
            //Vector3 position = transform.position;
            //position.y = agent.nextPosition.y;

            //animator.rootPosition = position;
            //agent.nextPosition = position;

            // RootAnimation
            //Vector3 position = animator.rootPosition;
            //position.y = agent.nextPosition.y;

            //agent.nextPosition = position;
            //transform.position = position;
        }

        #endregion Unity Methods

        #region Helper Methods
        public virtual bool IsAvailableAttack => false;

        public R ChangeState<R>() where R : State<EnemyController>
        {
            return stateMachine.ChangeState<R>();
        }

        #endregion Helper Methods

        #region EntranceAndExit Methods

        public enum State : int
        {
            None = -1,  // 사용전
            Ready = 0,  // 준비 완료
            Appear,     // 등장
            Battle,     // 전투중
            Dead,       // 사망
            Disappear,  // 퇴장
        }

        /// <summary>
        /// 현재 상태값
        /// </summary>
        [SerializeField]
        State CurrentState = State.None;

        /// <summary>
        /// 최고 속도
        /// </summary>
        protected const float MaxSpeed = 10.0f;

        /// <summary>
        /// 최고 속도에 이르는 시간
        /// </summary>
        const float MaxSpeedTime = 0.5f;


        /// <summary>
        /// 목표점
        /// </summary>
        [SerializeField]
        protected Vector3 TargetPosition;

        [SerializeField]
        protected float CurrentSpeed;

        /// <summary>
        /// 방향을 고려한 속도 벡터
        /// </summary>
        protected Vector3 CurrentVelocity;

        protected float MoveStartTime = 0.0f; // 이동시작 시간

        protected float LastActionUpdateTime = 0.0f;

        [SerializeField]
        string filePath;

        public string FilePath
        {
            get
            {
                return filePath;
            }
            set
            {
                filePath = value;
            }
        }

        Vector3 AppearPoint;      // 입장시 도착 위치
        Vector3 DisappearPoint;      // 퇴장시 목표 위치

        public void AddList()
        {
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EnemyManager.AddList(this);
        }

        public void Appear(Vector3 targetPos)
        {
            TargetPosition = targetPos;
            CurrentSpeed = MaxSpeed;    // 나타날때는 최고 스피드로 설정

            CurrentState = State.Appear;
            MoveStartTime = Time.time;
        }

        void Arrived()
        {
            CurrentSpeed = 0.0f;    // 도착했으므로 속도는 0
            if (CurrentState == State.Appear)
            {
                SetBattleState();
            }
            else // if (CurrentState == State.Disappear)
            {
                CurrentState = State.None;
                SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EnemyManager.RemoveEnemy(this);
            }
        }

        void Disappear(Vector3 targetPos)
        {
            TargetPosition = targetPos;
            CurrentSpeed = 0.0f;           // 사라질때는 0부터 속도 증가

            CurrentState = State.Disappear;
            MoveStartTime = Time.time;
        }

        protected override void Initialize()
        {
            base.Initialize();

        }

        public override void OnDead()
        {
            base.OnDead();

            InGameSceneMain inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>();
            inGameSceneMain.EnemyManager.RemoveEnemy(this);

            CurrentState = State.Dead;
        }

        public void RemoveList()
        {
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EnemyManager.RemoveList(this);
        }

        public void Reset(SquadronMemberStruct data)
        {
            ResetData(data);
        }

        void ResetData(SquadronMemberStruct data)
        {
            EnemyStruct enemyStruct = SystemManager.Instance.EnemyTable.GetEnemy(data.EnemyID);

            AppearPoint = new Vector3(data.AppearPointX, data.AppearPointY, data.AppearPointZ);             // 입장시 도착 위치 
            DisappearPoint = new Vector3(data.DisappearPointX, data.DisappearPointY, data.DisappearPointZ);    // 퇴장시 목표 위치

            CurrentState = State.Ready;
            LastActionUpdateTime = Time.time;

            isDead = false;      // Enemy는 재사용되므로 초기화시켜줘야 함
        }

        protected virtual void SetBattleState()
        {
            CurrentState = State.Battle;
            LastActionUpdateTime = Time.time;
        }

        protected override void UpdateActor()
        {
            switch (CurrentState)
            {
                case State.None:
                    break;
                case State.Ready:
                    UpdateReady();
                    break;
                case State.Dead:
                    break;
                case State.Appear:
                case State.Disappear:
                    UpdateSpeed();
                    UpdateMove();
                    break;
                case State.Battle:
                    UpdateBattle();
                    break;
                default:
                    Debug.LogError("Undefined State!");
                    break;
            }
        }

        protected virtual void UpdateBattle()
        {
            //if (CurrentState == State.Dead)
            //{
            //    Disappear(DisappearPoint);

            //    LastActionUpdateTime = Time.time;
            //}


            //if (Time.time - LastActionUpdateTime > 1.0f)
            //{
            //    Disappear(DisappearPoint);

            //    LastActionUpdateTime = Time.time;
            //}
        }

        void UpdateMove()
        {
            float distance = Vector3.Distance(TargetPosition, transform.position);
            if (distance < 0.05)
            {
                Arrived();
                return;
            }

            // 이동벡터 계산. 양 벡터의 차를 통해 이동벡터를 구한후 nomalized 로 단위벡터를 구한다. 속도를 곱해 현재 이동할 벡터를 계산
            CurrentVelocity = (TargetPosition - transform.position).normalized * CurrentSpeed;

            // 자연스러운 감속으로 목표지점에 도착할 수 있도록 계산
            // 속도 = 거리 / 시간 이므로 시간 = 거리/속도
            Vector3 pos =
                Vector3.SmoothDamp(transform.position, 
                TargetPosition,
                ref CurrentVelocity, 
                distance / CurrentSpeed, 
                MaxSpeed);

            if (CurrentState == State.Disappear)
            {
                transform.position = pos;
            }
            else
            {
                // transform.position = pos와 같다.
                GetComponent<CharacterController>().Move(pos - transform.position);
            }
        }

        void UpdateFaceMove()
        {
            Vector3 direction = (TargetPosition - transform.position).normalized;
            if (direction == Vector3.zero)
            {
                return;
            }
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation,
                lookRotation,
                Time.deltaTime * 5f);
        }

        void UpdateReady()
        {
            if (Time.time - LastActionUpdateTime > 1.0f)
            {
                Appear(AppearPoint);
            }
        }

        protected void UpdateSpeed()
        {
            // CurrentSpeed 에서 MaxSpeed 에 도달하는 비율을 흐른 시간많큼 계산
            CurrentSpeed = Mathf.Lerp(CurrentSpeed, MaxSpeed, (Time.time - MoveStartTime) / MaxSpeedTime);
        }

        #endregion EntranceAndExit Methods

    }
}

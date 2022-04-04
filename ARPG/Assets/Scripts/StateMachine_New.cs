using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARPG.AI
{
    public abstract class State<T>
    {
        protected StateMachine_New<T> stateMachine;
        protected T context;

        public State()
        {

        }

        internal void SetMachineAndContext(StateMachine_New<T> stateMachine, T context)
        {
            this.stateMachine = stateMachine;
            this.context = context;
            
            OnInitialized();
        }

        /// <summary>
        /// stateMachine과 context가 set후에 바로 호출됨
        /// </summary>
        public virtual void OnInitialized()
        { }

        public virtual void OnEnter()
        { }

        public abstract void Update(float deltaTime);

        public virtual void OnExit()
        { }
    }
    // 더이상 변형이 없는 클래스
    public sealed class StateMachine_New<T>
    {
        private T context;
        public event Action OnChangedState;
        // 현재 상태
        private State<T> currentState;
        public State<T> CurrentState => currentState;
        // 이전 상태(추후 편의를 위함)
        private State<T> previousState;
        public State<T> PreviousState => previousState;
        // 상태로 변환한 이후 흐른 시간
        private float elapsedTimeInState = 0.0f;
        public float ElapsedTimeInState => elapsedTimeInState;
        // 하나의 상태를 초기화하면서 등록하기 위한 자료구조
        private Dictionary<System.Type, State<T>> states = new Dictionary<Type, State<T>>();
        // 초기 상태를 가지고 시작함
        public StateMachine_New(T context, State<T> initialState)
        {
            this.context = context;

            // 초기 상태를 가지기
            AddState(initialState);
            currentState = initialState;
            // 초기 상태가 바로 진행함
            currentState.OnEnter();
        }

        /// <summary>
        /// 상태를 머신에 추가함
        /// </summary>
        public void AddState(State<T> state)
        {
            // 이 state가 등록된 machine을 추가, 이 machine을 소유하고 있는 소유자 추가
            state.SetMachineAndContext(this, context);
            states[state.GetType()] = state;
        }

        /// <summary>
        /// Machine이 업데이트 되면서 현재 진행되고 있는 state에 대한 시간을 추가
        /// </summary>
        public void Update(float deltaTime)
        {
            elapsedTimeInState += deltaTime;

            currentState.Update(deltaTime);
        }

        /// <summary>
        /// 상태에서 다른 상태로 변경함
        /// </summary>
        public R ChangeState<R>() where R : State<T>
        {
            // Type(= 상태)이 동일하면 상태전환 안하고 반환
            var newType = typeof(R);
            if (currentState.GetType() == newType)
            {
                return currentState as R;
            }

            // 현재 State가 존재하면 종료함
            if (currentState != null)
            {
                currentState.OnExit();
            }

            // State를 변경하고, OnEnter() 호출함
            previousState = currentState;
            currentState = states[newType];
            currentState.OnEnter();
            elapsedTimeInState = 0.0f;

            return currentState as R;
        }

    }
}

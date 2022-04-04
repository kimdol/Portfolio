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
        /// stateMachine�� context�� set�Ŀ� �ٷ� ȣ���
        /// </summary>
        public virtual void OnInitialized()
        { }

        public virtual void OnEnter()
        { }

        public abstract void Update(float deltaTime);

        public virtual void OnExit()
        { }
    }
    // ���̻� ������ ���� Ŭ����
    public sealed class StateMachine_New<T>
    {
        private T context;
        public event Action OnChangedState;
        // ���� ����
        private State<T> currentState;
        public State<T> CurrentState => currentState;
        // ���� ����(���� ���Ǹ� ����)
        private State<T> previousState;
        public State<T> PreviousState => previousState;
        // ���·� ��ȯ�� ���� �帥 �ð�
        private float elapsedTimeInState = 0.0f;
        public float ElapsedTimeInState => elapsedTimeInState;
        // �ϳ��� ���¸� �ʱ�ȭ�ϸ鼭 ����ϱ� ���� �ڷᱸ��
        private Dictionary<System.Type, State<T>> states = new Dictionary<Type, State<T>>();
        // �ʱ� ���¸� ������ ������
        public StateMachine_New(T context, State<T> initialState)
        {
            this.context = context;

            // �ʱ� ���¸� ������
            AddState(initialState);
            currentState = initialState;
            // �ʱ� ���°� �ٷ� ������
            currentState.OnEnter();
        }

        /// <summary>
        /// ���¸� �ӽſ� �߰���
        /// </summary>
        public void AddState(State<T> state)
        {
            // �� state�� ��ϵ� machine�� �߰�, �� machine�� �����ϰ� �ִ� ������ �߰�
            state.SetMachineAndContext(this, context);
            states[state.GetType()] = state;
        }

        /// <summary>
        /// Machine�� ������Ʈ �Ǹ鼭 ���� ����ǰ� �ִ� state�� ���� �ð��� �߰�
        /// </summary>
        public void Update(float deltaTime)
        {
            elapsedTimeInState += deltaTime;

            currentState.Update(deltaTime);
        }

        /// <summary>
        /// ���¿��� �ٸ� ���·� ������
        /// </summary>
        public R ChangeState<R>() where R : State<T>
        {
            // Type(= ����)�� �����ϸ� ������ȯ ���ϰ� ��ȯ
            var newType = typeof(R);
            if (currentState.GetType() == newType)
            {
                return currentState as R;
            }

            // ���� State�� �����ϸ� ������
            if (currentState != null)
            {
                currentState.OnExit();
            }

            // State�� �����ϰ�, OnEnter() ȣ����
            previousState = currentState;
            currentState = states[newType];
            currentState.OnEnter();
            elapsedTimeInState = 0.0f;

            return currentState as R;
        }

    }
}

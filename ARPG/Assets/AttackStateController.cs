using ARPG.Characters;
//using ARPG.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStateController : MonoBehaviour
{
    public delegate void OnEnterAttackState();
    public OnEnterAttackState enterAttackHandler;

    public delegate void OnExitAttackState();
    public OnExitAttackState exitAttackHandler;

    public bool IsInAttackState
    {
        get;
        private set;
    }

    private void Start()
    {
        // delegate Handler들을 초기화 함
        enterAttackHandler = new OnEnterAttackState(EnterAttackState);
        exitAttackHandler = new OnExitAttackState(ExitAttackState);
    }

    #region Helper Methods
    public void OnStartOfAttackState()
    {
        IsInAttackState = true;
        enterAttackHandler();
    }

    public void OnEndOfAttackState()
    {
        IsInAttackState = false;
        exitAttackHandler();
    }

    private void EnterAttackState()
    {
    }

    private void ExitAttackState()
    {
    }

    public void OnCheckAttackCollider(int attackIndex)
    {
        // 공격 애니메이션 도중에 어떠한 형태로 적을 검출해서 
        // 데미지를 입힐 것인가 라는 구현을 호출함
        // GetComponent<IAttackable>()?.OnExecuteAttack(attackIndex);
    }

    #endregion Helper Methods
}

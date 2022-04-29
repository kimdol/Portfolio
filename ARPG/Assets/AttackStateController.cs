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
        // delegate Handler���� �ʱ�ȭ ��
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
        // ���� �ִϸ��̼� ���߿� ��� ���·� ���� �����ؼ� 
        // �������� ���� ���ΰ� ��� ������ ȣ����
        // GetComponent<IAttackable>()?.OnExecuteAttack(attackIndex);
    }

    #endregion Helper Methods
}

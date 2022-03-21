using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleRandomStateMachineBehaviour : StateMachineBehaviour
{
    #region Variables
    public int numberOfStates = 3;
    public float minNormTime = 0f;
    public float maxNormTime = 5f;

    protected float randomNormalTime;

    readonly int hashRandomIdle = Animator.StringToHash("RandomIdle");
    #endregion Variables

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // �ٸ� �ִϸ��̼����� ��ȯ�� �ð��� �������� ����
        randomNormalTime = Random.Range(minNormTime, maxNormTime);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // ���� Base layer�� ��� �ƹ��͵� ���� �ʴ´�
        if (animator.IsInTransition(0) && animator.GetCurrentAnimatorStateInfo(0).fullPathHash == stateInfo.fullPathHash)
        {
            animator.SetInteger(hashRandomIdle, -1);
        }

        // randomNormalTime ���� �Ŀ� RandomIdle���� �����ϰ� ����
        if (stateInfo.normalizedTime > randomNormalTime && !animator.IsInTransition(0))
        {
            int ran = Random.Range(0, numberOfStates);
            Debug.Log(ran);
            animator.SetInteger(hashRandomIdle, ran);
        }
    }
}

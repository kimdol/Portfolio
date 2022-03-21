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
        // 다른 애니메이션으로 전환할 시간을 랜덤으로 결정
        randomNormalTime = Random.Range(minNormTime, maxNormTime);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 만약 Base layer일 경우 아무것도 하지 않는다
        if (animator.IsInTransition(0) && animator.GetCurrentAnimatorStateInfo(0).fullPathHash == stateInfo.fullPathHash)
        {
            animator.SetInteger(hashRandomIdle, -1);
        }

        // randomNormalTime 지난 후에 RandomIdle값을 랜덤하게 변경
        if (stateInfo.normalizedTime > randomNormalTime && !animator.IsInTransition(0))
        {
            int ran = Random.Range(0, numberOfStates);
            Debug.Log(ran);
            animator.SetInteger(hashRandomIdle, ran);
        }
    }
}

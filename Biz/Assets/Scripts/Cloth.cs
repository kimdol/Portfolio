using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloth : MonoBehaviour
{
    public const float ZPOS = 20;
    Vector3 mMoveVec;
    [SerializeField]
    float smoothTime = 0.3f;
    Vector3 velocity = Vector3.zero;
    Vector3 mTarget;
    // 움직이게 하는 플래그
    public bool mSlowMoveFlag = false;
    public bool mFastMoveFlag = false;
    


    // Start is called before the first frame update
    void Start()
    {
        mMoveVec = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (mFastMoveFlag)
        {
            Move();
        }
        else if (mSlowMoveFlag)
        {
            SlowMove();
        }
        
    }
    // 순식간에 움직이기
    void Move()
    {
        transform.position = mMoveVec;
        mFastMoveFlag = false;
    }
    // 느리게 움직이기
    void SlowMove()
    {
        SlowMoveVector(mTarget);
        transform.position = mMoveVec;
        if (transform.position == mTarget)
        {
            mSlowMoveFlag = false;
        }
    }
    // 이동 벡터 계산(순식간에 이동)
    public void InputMoveVector(Vector3 touchVec)
    {
        mFastMoveFlag = true;
        mMoveVec = touchVec;
    }
    // 이동 벡터 계산(느리게 이동)
    public void SlowMoveVector(Vector3 targetVec)
    {
        mSlowMoveFlag = true;
        mTarget = targetVec;
        mMoveVec = Vector3.SmoothDamp(transform.position, mTarget, ref velocity, smoothTime);
    }
}

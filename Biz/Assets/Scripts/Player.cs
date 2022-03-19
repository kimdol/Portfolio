using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    GameObject mCrashedObject = null;

    bool mIsCrash = false;
    public bool IsCrash
    {
        get
        {
            return mIsCrash;
        }
        set
        {
            mIsCrash = value;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // 접근 당하면 정답에 따라서 표정 변화함
    private void OnTriggerEnter(Collider other)
    {
        // CrashedObject함수를 위한 코드 한 줄
        mCrashedObject = other.gameObject;


        // 표정 변화
        ChangeExpression();
        // Crash 여부
        mIsCrash = true;
    }
    // 나와 부딪힌 게임 오브젝트를 반환함
    public GameObject CrashedObject()
    {
        // 부딪힌 게임 오브젝트를 가져옴
        GameObject go = mCrashedObject;
        // 그 오브젝트를 리턴함
        return go;
        
    }
    // 표정 변화
    private void ChangeExpression()
    {
        string whatMisson = null;

        // 표정 변화
        transform.GetChild(0).gameObject.SetActive(true);                   // 평상시 표정
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);

        if (SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().GamePointAccumulator.IsItCorrect(out whatMisson))      // 정답일 경우 웃음
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().GamePointAccumulator.SuccessOrNot(true, whatMisson);
        }
        else                                                                // 오답일 경우 찡그림
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(true);
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().GamePointAccumulator.SuccessOrNot(false, whatMisson);
        }
    }
}

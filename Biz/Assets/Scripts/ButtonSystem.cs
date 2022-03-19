using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSystem : MonoBehaviour
{

    int mPage = 0;
    int mPrevPage = 1;
    bool mTriggerFlag = true;

    int LASTPAGE = 3;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (mTriggerFlag)
        {
            ButtonAppear();
            mTriggerFlag = false;
        }
    }

    // 페이지 계산
    public void PageAccumulator(int num)
    {
        mPage += num;
        mTriggerFlag = true;
    }
    // 페이지에 따른 버튼과 실제 페이지가 나타남
    void ButtonAppear()
    {
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().ClothesSetting.Setting(mPage);

        // 만약 첫페이지이거나 마지막페이지일경우 버튼 나타남 토글
        if (mPage == 0)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(false);
        }
        else if (mPage == LASTPAGE)
        {
            transform.GetChild(1).gameObject.SetActive(true);
            transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(true);
        }
        
    }
}

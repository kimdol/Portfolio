using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class JobNameTitle : MonoBehaviour
{
    [SerializeField]
    List<string> mJobNames;

    TextMesh mUpdateText;

    int JobMaxCnt;
    int mRandomIndex;


    // Start is called before the first frame update
    void Start()
    {
        JobMaxCnt = mJobNames.Count;
        ChangeText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Text 바꾸기
    public void ChangeText()
    {
        // 랜덤한 숫자 하나 pick
        // 그 순자의 인덱스를 가진 문자열 pick
        // text = 그 문자열
        mRandomIndex = Random.Range(0, JobMaxCnt);
        mUpdateText = this.GetComponent<TextMesh>();
        mUpdateText.text = mJobNames[mRandomIndex];
        
    }
}

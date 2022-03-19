using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class SpeedWagan : MonoBehaviour
{
    [SerializeField]
    List<string> keys;

    [SerializeField]
    List<TextAsset> values;

    private static SpeedWagan instance = null;
    public static SpeedWagan Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        // 유일하게 존재할 수 있도록 에러 처리
        if (instance != null)
        {
            Debug.LogError("SystemManager is initialized twice!");
            Destroy(gameObject);
            return;
        }

        instance = this;

        // Scene 이동간에 사라지지 않도록 처리
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 이름에 맞는 Text를 대신써드림
    public void WriteText(out string updateText, string jobName)
    {
        int index = JobNameToIndex(jobName);
        if ( index == -1)
        {
            updateText = null;
            return;
        }
        updateText = values[index].text;

    }
    // 직업이름에 따른 index 던지기
    private int JobNameToIndex(string jobName)
    {
        int index = -1;

        index = keys.IndexOf(jobName);

        return index;
    }


}

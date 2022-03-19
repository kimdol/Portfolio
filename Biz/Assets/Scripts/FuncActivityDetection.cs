using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuncActivityDetection : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().Mel.IsCrash)
        {
            MelCrashOff();
        }
            
    }

    // 멜 충돌 감지 플래그 Off
    void MelCrashOff()
    {
        if (SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().GamePointAccumulator.ClothesToMelIsEnd)
        {
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().Mel.IsCrash = false;
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().GamePointAccumulator.ClothesToMelIsEnd = false;
        }
    }
}

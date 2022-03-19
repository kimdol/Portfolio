using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionSceneMain : BaseSceneMain
{
    [SerializeField]
    NextScrollManager nextScrollManager;
    public NextScrollManager NextScrollManager
    {
        get
        {
            return nextScrollManager;
        }
    }

    public void GotoTitleScene()
    {
        DestroyImmediate(SpeedWagan.Instance.gameObject);
        DestroyImmediate(MusicBox.Instance.gameObject);
        // 시스템 매니저를 파괴
        DestroyImmediate(SystemManager.Instance.gameObject);
        SceneController.Instance.LoadSceneImmediate(SceneNameConstants.TitleScene);

    }
}

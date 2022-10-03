using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleSceneMain : BaseSceneMain
{
    protected override void OnStart()
    {

    }

    public void OnStartButton()
    {
        GotoNextScene();
    }

    public void GotoNextScene()
    {
        SceneController.Instance.LoadScene(SceneNameConstants.LoadingScene);
    }
}

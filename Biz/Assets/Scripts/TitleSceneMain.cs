using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSceneMain : BaseSceneMain
{
    public void OnStartButton()
    {
        SceneController.Instance.LoadScene(SceneNameConstants.LoadingScene);
    }
    public void OnClothesButton()
    {
        Inventory.Instance.Prepare();
        SceneController.Instance.LoadScene(SceneNameConstants.CollectionScene);
    }
}

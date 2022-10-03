using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingSceneMain : BaseSceneMain
{
    /// <summary>
    /// 다음 Scene 이동전 대기시간
    /// </summary>
    const float NextSceneIntaval = 3.0f;
    const float ImageUpdateIntaval = 0.05f;

    [SerializeField]
    GameObject LoadingImage;

    float ImageRotation = 0.0f;
    float LastUpdateTime;

    float SceneStartTime;
    bool NextSceneCall = false;

    public UserDataHander userDataHander;

    protected override void OnStart()
    {
        SceneStartTime = Time.time;
        userDataHander.OnClickLoad();
    }

    protected override void UpdateScene()
    {
        base.UpdateScene();

        float currentTime = Time.time;
        if(currentTime - LastUpdateTime > ImageUpdateIntaval)
        {
            LoadingImage.GetComponent<RectTransform>().rotation = 
                Quaternion.Euler(new Vector3(0, 0, ImageRotation));

            ImageRotation += 1.0f;
            if(ImageRotation >= 360.0f)
            {
                ImageRotation = 0.0f;
            }


            LastUpdateTime = currentTime;
        }
        //
        if(currentTime - SceneStartTime > NextSceneIntaval)
        {
            if(!NextSceneCall)
            {
                GotoNextScene();
            }
        }
    }

    void GotoNextScene()
    {
        SceneController.Instance.LoadScene(SceneNameConstants.InGame);
        NextSceneCall = true;
    }

}

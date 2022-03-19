using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameEndPanel : BasePanel
{
    [SerializeField]
    GameObject Success;

    [SerializeField]
    GameObject Fail;

    public override void InitializePanel()
    {
        base.InitializePanel();
        Close();
    }

    public void ShowGameEnd(bool success)
    {
        base.Show();

        if (success)
        {
            Success.SetActive(true);
            Fail.SetActive(false);
        }
        else
        {
            Success.SetActive(false);
            Fail.SetActive(true);
        }
    }

    // 소리 버튼
    public void OnTTS()
    {
        Sprite sp = null;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == "SuccessEnd1")
            {
                for (int k = 0; k < transform.GetChild(i).childCount; k++)
                {
                    if (transform.GetChild(i).GetChild(k).name == "RecentlyCollection")
                    {
                        sp = transform.GetChild(i).GetChild(k).GetComponent<Image>().sprite;
                    }
                }
            }
        }
        MusicBox.Instance.InputPlayMusicName(sp.name);
    }

    public void OnOK()
    {
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().GotoTitleScene();
    }
}

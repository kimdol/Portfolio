using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpansionPanel : BasePanel
{
    [SerializeField]
    Text mExplainText;

    private Sprite mSprite;
    public override void InitializePanel()
    {
        base.InitializePanel();
        base.Close();
    }

    public void ShowExpansionPanel(Sprite sp)
    {
        mSprite = sp;
        base.Show();
        // 확장된 이미지를 보이기
        transform.GetChild(0).GetComponent<Image>().sprite = mSprite;
        // 설명보이기
        JobExplain();
    }

    public void OnOK()
    {
        base.Close();
    }

    // 소리 버튼
    public void OnTTS()
    {
        MusicBox.Instance.InputPlayMusicName(mSprite.name);
    }

    // 스피드웨건 등장
    private void JobExplain()
    {
        string upDateText = null;
        string RecentlyName = mSprite.name;

        SpeedWagan.Instance.WriteText(out upDateText, RecentlyName);
        mExplainText.text = upDateText;

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecentlyCollection : MonoBehaviour
{
    [SerializeField]
    Text mText;

    private Sprite mSprite;

    // Start is called before the first frame update
    void Start()
    {
        mSprite = Inventory.Instance.GetSprite(Inventory.Instance.RecentlyCollection.imageFilePath);
        gameObject.GetComponent<Image>().sprite = mSprite;
        JobExplain();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 스피드웨건 등장
    private void JobExplain()
    {
        string upDateText = null;
        string RecentlyName = mSprite.name;

        SpeedWagan.Instance.WriteText(out upDateText, RecentlyName);
        mText.text = upDateText;

    }
}

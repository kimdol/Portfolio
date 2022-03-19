using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothesSetting : MonoBehaviour
{
    GameObject go = null;

    // Start is called before the first frame update
    void Start()
    {
         
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 페이지에 따른 옷들 세팅
    public void Setting(int page)
    {
        if (go != null)
        {
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().ClothesManager.RemoveClothes(go.GetComponent<Clothes>());
        }
        go = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().ClothesManager.GenerateClothes(page, new Vector3(0, 0, 0));
    }
}

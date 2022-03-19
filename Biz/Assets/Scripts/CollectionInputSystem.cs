using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CollectionInputSystem : MonoBehaviour
{
    // CollctionLeftBotton
    public void OnCollectionLeft()
    {
        CollectionSceneMain scene = SystemManager.Instance.GetCurrentSceneMain<CollectionSceneMain>();
        scene.NextScrollManager.scrollbar.value -= scene.NextScrollManager.Next;


    }
    // CollctionRightBotton
    public void OnCollectionRight()
    {
        CollectionSceneMain scene = SystemManager.Instance.GetCurrentSceneMain<CollectionSceneMain>();
        scene.NextScrollManager.scrollbar.value += scene.NextScrollManager.Next;


    }
    // 확장한 패널 보이기
    public void OnExpantionPanel()
    {
        ExpansionPanel panel = PanelManager.GetPanel(typeof(ExpansionPanel)) as ExpansionPanel;
        panel.ShowExpansionPanel(EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite);
    }
    // 처음 화면으로 가기
    public void OnBack()
    {
        SystemManager.Instance.GetCurrentSceneMain<CollectionSceneMain>().GotoTitleScene();
    }
}

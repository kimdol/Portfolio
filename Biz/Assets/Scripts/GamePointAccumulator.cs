using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePointAccumulator : MonoBehaviour
{
    [SerializeField]
    GameObject CompleteBotton;
    private bool mComplete = false;
    public bool Complete
    {
        get
        {
            return mComplete;
        }
    }

    private CollectionFilePath mCollectionName = null;
    private bool mClothMission = false;
    private bool mToolMission = false;
    private bool mMissionClear = false;
    // private bool mAlreadyDone = false;

    private bool mClothesToMelIsEnd = false;
    public bool ClothesToMelIsEnd
    {
        get
        {
            return mClothesToMelIsEnd;
        }
        set
        {
            mClothesToMelIsEnd = value;
        }
    }
    private string mRecentlyClothName = null;
    private GameObject mRecentlyCloth = null;
    private string mRecentlyToolName = null;
    private GameObject mRecentlyTool = null;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().Mel.IsCrash)
        {
            ClothesToMel();
        }
    }

    // 올바른 답을 찾으면 1, 아니면 0
    public bool IsItCorrect(out string whatmission)
    {
        // 밑의 동작의 수월함을 위한 예외처리
        GameObject go = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().Mel.CrashedObject();
        if (!go)
        {
            Debug.Log("Crashed Gameobject is none");
            whatmission = null;
            return false;
        }
        // "타이틀 이름"과 "player와 부딪힌 오브젝트 이름"을 비교 후
        // 맞으면 true, 틀리면 false
        string text = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().JobNameTitle.GetComponent<TextMesh>().text;
        if (go.name.Contains(text))
        {
            switch (text)
            {
                case "Artist":
                case "화가":
                    mCollectionName = Inventory.CollectionConstants.Artist;
                    break;
                case "Farmer":
                case "농부":
                    mCollectionName = Inventory.CollectionConstants.Farmer;
                    break;
                case "Firefighter":
                case "소방관":
                    mCollectionName = Inventory.CollectionConstants.Firefighter;
                    break;
                case "Carpenter":
                case "목수":
                    mCollectionName = Inventory.CollectionConstants.Carpenter;
                    break;
                case "Hairdresser":
                case "미용사":
                    mCollectionName = Inventory.CollectionConstants.Hairdresser;
                    break;
                case "Police":
                case "경찰":
                    mCollectionName = Inventory.CollectionConstants.Police;
                    break;
                case "Marine":
                case "군인":
                    mCollectionName = Inventory.CollectionConstants.Marine;
                    break;
                case "Doctor":
                case "의사":
                    mCollectionName = Inventory.CollectionConstants.Doctor;
                    break;
                case "Cook":
                case "요리사":
                    mCollectionName = Inventory.CollectionConstants.Cook;
                    break;
                case "Singer":
                case "가수":
                    mCollectionName = Inventory.CollectionConstants.Singer;
                    break;
            }
            if (go.name.Contains("도구"))
            {
                whatmission = "tool";
            }
            else
            {
                whatmission = "cloth";
            }
            return true;
        }
        else
        {
            whatmission = null;
            return false;
        }
    }
    public void SuccessOrNot(bool success, string mission)
    {
        if ((mission == "tool") && (success == true))
        {
            mToolMission = true;
        }
        else if ((mission == "cloth") && (success == true))
        {
            mClothMission = true;
        }

        if (mToolMission && mClothMission)
        {
            Inventory.Instance.GenerateCache(mCollectionName, 1);
            mMissionClear = true;
        }
    }
    public void OnCompleteButton()
    {
        Inventory.Instance.Prepare();
        mComplete = true;
        CompleteBotton.SetActive(false);
        GameEndPanel gameEndPanel = PanelManager.GetPanel(typeof(GameEndPanel)) as GameEndPanel;
        gameEndPanel.ShowGameEnd(mMissionClear);
    }
    // 해당 옷을 멜에게 주면 그 옷을 입어야 하는 룰
    private void ClothesToMel()
    {
        /*해당 옷이 멜에게 놓으면 그 옷을 확대후 입혀라*/
        // 함수를 반드시 1번 실행하게 끔 함
        if (!mClothesToMelIsEnd)
        {
            // 옷이 멜에게 부딪혔을때 옷의 이름을 가져와라
            mRecentlyClothName = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().Mel.CrashedObject().name;

            // 만약 입혀진 옷이 있을 때
            if ((mRecentlyCloth != null) && (!mRecentlyClothName.Contains("도구")))
            {
                // 입혀진 옷을 원래 크기로 만든 후 캐쉬로 반환
                SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().ItemManager.RemoveItem(mRecentlyCloth.GetComponent<Item>());
            }
            // 만약 입혀진 도구가 있을 때
            if ((mRecentlyTool != null) && (mRecentlyClothName.Contains("도구")))
            {
                // 입혀진 옷을 원래 크기로 만든 후 캐쉬로 반환
                SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().ItemManager.RemoveItem(mRecentlyTool.GetComponent<Item>());
            }

            // 그 옷을 Item 캐쉬에서 꺼내와서 적절한 위치에 고정배치하라
            ItemManager itmmng = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().ItemManager;
            if (mRecentlyClothName.Contains("도구"))
            {
                mRecentlyToolName = mRecentlyClothName;
                mRecentlyClothName = null;
                mRecentlyTool = itmmng.GenerateItem(itmmng.ItemNameToIndex(mRecentlyToolName), new Vector3(-5.52f, -0.66f, 19.5f));
                // 함수 종료 플래그 온
                mClothesToMelIsEnd = true;

                return;
            }
            mRecentlyCloth = itmmng.GenerateItem(itmmng.ItemNameToIndex(mRecentlyClothName), new Vector3(-5.52f, -0.66f, 19.5f));
            // 함수 종료 플래그 온
            mClothesToMelIsEnd = true;
        }
    }
}

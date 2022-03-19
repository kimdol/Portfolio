using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameSceneMain : BaseSceneMain
{
    const float GameReadyIntaval = 3.0f;

    public enum GameState : int
    {
        Ready = 0,
        Running,
        End
    }

    GameState currentGameState = GameState.Ready;
    public GameState CurrentGameState
    {
        get
        {
            return currentGameState;
        }
    }

    [SerializeField]
    Player player;

    public Player Mel
    {
        get
        {
            return player;
        }
    }

    [SerializeField]
    ClothesManager clothesManager;
    public ClothesManager ClothesManager
    {
        get
        {
            return clothesManager;
        }
    }

    [SerializeField]
    ItemManager itemManager;
    public ItemManager ItemManager
    {
        get
        {
            return itemManager;
        }
    }

    [SerializeField]
    JobNameTitle jabnameTitle;
    public JobNameTitle JobNameTitle
    {
        get
        {
            return jabnameTitle;
        }
    }

    [SerializeField]
    InputController inputContrller;
    public InputController InputController
    {
        get
        {
            return inputContrller;
        }
    }

    [SerializeField]
    GamePointAccumulator gamePointAccumulator;
    public GamePointAccumulator GamePointAccumulator
    {
        get
        {
            return gamePointAccumulator;
        }
    }

    [SerializeField]
    ClothesSetting clothesSetting;
    public ClothesSetting ClothesSetting
    {
        get
        {
            return clothesSetting;
        }
    }

    [SerializeField]
    ButtonSystem buttonSystem;
    public ButtonSystem ButtonSystem
    {
        get
        {
            return buttonSystem;
        }
    }

    PrefabCacheSystem clothesCacheSystem = new PrefabCacheSystem();
    public PrefabCacheSystem ClothesCacheSystem
    {
        get
        {
            return clothesCacheSystem;
        }
    }

    PrefabCacheSystem itemCacheSystem = new PrefabCacheSystem();
    public PrefabCacheSystem ItemCacheSystem
    {
        get
        {
            return itemCacheSystem;
        }
    }

    [SerializeField]
    CollectionManager collectionManager;
    public CollectionManager CollectionManager
    {
        get
        {
            return collectionManager;
        }
    }

    PrefabCacheSystem collectionCacheSystem = new PrefabCacheSystem();
    public PrefabCacheSystem CollectionCacheSystem
    {
        get
        {
            return collectionCacheSystem;
        }
    }


    [SerializeField]
    Transform mainBGQuadTransform;

    public Transform MainBGQuadTransform
    {
        get
        {
            return mainBGQuadTransform;
        }
    }

    public void OnGameEnd(bool success)
    {
        
    }
    float SceneStartTime;
    protected override void OnStart()
    {
        SceneStartTime = Time.time;
    }
    protected override void UpdateScene()
    {
        base.UpdateScene();

        float currentTime = Time.time;

        if (currentGameState == GameState.Ready)
        {
            if (currentTime - SceneStartTime > GameReadyIntaval)
            {
                /* Game Start!! */
            }
        }
    }
    public void OnClothesButton()
    {
        SceneController.Instance.LoadScene(SceneNameConstants.CollectionScene);
    }

    public void GotoTitleScene()
    {
        DestroyImmediate(MusicBox.Instance.gameObject);
        DestroyImmediate(SpeedWagan.Instance.gameObject);
        // 시스템 매니저를 파괴
        DestroyImmediate(SystemManager.Instance.gameObject);
        SceneController.Instance.LoadSceneImmediate(SceneNameConstants.TitleScene);

    }

}

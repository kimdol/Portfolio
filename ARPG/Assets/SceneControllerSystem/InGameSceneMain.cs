using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class InGameSceneMain : BaseSceneMain
{
    const float GameReadyIntaval = 3.0f;

    public enum GameState : int
    {
        Ready = 0,
        Running,
        End,
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
    EnemyManager enemyManager;

    public EnemyManager EnemyManager
    {
        get
        {
            return enemyManager;
        }
    }

    PrefabCacheSystem enemyCacheSystem = new PrefabCacheSystem();
    public PrefabCacheSystem EnemyCacheSystem
    {
        get
        {
            return enemyCacheSystem;
        }
    }


    [SerializeField]
    SquadronManager squadronManager;
    public SquadronManager SquadronManager
    {
        get
        {
            return squadronManager;
        }
    }

    float SceneStartTime;
    protected override void OnStart()
    {
        base.OnStart();

        SceneStartTime = Time.time;
    }

    protected override void UpdateScene()
    {
        base.UpdateScene();

        float currentTime = Time.time;
        if (CurrentGameState == GameState.Ready)
        {
            if (currentTime - SceneStartTime > GameReadyIntaval)
            {
                SquadronManager.StartGame();
                currentGameState = GameState.Running;
            }
        }
    }

    public void GameStart()
    {

    }

    public void ShowWarningUI()
    {

    }
    

    public void SetRunningState()
    {
        
    }

    public void OnGameEnd(bool success)
    {
        
    }

    public void GotoTitleScene()
    {
        // 시스템 매니저를 파괴
        DestroyImmediate(SystemManager.Instance.gameObject);
        SceneController.Instance.LoadSceneImmediate(SceneNameConstants.TitleScene);

    }
}

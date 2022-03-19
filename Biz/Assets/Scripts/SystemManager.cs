using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemManager : MonoBehaviour
{
    static SystemManager instance = null;

    public static SystemManager Instance
    {
        get
        {
            return instance;
        }
    }

    BaseSceneMain currentSceneMain;
    public BaseSceneMain CurrentSceneMain
    {
        set
        {
            currentSceneMain = value;
        }
    }



    private void Awake()
    {
        // 유일하게 존재할 수 있도록 에러 처리
        if (instance != null)
        {
            Debug.LogError("SystemManager is initialized twice!");
            Destroy(gameObject);
            return;
        }

        instance = this;

        // Scene 이동간에 사라지지 않도록 처리
        DontDestroyOnLoad(gameObject);
    }


    // Start is called before the first frame update
    void Start()
    {
        BaseSceneMain baseSceneMain = GameObject.FindObjectOfType<BaseSceneMain>();
        SystemManager.Instance.CurrentSceneMain = baseSceneMain;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public T GetCurrentSceneMain<T>()
        where T : BaseSceneMain
    {
        return currentSceneMain as T;
    }
}

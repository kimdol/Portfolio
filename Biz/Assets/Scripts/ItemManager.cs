using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField]
    PrefabCacheData[] ItemFiles;

    Dictionary<string, GameObject> FileCache = new Dictionary<string, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        Prepare();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public GameObject GenerateItem(int index, Vector3 position)
    {
        if (index < 0 || index >= ItemFiles.Length)
        {
            Debug.LogError("GenerateEffect error! out of range! index = " + index);
            return null;
        }

        string filePath = ItemFiles[index].filePath;
        GameObject go = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().ItemCacheSystem.Archive(filePath);

        Item item = go.GetComponent<Item>();
        item.FilePath = filePath;

        return go;
    }

    public GameObject Load(string resourcePath)
    {
        GameObject go = null;

        if (FileCache.ContainsKey(resourcePath))   // 캐시 확인
        {
            go = FileCache[resourcePath];
        }
        else
        {
            // 캐시에 없으므로 로드
            go = Resources.Load<GameObject>(resourcePath);
            if (!go)
            {
                Debug.LogError("Load error! path = " + resourcePath);
                return null;
            }
            // 로드 후 캐시에 적재
            FileCache.Add(resourcePath, go);
        }

        return go;
    }
    public void Prepare()
    {
        for (int i = 0; i < ItemFiles.Length; i++)
        {
            GameObject go = Load(ItemFiles[i].filePath);
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().ItemCacheSystem.GenerateCache(ItemFiles[i].filePath, go, ItemFiles[i].cacheCount);
        }
    }
    public bool RemoveItem(Item item)
    {
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().ItemCacheSystem.Restore(item.FilePath, item.gameObject);
        return true;
    }
    // 아이템이름을 index로 바꾸기(실패시 -1)
    public int ItemNameToIndex(string name)
    {
        switch (name)
        {
            case "Artist":
            case "화가":
                return 0;
            case "화가도구":
                return 1;
            case "Farmer":
            case "농부":
                return 8;
            case "농부도구":
                return 9;
            case "Firefighter":
            case "소방관":
                return 10;
            case "소방관도구":
                return 11;
            case "Carpenter":
            case "목수":
                return 2;
            case "목수도구":
                return 3;
            case "Hairdresser":
            case "미용사":
                return 12;
            case "미용사도구":
                return 13;
            case "Police":
            case "경찰":
                return 16;
            case "경찰도구":
                return 17;
            case "Marine":
            case "군인":
                return 14;
            case "군인도구":
                return 15;
            case "Doctor":
            case "의사":
                return 6;
            case "의사도구":
                return 7;
            case "Cook":
            case "요리사":
                return 4;
            case "요리사도구":
                return 5;
            case "Singer":
            case "가수":
                return 18;
            case "가수도구":
                return 19;
        }
        return -1;
    }
}

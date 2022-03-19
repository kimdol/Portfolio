using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothesManager : MonoBehaviour
{
    [SerializeField]
    PrefabCacheData[] ClothesFiles;

    Dictionary<string, GameObject> FileCache = new Dictionary<string, GameObject>();

    public int Clothes01Index = 0;
    public int Clothes02Index = 1;
    public int Clothes03Index = 2;
    public int Clothes04Index = 3;

    // Start is called before the first frame update
    void Start()
    {
        Prepare();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject GenerateClothes(int index, Vector3 position)
    {
        if (index < 0 || index >= ClothesFiles.Length)
        {
            Debug.LogError("GenerateEffect error! out of range! index = " + index);
            return null;
        }
        
        string filePath = ClothesFiles[index].filePath;
        GameObject go = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().ClothesCacheSystem.Archive(filePath);
        go.transform.position = position;
        
        Clothes clothes = go.GetComponent<Clothes>();
        clothes.FilePath = filePath;

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
        for (int i = 0; i < ClothesFiles.Length; i++)
        {
            GameObject go = Load(ClothesFiles[i].filePath);
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().ClothesCacheSystem.GenerateCache(ClothesFiles[i].filePath, go, ClothesFiles[i].cacheCount);
        }
    }
    public bool RemoveClothes(Clothes clothes)
    {
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().ClothesCacheSystem.Restore(clothes.FilePath, clothes.gameObject);
        return true;
    }
}

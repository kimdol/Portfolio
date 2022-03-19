using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionManager : MonoBehaviour
{
    [SerializeField]
    PrefabCacheData[] CollectionFiles;

    Dictionary<string, GameObject> FileCache = new Dictionary<string, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public GameObject Generate(int index, Vector3 position)
    {

        if (index < 0 || index >= CollectionFiles.Length)
        {
            Debug.LogError("Generate error! out of range! index = " + index);
            return null;
        }

        string filePath = CollectionFiles[index].filePath;
        GameObject go = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().CollectionCacheSystem.Archive(filePath);
        go.transform.position = position;

        Collection collection = go.GetComponent<Collection>();
        collection.FilePath = filePath;

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
        for (int i = 0; i < CollectionFiles.Length; i++)
        {
            GameObject go = Load(CollectionFiles[i].filePath);
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().CollectionCacheSystem.GenerateCache(CollectionFiles[i].filePath, go, CollectionFiles[i].cacheCount);
        }
    }

    public bool Remove(Collection collection)
    {
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().CollectionCacheSystem.Restore(collection.FilePath, collection.gameObject);
        return true;
    }
}

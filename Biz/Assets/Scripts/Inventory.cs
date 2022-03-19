using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CollectionFilePath
{
    public string imageFilePath;
    public string prefabFilePath;
    public CollectionFilePath(string _imageFilePath, string _prefabFilePath)
    {
        imageFilePath = _imageFilePath;
        prefabFilePath = _prefabFilePath;
    }
}
    public class Inventory : MonoBehaviour
{ 
    public class CollectionConstants
    {
        public static CollectionFilePath Artist = new CollectionFilePath("UI/SuccessGame/Collection/Artist", "Prefabs/Etc/Collection/ArtistCollection");
        public static CollectionFilePath Carpenter = new CollectionFilePath("UI/SuccessGame/Collection/Carpenter", "Prefabs/Etc/Collection/CarpenterCollection");
        public static CollectionFilePath Cook = new CollectionFilePath("UI/SuccessGame/Collection/Cook", "Prefabs/Etc/Collection/CookCollection");
        public static CollectionFilePath Doctor = new CollectionFilePath("UI/SuccessGame/Collection/Doctor", "Prefabs/Etc/Collection/DoctorCollection");
        public static CollectionFilePath Farmer = new CollectionFilePath("UI/SuccessGame/Collection/Farmer", "Prefabs/Etc/Collection/FarmerCollection");
        public static CollectionFilePath Hairdresser = new CollectionFilePath("UI/SuccessGame/Collection/Hairdresser", "Prefabs/Etc/Collection/HairdresserCollection");
        public static CollectionFilePath Marine = new CollectionFilePath("UI/SuccessGame/Collection/Marine", "Prefabs/Etc/Collection/MarineCollection");
        public static CollectionFilePath Police = new CollectionFilePath("UI/SuccessGame/Collection/Police", "Prefabs/Etc/Collection/PoliceCollection");
        public static CollectionFilePath Singer = new CollectionFilePath("UI/SuccessGame/Collection/Singer", "Prefabs/Etc/Collection/SingerCollection");
        public static CollectionFilePath Firefighter = new CollectionFilePath("UI/SuccessGame/Collection/Firefighter", "Prefabs/Etc/Collection/SingerCollection");
    }

    private Dictionary<CollectionFilePath, int> inventoryCache = new Dictionary<CollectionFilePath, int>();
    private CollectionFilePath mRecentlyCollection = null;
    public CollectionFilePath RecentlyCollection
    {
        get
        {
            return mRecentlyCollection;
        }
    }


    private Dictionary<string, Sprite> imageCache = new Dictionary<string, Sprite>();


    private static Inventory instance = null;
    public static Inventory Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = GameObject.Find("Inventory");
                if (go == null)
                {
                    go = new GameObject("Inventory");

                    Inventory inventory = go.AddComponent<Inventory>();
                    return inventory;
                }
                else
                {
                    instance = go.GetComponent<Inventory>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Can't have two instance of singletone");
            DestroyImmediate(this);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // 인벤토리에 저장
    public void GenerateCache(CollectionFilePath filePath, int count)
    {
        if (inventoryCache.ContainsKey(filePath))
        {
            inventoryCache[filePath] += count;
            //Debug.Log("Already cache generated! filePath = " + filePath + " + " + inventoryCache[filePath]);
        }
        else
        {
            inventoryCache.Add(filePath, count);
        }
        mRecentlyCollection = filePath;
        return;
    }
    // 인벤토리의 sprite 접근
    public Sprite GetSprite(string imageFilePath)
    {

        if (!imageCache.ContainsKey(imageFilePath))
        {
            Debug.LogError("You entered a wrong path.");
            return null;
        }

        return imageCache[imageFilePath];
    }
    public List<Sprite> GetAllSprite()
    {
        List<Sprite> sprite = new List<Sprite>();

        foreach(var sp in imageCache)
        {
            sprite.Add(sp.Value);
        }

        return sprite;
    }
    public void Load(string resourcePath)
    {
        Sprite Sp = null;

        if (imageCache.ContainsKey(resourcePath))   // 캐시 확인
        {
            //Debug.Log("Already Sprite Key");
        }
        else
        {
            // 캐시에 없으므로 로드
            Sp = Resources.Load<Sprite>(resourcePath);
            if (!Sp)
            {
                Debug.LogError("Load error! path = " + resourcePath);
                return;
            }
            // 로드 후 캐시에 적재
            imageCache.Add(resourcePath, Sp);
        }

        return;
    }
    public void Prepare()
    {
        foreach(var inventory in inventoryCache)
        {
            Load(inventory.Key.imageFilePath);
        }
    }
}

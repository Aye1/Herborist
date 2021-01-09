using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ResourcesManager : MonoBehaviour
{
    public static ResourcesManager Instance { get; private set; }

    private readonly string COLLECTIBLES_PATH = "Collectibles";

    private Dictionary<string, List<Object>> _resources;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _resources = new Dictionary<string, List<Object>>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public CollectibleScriptableObject GetCollectibleScriptableObjectWithName(string developmentName)
    {
        IEnumerable<CollectibleScriptableObject> allCollectibles = GetResourcesAtPath(COLLECTIBLES_PATH).Select(c => c as CollectibleScriptableObject);
        CollectibleScriptableObject res = allCollectibles.Where(c => c.developmentName == developmentName).FirstOrDefault();
        if(res == default(CollectibleScriptableObject))
        {
            Debug.LogError("Collectible with developmentName " + developmentName + " not found");
        }
        return res;
    }

    private List<Object> GetResourcesAtPath(string path)
    {
        if(_resources.Keys.Contains(path))
        {
            return _resources[path];
        }
        Object[] res = Resources.LoadAll(path);
        List<Object> objects = new List<Object>();
        objects.AddRange(res);
        _resources.Add(path, objects);
        return objects;
    }
}

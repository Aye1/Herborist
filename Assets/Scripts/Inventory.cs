using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.Linq;
using MoreMountains.Feedbacks;
using UnityEngine;
using System;

public class InventoryState : SaveState
{
    public List<CollectiblePackage> collectibles;
}

public class Inventory : SerializedMonoBehaviour, ISavable
{
    //[ReadOnly]
    //public List<CollectiblePackage> inventoryList; // OLD - TBR
    [ReadOnly]
    public Dictionary<CollectibleScriptableObject, uint> collectibleDic; // NEW

    public uint maxPlantCount = 10;
    public uint inventorySize = 10;
    public MMFeedbacks feedback;

    private void Awake()
    {
        //inventoryList = new List<CollectiblePackage>();
        collectibleDic = new Dictionary<CollectibleScriptableObject, uint>();
    }

    // NEW
    public uint Add(CollectibleScriptableObject collectible, uint count)
    {
        uint added = 0;
        if(collectibleDic.ContainsKey(collectible))
        {
            // Update current inventory entry
            uint currentCount = collectibleDic[collectible];
            uint availableRoom = maxPlantCount - currentCount;
            added = (uint) Mathf.Min(count, availableRoom);
            collectibleDic[collectible] += added;
        } else if (collectibleDic.Count < inventorySize) // Check for maximum bag size
        {
            // Create new inventory entry
            added = (uint)Mathf.Min(count, maxPlantCount);
            collectibleDic.Add(collectible, added);
        }
        return added;
    }

    // NEW + OLD
    public uint Add(CollectiblePackage pck)
    {
        return Add(pck.type, (uint)pck.count);
    }

    // NEW
    public void Add(Dictionary<CollectibleScriptableObject, uint> collectibles)
    {
        foreach(KeyValuePair<CollectibleScriptableObject, uint> pair in collectibles)
        {
            Add(pair.Key, pair.Value);
        }
    }

    // NEW + OLD
    public void Add(IEnumerable<CollectiblePackage> packages)
    {
        foreach(CollectiblePackage pck in packages)
        {
            Add(pck);
        }
    }

    // NEW
    public bool CanAddCollectible(CollectibleScriptableObject collectible)
    {
        bool canAdd = false;

        if(collectibleDic.ContainsKey(collectible))
        {
            canAdd = collectibleDic[collectible] < maxPlantCount;
        }
        else
        {
            canAdd = collectibleDic.Count < inventorySize;
        }

        return canAdd;
    }

    // NEW
    public void Remove(CollectibleScriptableObject collectible, uint count)
    {
        if(collectibleDic.ContainsKey(collectible))
        {
            collectibleDic[collectible] -= count;
            if(collectibleDic[collectible] <= 0)
            {
                collectibleDic.Remove(collectible);
            }
        }
    }

    // NEW
    public Dictionary<CollectibleScriptableObject, uint> Empty()
    {
        Dictionary<CollectibleScriptableObject, uint> res = new Dictionary<CollectibleScriptableObject, uint>(collectibleDic);
        collectibleDic.Clear();
        return res;
    }

    // NEW + OLD
    public IEnumerable<CollectiblePackage> EmptyAsPackages()
    {
        List<CollectiblePackage> packages = new List<CollectiblePackage>();
        foreach(KeyValuePair<CollectibleScriptableObject, uint> pair in collectibleDic)
        {
            CollectiblePackage pck = new CollectiblePackage()
            {
                type = pair.Key,
                count = (int)pair.Value
            };
            packages.Add(pck);
        }
        collectibleDic.Clear();
        return packages;
    }

    // NEW
    public uint GetCollectibleCount(CollectibleScriptableObject collectible)
    {
        return collectibleDic.ContainsKey(collectible) ? collectibleDic[collectible] : 0;
    }

    // NEW
    public IEnumerable<CollectibleScriptableObject> GetCollectiblesWhere(Func<CollectibleScriptableObject, bool> predicate)
    {
        return collectibleDic.Keys.Where(predicate ?? (c => true));
    }

    // NEW
    public IEnumerable<CollectibleScriptableObject> GetUnidentifiedCollectibles()
    {
        return GetCollectiblesWhere(c => !PlantIdentificationInfos.Instance.IsIdentified(c));
    }

    // OLD + NEW
    public IEnumerable<CollectiblePackage> GetCollectiblePackages()
    {
        return collectibleDic.Select(p => new CollectiblePackage()
        {
            type = p.Key,
            count = (int)p.Value
        });
    }

    // OLD + NEW
    private void LoadFromCollectiblePackages(IEnumerable<CollectiblePackage> packages)
    {
        if (collectibleDic == null)
        {
            collectibleDic = new Dictionary<CollectibleScriptableObject, uint>();
        }
        else
        {
            collectibleDic.Clear();
        }
        Add(packages);
    }

    // NEW
    public bool Contains(PlantComponentScriptableObject component)
    {
        return collectibleDic.ContainsKey(component.collectibleInfo);
    }

    #region ISavable implementation
    public SaveState GetObjectToSave()
    {
        InventoryState state = new InventoryState()
        {
            collectibles = GetCollectiblePackages().ToList()
        };
        Debug.Log("Saving " + state.collectibles.Count);
        return state;
    }

    public void LoadObject(SaveState saveState)
    {
        InventoryState state = saveState as InventoryState;
        LoadFromCollectiblePackages(state.collectibles);
        Debug.Log("Loading " + state.collectibles.Count);
    }

    public string GetSaveName()
    {
        return gameObject.name + ".inventory";
    }
    #endregion

    // TODO: probably move outside inventory
    public void PlayInventoryFull()
    {
        feedback.PlayFeedbacks();
    }
}

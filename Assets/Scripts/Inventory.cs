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
    [ReadOnly]
    public List<CollectiblePackage> inventoryList; // OLD - TBR
    [ReadOnly]
    public Dictionary<CollectibleScriptableObject, uint> collectibleDic; // NEW

    public uint maxPlantCount = 10;
    public uint inventorySize = 10;
    public MMFeedbacks feedback;

    private void Awake()
    {
        inventoryList = new List<CollectiblePackage>();
        collectibleDic = new Dictionary<CollectibleScriptableObject, uint>();
    }

    // OLD - TBR
    /*public void Add(CollectibleScriptableObject itemType)
    {
        CollectiblePackage pck = inventoryList.Where(p => p.type == itemType).FirstOrDefault();
        if (pck != default(CollectiblePackage))
        {
            pck.count++;
        }
        else
        {
            inventoryList.Add(new CollectiblePackage()
            {
                type = itemType,
                count = 1
            });
        }
    }*/

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

    // OLD - TBR
    /*public void Add(IEnumerable<CollectibleScriptableObject> itemsTypes)
    {
        foreach (CollectibleScriptableObject item in itemsTypes)
        {
            Add(item);
        }
    }*/

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

    // OLD - TBR
    /*public bool CanAddCollectible(CollectiblePackage collectible)
    {
        bool returnValue = false;
        CollectiblePackage pck = inventoryList.Where(p => p.type == collectible.type).FirstOrDefault();
        if (pck != default(CollectiblePackage))
        {
            returnValue = (pck.count + collectible.count <= maxPlantCount);
        }
        else
        {
            returnValue = inventoryList.Count < inventorySize;
        }
        return returnValue;
    }*/

    // OLD - TBR
    /*public void Add(CollectiblePackage collectible)
    {
        CollectiblePackage pck = inventoryList.Where(p => p.type == collectible.type).FirstOrDefault();
        if (pck != default(CollectiblePackage))
        {
            pck.count += collectible.count;
        }
        else
        {
            inventoryList.Add(collectible);
        }
    }*/

    // OLD - TBR
    /*public void Add(IEnumerable<CollectiblePackage> collectibles)
    {
        foreach (CollectiblePackage p in collectibles)
        {
            Add(p);
        }
    }*/

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
        return packages;
    }

    // OLD - TBR
    /*public void Remove(CollectibleScriptableObject itemType, int number)
    {
        CollectiblePackage pck = inventoryList.Where(p => p.type == itemType).FirstOrDefault();
        if (pck != default(CollectiblePackage))
        {
            pck.count -= number;
            if (pck.count <= 0)
            {
                inventoryList.Remove(pck);
            }
        }
    }*/

    // OLD - TBR
    /*public void RemoveAll(CollectibleScriptableObject itemType)
    {
        inventoryList.RemoveAll(p => p.type == itemType);
    }*/

    // OLD - TBR
    /*public List<CollectiblePackage> EmptyInventory()
    {
        List<CollectiblePackage> res = new List<CollectiblePackage>(inventoryList);
        inventoryList.Clear();
        return res;
    }*/

    // NEW
    public uint GetCollectibleCount(CollectibleScriptableObject collectible)
    {
        return collectibleDic.ContainsKey(collectible) ? collectibleDic[collectible] : 0;
    }

    // OLD - TBR
    /*public int GetItemCount(CollectibleScriptableObject itemType)
    {
        return inventoryList.Where(p => p.type == itemType).FirstOrDefault().count;
    }*/

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

    // OLD - TBR
    /*public List<CollectibleScriptableObject> GetUnidentifiedComponents()
    {
        return inventoryList.Where(p => !PlantIdentificationInfos.Instance.IsIdentified(p.type)).Select(p => p.type).ToList();
    }*/

    // NEW
    public bool Contains(PlantComponentScriptableObject component)
    {
        return collectibleDic.ContainsKey(component.collectibleInfo);
    }

    // OLD - TBR
    /*public bool IsInInventory(PlantComponentScriptableObject component)
    {
        return inventoryList.Exists(p => p.type == component.collectibleInfo);
    }*/

    #region ISavable implementation
    public SaveState GetObjectToSave()
    {
        InventoryState state = new InventoryState()
        {
            collectibles = new List<CollectiblePackage>()
        };
        if (inventoryList != null && inventoryList.Count > 0)
        {
            state.collectibles.AddRange(inventoryList);
        }
        return state;
    }

    public void LoadObject(SaveState saveState)
    {
        InventoryState state = saveState as InventoryState;
        if (inventoryList == null)
        {
            inventoryList = new List<CollectiblePackage>();
        }
        else
        {
            inventoryList.Clear();
        }
        inventoryList.AddRange(state.collectibles);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using System.Linq;

public class InventoryState: SaveState
{
    public List<CollectiblePackage> collectibles;
}

public class Inventory : SerializedMonoBehaviour, ISavable
{
    [ReadOnly]
    public List<CollectiblePackage> inventoryList;

    private void Awake()
    {
        inventoryList = new List<CollectiblePackage>();
    }

    public void Add(CollectibleScriptableObject itemType)
    {
        CollectiblePackage pck = inventoryList.Where(p => p.type == itemType).FirstOrDefault();
        if(pck != default(CollectiblePackage))
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
    }

    public void Add(IEnumerable<CollectibleScriptableObject> itemsTypes)
    {
        foreach(CollectibleScriptableObject item in itemsTypes)
        {
            Add(item);
        }
    }

    public void Add(CollectiblePackage collectible)
    {
        CollectiblePackage pck = inventoryList.Where(p => p.type == collectible.type).FirstOrDefault();
        if(pck != default(CollectiblePackage))
        {
            pck.count += collectible.count;
        }
        else
        {
            inventoryList.Add(collectible);
        }
    }

    public void Add(IEnumerable<CollectiblePackage> collectibles)
    {
        foreach(CollectiblePackage p in collectibles)
        {
            Add(p);
        }
    }

    public void Remove(CollectibleScriptableObject itemType, int number)
    {
        CollectiblePackage pck = inventoryList.Where(p => p.type == itemType).FirstOrDefault();
        if(pck != default(CollectiblePackage))
        {
            pck.count -= number;
            if(pck.count <= 0)
            {
                inventoryList.Remove(pck);
            }
        }
    }

    public void RemoveAll(CollectibleScriptableObject itemType)
    {
        inventoryList.RemoveAll(p => p.type == itemType);
    }

    public int GetItemCount(CollectibleScriptableObject itemType)
    {
        return inventoryList.Where(p => p.type == itemType).FirstOrDefault().count;
    }

    #region ISavable implementation
    public SaveState GetObjectToSave()
    {
        Debug.Log("saving inventory " + inventoryList[0].count);
        InventoryState state = new InventoryState()
        {
            collectibles = new List<CollectiblePackage>()
        };
        state.collectibles.AddRange(inventoryList);
        return state;
    }

    public void LoadObject(SaveState saveState)
    {
        InventoryState state = saveState as InventoryState;
        inventoryList.Clear();
        inventoryList.AddRange(state.collectibles);
        Debug.Log("loading inventory " + inventoryList[0].count);
    }

    public string GetSaveName()
    {
        return "inventory.save";
    }
    #endregion
}

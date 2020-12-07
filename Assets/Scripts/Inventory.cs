using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class Inventory : SerializedMonoBehaviour
{
    [ReadOnly]
    [DictionaryDrawerSettings(KeyLabel = "Collectible", ValueLabel = "Count")]
    public Dictionary<CollectibleScriptableObject, int> inventory;

    private void Awake()
    {
        inventory = new Dictionary<CollectibleScriptableObject, int>();
    }

    public void Add(CollectibleScriptableObject itemType)
    {
        if(inventory.ContainsKey(itemType))
        {
            inventory[itemType]++;
        } else
        {
            inventory.Add(itemType, 1);
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
        if(inventory.ContainsKey(collectible.type))
        {
            inventory[collectible.type] += collectible.count;
        } else
        {
            inventory.Add(collectible.type, collectible.count);
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
        if(inventory.ContainsKey(itemType))
        {
            inventory[itemType] -= number;
            if(inventory[itemType] <= 0)
            {
                inventory.Remove(itemType);
            }
        }
    }

    public void RemoveAll(CollectibleScriptableObject itemType)
    {
        if(inventory.ContainsKey(itemType))
        {
            inventory.Remove(itemType);
        }
    }

    public int GetItemCount(CollectibleScriptableObject itemType)
    {
        if(inventory.ContainsKey(itemType))
        {
            return inventory[itemType];
        }
        return 0;
    }
}

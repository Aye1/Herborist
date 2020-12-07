using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class Inventory : SerializedMonoBehaviour
{
    [ReadOnly]
    public Dictionary<Type, int> _inventory;

    private void Awake()
    {
        _inventory = new Dictionary<Type, int>();
    }

    public void Add(Type itemType)
    {
        if(_inventory.ContainsKey(itemType))
        {
            _inventory[itemType]++;
        } else
        {
            _inventory.Add(itemType, 1);
        }
    }

    public void Add(IEnumerable<Type> itemsTypes)
    {
        foreach(Type item in itemsTypes)
        {
            Add(item);
        }
    }

    public void Add(CollectiblePackage collectible)
    {
        if(_inventory.ContainsKey(collectible.type))
        {
            _inventory[collectible.type] += collectible.count;
        } else
        {
            _inventory.Add(collectible.type, collectible.count);
        }
    }

    public void Add(IEnumerable<CollectiblePackage> collectibles)
    {
        foreach(CollectiblePackage p in collectibles)
        {
            Add(p);
        }
    }

    public void Remove(Type itemType, int number)
    {
        if(_inventory.ContainsKey(itemType))
        {
            _inventory[itemType] -= number;
            if(_inventory[itemType] <= 0)
            {
                _inventory.Remove(itemType);
            }
        }
    }

    public void RemoveAll(Type itemType)
    {
        if(_inventory.ContainsKey(itemType))
        {
            _inventory.Remove(itemType);
        }
    }

    public int GetItemCount(Type itemType)
    {
        if(_inventory.ContainsKey(itemType))
        {
            return _inventory[itemType];
        }
        return 0;
    }
}

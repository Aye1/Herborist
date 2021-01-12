using System.Collections.Generic;
using Sirenix.OdinInspector;
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

    public List<CollectiblePackage> EmptyInventory()
    {
        List<CollectiblePackage> res = new List<CollectiblePackage>(inventoryList);
        inventoryList.Clear();
        return res;
    }

    public int GetItemCount(CollectibleScriptableObject itemType)
    {
        return inventoryList.Where(p => p.type == itemType).FirstOrDefault().count;
    }

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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class InventoryUI : BasePopup
{
    [SerializeField, Required, AssetsOnly] private InventoryItemUI _itemTemplate;
    [SerializeField, Required, ChildGameObjectsOnly] private Transform _itemContainer;

    void PopulateGrid()
    {
        Inventory inventory = HouseStorage.Instance.StorageInventory;
        foreach(CollectiblePackage package in inventory.inventoryList)
        {
            InventoryItemUI newItem = Instantiate(_itemTemplate, _itemContainer);
            newItem.Collectible = package;
        }
    }

    void CleanGrid()
    {
        foreach(Transform child in _itemContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }

    #region BasePopup implementation
    protected override GameObject GetObjectToDeactivate()
    {
        return gameObject;
    }

    protected override void OnPopupClosing()
    {
        return;
    }

    protected override void CustomOnEnable()
    {
        PopulateGrid();
    }

    protected override void CustomOnDisable()
    {
        CleanGrid();
    }
    #endregion
}

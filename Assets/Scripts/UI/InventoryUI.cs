﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class InventoryUI : BasePopup
{
    [SerializeField, Required, AssetsOnly] private InventoryItemUI _itemTemplate;
    [SerializeField, Required, ChildGameObjectsOnly] private Transform _itemContainer;
    [SerializeField, Required, ChildGameObjectsOnly] private InventoryItemDetailsUI _detailView;

    void PopulateGrid()
    {
        Inventory inventory = HouseStorage.Instance.StorageInventory;
        foreach(CollectiblePackage package in inventory.inventoryList)
        {
            InventoryItemUI newItem = Instantiate(_itemTemplate, _itemContainer);
            newItem.Collectible = package;
        }
        if(inventory.inventoryList.Count > 0)
        {
            SetEventSystemFocus();
        }
    }

    void CleanGrid()
    {
        foreach(Transform child in _itemContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void BindEvents()
    {
        InventoryItemUI.OnCollectibleSelected += OnCollectibleSelected;
    }

    void UnBindEvents()
    {
        InventoryItemUI.OnCollectibleSelected -= OnCollectibleSelected;
    }

    void SetEventSystemFocus()
    {
        NavigationManager.Instance.SetFocus(_itemContainer.GetChild(0).gameObject);
    }

    void OnCollectibleSelected(CollectiblePackage collectible)
    {
        _detailView.Collectible = collectible;
    }

    #region BasePopup implementation
    protected override void OnPopupClosing()
    {
        return;
    }

    protected override void CustomOnEnable()
    {
        BindEvents();
        PopulateGrid();
    }

    protected override void CustomOnDisable()
    {
        CleanGrid();
        UnBindEvents();
    }
    #endregion
}

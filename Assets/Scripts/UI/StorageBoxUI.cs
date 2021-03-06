﻿using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class StorageBoxUI : BasePopup
{
    [SerializeField, Required, ChildGameObjectsOnly] private GameObject _verticalLayout;
    [SerializeField, Required, ChildGameObjectsOnly] private Button _closeButton;
    [SerializeField, Required, AssetsOnly] private StorageBoxElementUI _collectibleCellTemplate;

    private List<CollectiblePackage> _collectibles;

    public List<CollectiblePackage> Collectibles
    {
        get { return _collectibles; }
        set
        {
            if(_collectibles != value)
            {
                _collectibles = value;
                CreateCollectiblesList();
            }
        }
    }

    private void Awake()
    {
        _closeButton.onClick.AddListener(ClosePopup);
    }

    private void CreateCollectiblesList()
    {
        foreach(CollectiblePackage collectible in Collectibles)
        {
            StorageBoxElementUI newCell = Instantiate(_collectibleCellTemplate, _verticalLayout.transform);
            newCell.Collectible = collectible;
        }
    }

    private void CleanCollectiblesList()
    {
        foreach(Transform child in _verticalLayout.transform)
        {
            Destroy(child.gameObject);
        }
    }

    #region BasePopup implementation

    protected override void OnPopupClosing()
    {
        return;
    }

    protected override void CustomOnEnable()
    {
        return;
    }

    protected override void CustomOnDisable()
    {
        CleanCollectiblesList();
    }
    #endregion
}

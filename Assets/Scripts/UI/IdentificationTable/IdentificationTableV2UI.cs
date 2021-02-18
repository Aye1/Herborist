using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;

public class IdentificationTableV2UI : BasePopup
{
    [Title("Editor bindings")]
    [SerializeField, Required, AssetsOnly] private IdentificationTableV2CollectibleButton _collectibleButtonTemplate;
    [SerializeField, Required, ChildGameObjectsOnly] private Transform _buttonsHolder;

    private List<CollectibleScriptableObject> _toIdentifyCollectibles;

    void Awake()
    {
        _toIdentifyCollectibles = new List<CollectibleScriptableObject>();
    }

    void UpdateUI()
    {
        foreach(CollectibleScriptableObject collectible in _toIdentifyCollectibles)
        {
            IdentificationTableV2CollectibleButton newButton = Instantiate(_collectibleButtonTemplate, _buttonsHolder);
            newButton.Collectible = collectible;
        }
        SetFocus();
    }

    private void SetFocus()
    {
        if (_buttonsHolder.childCount > 0)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(_buttonsHolder.GetChild(0).gameObject);
        }
    }

    #region BasePopup implementation
    protected override void CustomOnDisable()
    {
        return;
    }

    protected override void CustomOnEnable()
    {
        _toIdentifyCollectibles.Clear();
        _toIdentifyCollectibles.AddRange(HouseStorage.Instance.GetUnidentifiedComponents());
        UpdateUI();
    }

    protected override GameObject GetObjectToDeactivate()
    {
        return gameObject;
    }

    protected override void OnPopupClosing()
    {
        return;
    }
    #endregion
}

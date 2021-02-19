using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;

public class IdentificationTableV2UI : BasePopup
{
    [Title("Editor bindings - Assets")]
    [SerializeField, Required, AssetsOnly] private IdentificationTableV2CollectibleButton _collectibleButtonTemplate;

    [Title("Editor bindings - Children objects")]
    [SerializeField, Required, ChildGameObjectsOnly] private Transform _selector;
    [SerializeField, Required, ChildGameObjectsOnly] private Transform _buttonsHolder;
    [SerializeField, Required, ChildGameObjectsOnly] private IdentificationQuestionsListUI _questionsList;

    private List<CollectibleScriptableObject> _toIdentifyCollectibles;

    void Awake()
    {
        _toIdentifyCollectibles = new List<CollectibleScriptableObject>();
        ToggleSelectorVisiblity(true);
        _questionsList.OnComponentIdentified += OnComponentIdentified;
    }

    private void OnDestroy()
    {
        _questionsList.OnComponentIdentified -= OnComponentIdentified; // Should be useless if everything goes right 
    }

    void UpdateUI()
    {
        foreach(CollectibleScriptableObject collectible in _toIdentifyCollectibles)
        {
            IdentificationTableV2CollectibleButton newButton = Instantiate(_collectibleButtonTemplate, _buttonsHolder);
            newButton.Collectible = collectible;
            newButton.SelfButton.onClick.AddListener(() => OnComponentSelected(collectible.parentPlantComponent));
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

    private void OnComponentSelected(PlantComponentScriptableObject component)
    {
        ToggleSelectorVisiblity(false);
        _questionsList.CurrentComponent = component;
    }

    private void OnComponentIdentified(PlantComponentScriptableObject component)
    {
        PlantIdentificationInfos.Instance.SetComponentIdentification(component, true);
        ToggleSelectorVisiblity(true);
        UpdateUnidentifiedComponentsList();
    }

    private void ToggleSelectorVisiblity(bool active)
    {
        _selector.gameObject.SetActive(active);
        _questionsList.gameObject.SetActive(!active);
    }

    private void UpdateUnidentifiedComponentsList()
    {
        ClearComponentsList();
        _toIdentifyCollectibles.AddRange(HouseStorage.Instance.GetUnidentifiedComponents());
        UpdateUI();
    }

    private void ClearComponentsList()
    {
        if(_buttonsHolder.childCount > 0)
        {
            _toIdentifyCollectibles.Clear();
            foreach(Transform child in _buttonsHolder)
            {
                Destroy(child.gameObject);
            }
        }
    }

    #region BasePopup implementation
    protected override void CustomOnDisable()
    {
        return;
    }

    protected override void CustomOnEnable()
    {
        UpdateUnidentifiedComponentsList();
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

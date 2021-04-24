using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;

public class BookPlateUI : BookPageUI
{
    [SerializeField, Required, ChildGameObjectsOnly] private TextMeshProUGUI _plantNameText;
    [SerializeField, Required, ChildGameObjectsOnly] private Image _plantFullImage;
    [SerializeField, Required, ChildGameObjectsOnly] private Transform _componentHolder;
    [SerializeField, Required, AssetsOnly] private IdentificationTableV2CollectibleButton _componentButtonTemplate;

    public BookUI bookParent;

    public override void UpdateUI()
    {
        if (Plant == null)
        {
            UpdateEmptyUI();
        }
        else
        {
            _plantNameText.text = Plant.name;
            _plantFullImage.sprite = Plant.fullPicture;
            PopulateComponents();
        }
    }

    void UpdateEmptyUI()
    {
        _plantNameText.text = "Unknown";
        _plantFullImage.sprite = null;
        ClearComponents();
    }

    void ClearComponents()
    {
        foreach (Transform child in _componentHolder)
        {
            Destroy(child.gameObject);
        }
    }

    void PopulateComponents()
    {
        ClearComponents();
        bool isFirst = true;
        foreach (PlantComponentScriptableObject component in Plant.components)
        {
            IdentificationTableV2CollectibleButton newComponent = Instantiate(_componentButtonTemplate, _componentHolder);
            newComponent.PlantComponent = component;
            newComponent.SelfButton.onClick.AddListener(() => OnComponentSelected(newComponent.PlantComponent));

            if (isFirst && newComponent.SelfButton.interactable)
            {
                NavigationManager.Instance.SetFocus(newComponent.gameObject);
                isFirst = false;
            }

        }
    }

    private void OnComponentSelected(PlantComponentScriptableObject component)
    {
        bookParent.OpenIdentificationTable(component);
    }

    override public void ActivateFocusableElements(bool isActivate)
    {
        base.ActivateFocusableElements(isActivate);
        bool isFirst = isActivate;

        foreach (Transform child in _componentHolder)
        {
            IdentificationTableV2CollectibleButton collectibleButton = child.GetComponent<IdentificationTableV2CollectibleButton>();
            collectibleButton.SelfButton.interactable = isActivate && collectibleButton.IsFocusable();
            if (isFirst && collectibleButton.SelfButton.interactable)
            {
                NavigationManager.Instance.SetFocus(child.gameObject);
                isFirst = false;
            }
        }
    }
}

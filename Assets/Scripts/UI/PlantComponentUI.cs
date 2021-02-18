using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using TMPro;
using Unisloth.Localization;

public class PlantComponentUI : MonoBehaviour
{
    [Title("Editor bindings")]
    [SerializeField, Required, ChildGameObjectsOnly] private TextMeshProUGUI _componentNameText;
    [SerializeField, Required, ChildGameObjectsOnly] private Image _componentImage;

    private PlantComponentScriptableObject _component;
    public PlantComponentScriptableObject Component
    {
        get { return _component; }
        set
        {
            if(_component != value)
            {
                _component = value;
                UpdateUI();
            }
        }
    }

    private void UpdateUI()
    {
        if(Component != null)
        {
            _componentNameText.text = LocalizationManager.Instance.GetTranslation(Component.plantParent.commonNameLocKey);
            _componentImage.sprite = Component.componentPicture;
        }
    }
}

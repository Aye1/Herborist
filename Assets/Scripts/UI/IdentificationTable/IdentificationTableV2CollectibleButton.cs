using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.UI;

public class IdentificationTableV2CollectibleButton : MonoBehaviour
{
    [Title("Editor bindings")]
    [SerializeField, Required, ChildGameObjectsOnly] private TextMeshProUGUI _componentNameText;
    [SerializeField, Required, ChildGameObjectsOnly] private Image _componentImage;
    [SerializeField, Required, ChildGameObjectsOnly] private Button _selfButton;

    private PlantComponentScriptableObject _plantComponent;

    public PlantComponentScriptableObject PlantComponent
    {
        get
        {
            return _plantComponent;
        }
        set
        {
            _plantComponent = value;
            UpdateUI();
        }
    }

    public Button SelfButton
    {
        get { return _selfButton; }
    }

    private void UpdateUI()
    {
        if (PlantComponent != null)
        {
            _componentNameText.text = PlantIdentificationInfos.Instance.GetPlantCurrentName(PlantComponent);
            if (PlantComponent.collectibleInfo != null && PlantComponent.collectibleInfo.type != null)
            {
                _componentImage.sprite = PlantComponent.collectibleInfo.type.sprite;
            }
        }
    }
}

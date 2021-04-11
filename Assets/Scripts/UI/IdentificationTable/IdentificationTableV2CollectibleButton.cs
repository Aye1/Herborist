using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.UI;

public class IdentificationTableV2CollectibleButton : MonoBehaviour
{
    [Title("Editor bindings")]
    [SerializeField, Required, ChildGameObjectsOnly] private Image _componentImage;
    [SerializeField, Required, ChildGameObjectsOnly] private Button _selfButton;
    [SerializeField, Required, AssetsOnly] private Sprite _unknownComponent;

    private PlantComponentScriptableObject _plantComponent;
    enum ComponentState { UNKNOWN, UNIDENTIFIED, IDENTIFIED };
    private ComponentState _currentState;
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
            UpdateCurrentState();
            if (_currentState != ComponentState.UNKNOWN)
            {
                if (PlantComponent.collectibleInfo != null)
                {
                    _componentImage.sprite = PlantComponent.collectibleInfo.sprite;
                }
                if (_currentState == ComponentState.IDENTIFIED)
                {
                    SelfButton.interactable = false;
                }
            }
            else
            {
                _componentImage.sprite = _unknownComponent;
                SelfButton.interactable = false;
            }
        }
    }
    private void UpdateCurrentState()
    {
        if (PlantIdentificationInfos.Instance.IsIdentified(PlantComponent))
        {
            _currentState = ComponentState.IDENTIFIED;
        }
        else if (HouseStorage.Instance.IsInInventory(PlantComponent))
        {
            _currentState = ComponentState.UNIDENTIFIED;
        }
        else
        {
            _currentState = ComponentState.UNKNOWN;
        }
    }
}

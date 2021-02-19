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

    private CollectibleScriptableObject _collectible;

    public CollectibleScriptableObject Collectible
    {
        get
        {
            return _collectible;
        }
        set
        {
            _collectible = value;
            UpdateUI();
        }
    }

    public Button SelfButton
    {
        get { return _selfButton; }
    }

    private void UpdateUI()
    {
        if(Collectible != null)
        {
            _componentNameText.text = PlantIdentificationInfos.Instance.GetPlantCurrentName(Collectible);
            _componentImage.sprite = Collectible.sprite;
        }
    }
}

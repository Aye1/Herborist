using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using TMPro;

public class InventoryItemDetailsUI : MonoBehaviour
{
    [SerializeField, Required, ChildGameObjectsOnly] private Image _itemImage;
    [SerializeField, Required, ChildGameObjectsOnly] private TextMeshProUGUI _itemNameText;
    [SerializeField, Required, ChildGameObjectsOnly] private TextMeshProUGUI _itemCountText;

    private CollectiblePackage _collectible;
    public CollectiblePackage Collectible
    {
        get { return _collectible; }
        set
        {
            // We force to update UI even if the value hasn't changed
            // In some cases, we assign the same Collectible, but the count has changed, so we want to update the text
            _collectible = value;
            UpdateUI();
        }
    }

    private void OnEnable()
    {
        UpdateEmptyUI();
    }

    private void UpdateUI()
    {
        _itemImage.enabled = true;
        _itemImage.sprite = Collectible.type.sprite;
        _itemNameText.text = PlantIdentificationInfos.Instance.GetPlantCurrentName(Collectible.type);
        _itemCountText.text = Collectible.count.ToString();
    }

    private void UpdateEmptyUI()
    {
        _itemImage.enabled = false;
        _itemNameText.text = "";
        _itemCountText.text = "";
    }
}

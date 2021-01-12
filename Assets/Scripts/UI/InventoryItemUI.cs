using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using TMPro;

public class InventoryItemUI : MonoBehaviour
{
    [SerializeField, Required, ChildGameObjectsOnly] private Image _itemImage;
    [SerializeField, Required, ChildGameObjectsOnly] private TextMeshProUGUI _countText;

    private CollectiblePackage _collectible;
    public CollectiblePackage Collectible
    {
        get { return _collectible; }
        set
        {
            if(_collectible != value)
            {
                _collectible = value;
                UpdateUI();
            }
        }
    }

    void UpdateUI()
    {
        _itemImage.sprite = Collectible.type.sprite;
        _countText.text = Collectible.count.ToString();
    }
}

using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventoryItemUI : Selectable
{
    [SerializeField, Required, ChildGameObjectsOnly] private Image _itemImage;
    [SerializeField, Required, ChildGameObjectsOnly] private TextMeshProUGUI _countText;

    public delegate void CollectibleEvent(CollectiblePackage collectible);
    public static CollectibleEvent OnCollectibleSelected;

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

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        OnCollectibleSelected?.Invoke(Collectible);
    }
}

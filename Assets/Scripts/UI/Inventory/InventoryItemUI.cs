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
            _collectible = value;

            UpdateUI();
        }
    }

    void UpdateUI()
    {
        if (_collectible != null)
        {
            _itemImage.sprite = Collectible.type.sprite;
            _countText.text = Collectible.count.ToString();
        }
        else
        {
            _itemImage.enabled = false;
            _countText.text = "";
        }
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        OnCollectibleSelected?.Invoke(Collectible);
    }
}

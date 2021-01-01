using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using MoreMountains.Feedbacks;

public class CollectiblePicked : MonoBehaviour
{
    private CollectiblePackage _package;

    public CollectiblePackage Package
    {
        get { return _package; }
        set
        {
            if (_package != value)
            {
                _package = value;
                UpdateUI();
            }
        }
    }

    [SerializeField, Required] private TextMeshProUGUI _quantityText;
    [SerializeField, Required] private Image _collectibleImage;
    [SerializeField, Required] private MMFeedbacks _feedbacks;

    private void UpdateUI()
    {
        _quantityText.text = "+" + Package.count;
        _collectibleImage.sprite = Package.type.sprite;
    }
}

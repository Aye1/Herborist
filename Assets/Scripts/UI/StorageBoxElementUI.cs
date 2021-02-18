using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;

public class StorageBoxElementUI : MonoBehaviour
{
    [SerializeField, Required, ChildGameObjectsOnly] private Image _collectibleImage;
    [SerializeField, Required, ChildGameObjectsOnly] private TextMeshProUGUI _collectibleText;

    private CollectiblePackage _collectible;

    public CollectiblePackage Collectible
    {
        get { return _collectible; }
        set
        {
            if(_collectible != value)
            {
                _collectible = value;
                UpdateCollectibleUI();
            }
        }
    }

    private void UpdateCollectibleUI()
    {
        _collectibleImage.sprite = Collectible.type.sprite;
        string name = PlantIdentificationInfos.Instance.GetPlantCurrentName(Collectible.type);
        _collectibleText.text = name + " x" + Collectible.count;
    }
}

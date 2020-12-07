using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Collectible", menuName = "ScriptableObjects/Collectible")]
public class CollectibleScriptableObject : ScriptableObject
{
    [Required]
    [VerticalGroup("Split/NameInfos")]
    public string developmentName;

    [Required]
    [VerticalGroup("Split/NameInfos")]
    public string nameLocKey;

    [HorizontalGroup("Split", Width = 50, Title = "Information")]
    [HideLabel]
    [PreviewField(50)]
    public Sprite sprite;

    [Title("Gathering data", HorizontalLine = false)]
    [MinMaxSlider(1, 100, true)]
    [PropertySpace(SpaceBefore = 20)]
    public Vector2Int spawnQuantity = Vector2Int.one;

    [MinMaxSlider(0, "@spawnQuantity.y", true)]
    [PropertySpace(SpaceBefore = 20)]
    public Vector2Int handGatherQuantity = Vector2Int.one;

}

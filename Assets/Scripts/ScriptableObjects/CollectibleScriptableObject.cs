using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Collectible", menuName = "ScriptableObjects/Collectible")]
public class CollectibleScriptableObject : ScriptableObject
{
    [Required]
    public string nameLocKey;

    [PreviewField(Alignment = ObjectFieldAlignment.Left)]
    public Sprite sprite;

    [MinMaxSlider(1, 100, true)]
    [PropertySpace(SpaceBefore = 20)]
    public Vector2Int quantity = Vector2Int.one;

    [MinMaxSlider(0, "@quantity.y", true)]
    [PropertySpace(SpaceBefore = 20)]
    public Vector2Int handGatherQuantity = Vector2Int.one;

}

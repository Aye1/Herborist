
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public enum PlantPart
{
    Size,
    Leaf,
    FLower,
    Fruit,
    Root
}

[CreateAssetMenu(fileName = "New Identification Key Data", menuName = "ScriptableObjects/FloraTree/IdentificationKeyData")]
public class IdentificationKeyData : ScriptableObject
{
    [Title("Node data", HorizontalLine = false)]
    public string identificationTitle;
    public string identificationDescription;
    public PlantPart plantPart;
    [PreviewField, HideLabel]
    public Sprite plate;
}

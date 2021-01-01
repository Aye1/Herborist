using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Identification Key Leaf", menuName = "ScriptableObjects/FloraTree/IdentificationKeyLeaf")]
public class IdentificationKeyLeaf : ScriptableObject
{
    [Title("Node data", HorizontalLine = false)]
    public string identificationTitle;
    public string identificationDescription;
    [PreviewField, HideLabel]
    public Sprite plate;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public enum PlantComponentType
{
    Leaf,
    Flower,
    Fruit,
    Root,
    Bark
}

[CreateAssetMenu(fileName = "Plant Component", menuName = "ScriptableObjects/Plant Component")]
public class PlantComponentScriptableObject : SerializedScriptableObject
{
    public PlantComponentType componentType;
    public Sprite icon;
    public Sprite componentPicture;
    //public CollectiblePackage collectibleInfo;
    public CollectibleScriptableObject collectibleInfo;
    [DictionaryDrawerSettings(KeyLabel = "Parameter", ValueLabel = "Value")]
    public Dictionary<PlantIdentificationParameterScriptableObject, PlantIdentificationValueScriptableObject> parameters;
    public PlantScriptableObject plantParent;
}

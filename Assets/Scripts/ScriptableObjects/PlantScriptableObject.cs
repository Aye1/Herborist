using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Plant", menuName = "ScriptableObjects/Plant")]
public class PlantScriptableObject : ScriptableObject
{
    public string unidentifiedPlantLetter;
    public string commonNameLocKey;
    public string speciesName;
    public string familyName;
    public List<string> loreLocKey;
    public Sprite fullPicture;
    public List<PlantComponentScriptableObject> components;
}

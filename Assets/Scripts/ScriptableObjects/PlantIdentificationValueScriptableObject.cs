using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Identification Value", menuName = "ScriptableObjects/Plant Identification Value")]
public class PlantIdentificationValueScriptableObject : ScriptableObject
{
    public string valueNameLocKey;
    public Sprite icon;
    public string descriptionLocKey;

}

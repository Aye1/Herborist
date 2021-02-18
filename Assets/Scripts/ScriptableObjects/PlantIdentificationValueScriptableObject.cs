using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unisloth.Localization;

[CreateAssetMenu(fileName = "Identification Value", menuName = "ScriptableObjects/Plant Identification Value")]
public class PlantIdentificationValueScriptableObject : ScriptableObject
{
    public Sprite icon;
    [TranslationKey]
    public string valueNameLocKey;
    [TranslationKey]
    public string descriptionLocKey;

}

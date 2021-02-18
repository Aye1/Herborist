using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unisloth.Localization;

[CreateAssetMenu(fileName = "Identification Parameter", menuName = "ScriptableObjects/Plant Identification Parameter")]
public class PlantIdentificationParameterScriptableObject : ScriptableObject
{
    [TranslationKey]
    public string questionLocKey;
    [TranslationKey]
    public string parameterNameLocKey;
    public List<PlantIdentificationValueScriptableObject> possibleValues;
}

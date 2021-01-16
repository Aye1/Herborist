using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Identification Parameter", menuName = "ScriptableObjects/Plant Identification Parameter")]
public class PlantIdentificationParameterScriptableObject : ScriptableObject
{
    public string questionLocKey;
    public string parameterNameLocKey;
    public List<PlantIdentificationValueScriptableObject> possibleValues;
}

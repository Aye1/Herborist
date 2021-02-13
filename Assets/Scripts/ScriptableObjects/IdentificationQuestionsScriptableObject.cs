using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Identification Questions", menuName ="ScriptableObjects/Identification Questions")]
public class IdentificationQuestionsScriptableObject : ScriptableObject
{
    public List<PlantIdentificationParameterScriptableObject> questions;
}

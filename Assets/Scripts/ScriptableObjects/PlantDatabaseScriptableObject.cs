using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Plant Database", menuName = "ScriptableObjects/Plant Database")]
public class PlantDatabaseScriptableObject : ScriptableObject
{
    public List<PlantScriptableObject> plants;
}

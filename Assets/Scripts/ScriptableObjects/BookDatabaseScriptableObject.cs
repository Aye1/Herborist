using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Book Database", menuName = "ScriptableObjects/Book Database")]
public class BookDatabaseScriptableObject : SerializedScriptableObject
{
    public List<PlantScriptableObject> plants;
}

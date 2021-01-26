using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

public class PrefabContextualMenuCreator : MonoBehaviour
{
    private static string _baseButtonPath = "Base Button";
    private static string _highlightButtonPath = "Highlight Button";

    [MenuItem("GameObject/Botaniceae/UI/Base Button", false, 8)]
    public static void CreateBaseButton()
    {
        InstantiatePrefab(_baseButtonPath);
    }

    [MenuItem("GameObject/Botaniceae/UI/Highlight Button", false, 8)]
    public static void CreateHighlightButton()
    {
        InstantiatePrefab(_highlightButtonPath);
    }

    private static void InstantiatePrefab(string path)
    {
        GameObject prefab = Resources.Load<GameObject>(path);
        GameObject parent = Selection.activeGameObject;
        Transform t = parent == null ? null : parent.transform;
        GameObject newObject = PrefabUtility.InstantiatePrefab(prefab, t) as GameObject;
        newObject.name = path;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

public class PrefabContextualMenuCreator : MonoBehaviour
{
    private static string _baseButtonPath = "BaseButton";

    [MenuItem("GameObject/Botaniceae/UI/BaseButton", false, 8)]
    public static void CreateBaseButton()
    {
        InstantiatePrefab(_baseButtonPath);
    }

    private static void InstantiatePrefab(string path)
    {
        GameObject prefab = Resources.Load<GameObject>(path);
        GameObject parent = Selection.activeGameObject;
        Transform t = parent == null ? null : parent.transform;
        GameObject newObject = Instantiate(prefab, t);
        newObject.name = "Base Button";
    }
}

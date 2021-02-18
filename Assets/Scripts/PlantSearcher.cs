using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;

public class PlantSearcher : MonoBehaviour
{
    public List<PlantScriptableObject> plants;

    public PlantIdentificationParameterScriptableObject paramTest;
    public PlantIdentificationValueScriptableObject valueTest;

    public List<PlantScriptableObject> foundPlants;

    public static PlantSearcher Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [Button("Search plants")]
    private void TestSearch()
    {
        Dictionary<PlantIdentificationParameterScriptableObject, PlantIdentificationValueScriptableObject> testParams = new Dictionary<PlantIdentificationParameterScriptableObject, PlantIdentificationValueScriptableObject>();
        testParams.Add(paramTest, valueTest);
        foundPlants = FindPlants(testParams);
    }

    public List<PlantComponentScriptableObject> FindComponents(Dictionary<PlantIdentificationParameterScriptableObject, PlantIdentificationValueScriptableObject> parameters)
    {
        List<PlantComponentScriptableObject> components = plants.SelectMany(p => p.components).ToList();
        foreach(KeyValuePair<PlantIdentificationParameterScriptableObject, PlantIdentificationValueScriptableObject> param in parameters)
        {
            components = components.Where(c => IsValid(c, param.Key, param.Value)).ToList();
            Debug.LogFormat("Results found after param {1}: {0}", components.Count, param.Key.name);
        }
        return components;
    }

    public List<PlantScriptableObject> FindPlants(Dictionary<PlantIdentificationParameterScriptableObject, PlantIdentificationValueScriptableObject> parameters)
    {
        List<PlantComponentScriptableObject> filteredComponents = FindComponents(parameters);
        return plants.Where(p => p.components.Any(c => filteredComponents.Contains(c))).ToList();
    }

    private bool IsValid(PlantComponentScriptableObject component, PlantIdentificationParameterScriptableObject param, PlantIdentificationValueScriptableObject value)
    {
        bool res = component.parameters != null;
        res = res && component.parameters.ContainsKey(param) && component.parameters[param] == value;
        return res;
    }
}

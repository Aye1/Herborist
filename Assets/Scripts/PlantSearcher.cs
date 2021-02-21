using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;

public class PlantSearcher : MonoBehaviour
{
    [SerializeField, Required, AssetsOnly] private PlantDatabaseScriptableObject _database;

#if UNITY_EDITOR
    public PlantIdentificationParameterScriptableObject paramTest;
    public PlantIdentificationValueScriptableObject valueTest;
#endif

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

#if UNITY_EDITOR
    [Button("Search plants")]
    private void TestSearch()
    {
        Dictionary<PlantIdentificationParameterScriptableObject, PlantIdentificationValueScriptableObject> testParams = new Dictionary<PlantIdentificationParameterScriptableObject, PlantIdentificationValueScriptableObject>();
        testParams.Add(paramTest, valueTest);
        foundPlants = FindPlants(testParams);
    }
#endif

    public List<PlantComponentScriptableObject> FindComponents(Dictionary<PlantIdentificationParameterScriptableObject, PlantIdentificationValueScriptableObject> parameters)
    {
        List<PlantComponentScriptableObject> components = _database.plants.SelectMany(p => p.components).ToList();
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
        return _database.plants.Where(p => p.components.Any(c => filteredComponents.Contains(c))).ToList();
    }

    private bool IsValid(PlantComponentScriptableObject component, PlantIdentificationParameterScriptableObject param, PlantIdentificationValueScriptableObject value)
    {
        bool res = component.parameters != null;
        res = res && component.parameters.ContainsKey(param) && component.parameters[param] == value;
        return res;
    }
}

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
            DontDestroyOnLoad(gameObject);
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
            components = components.Where(c => c.parameters.ContainsKey(param.Key) && c.parameters[param.Key] == param.Value).ToList();
        }
        return components;
    }

    public List<PlantScriptableObject> FindPlants(Dictionary<PlantIdentificationParameterScriptableObject, PlantIdentificationValueScriptableObject> parameters)
    {
        List<PlantComponentScriptableObject> filteredComponents = FindComponents(parameters);
        return plants.Where(p => p.components.Any(c => filteredComponents.Contains(c))).ToList();
    }
}

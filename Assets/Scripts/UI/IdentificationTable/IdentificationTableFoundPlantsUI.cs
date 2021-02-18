using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class IdentificationTableFoundPlantsUI : MonoBehaviour
{
    [Title("Editor bindings")]
    [SerializeField, Required, AssetsOnly] private PlantDatabaseScriptableObject _database;
    [SerializeField, Required, AssetsOnly] private PlantComponentUI _plantFoundTemplate;
    [SerializeField, Required] private Transform _plantsFoundHolder;
    [SerializeField, Required] private PlantSearcher _plantSearcher;

    private Dictionary<PlantIdentificationParameterScriptableObject, PlantIdentificationValueScriptableObject> _filters;

    public Dictionary<PlantIdentificationParameterScriptableObject, PlantIdentificationValueScriptableObject> Filters
    {
        get { return _filters; }
        set
        {
            _filters = value;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        ClearResults();
        _plantSearcher.plants = _database.plants;
        List<PlantComponentScriptableObject> foundComponents = _plantSearcher.FindComponents(Filters);
        foreach(PlantComponentScriptableObject component in foundComponents)
        {
            PlantComponentUI newObj = Instantiate(_plantFoundTemplate, _plantsFoundHolder);
            newObj.Component = component;
        }
    }

    private void ClearResults()
    {
        foreach(Transform child in _plantsFoundHolder.transform)
        {
            Destroy(child.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

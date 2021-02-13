using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

public class PlantInformationUI : MonoBehaviour
{
    [SerializeField, Required, ChildGameObjectsOnly] private TextMeshProUGUI _plantName;

    private Collectible _currentCollectible;
    public Collectible CurrentCollectible
    {
        get { return _currentCollectible; }
        set
        {
            if(_currentCollectible != value)
            {
                _currentCollectible = value;
                UpdateUI();
            }
        }
    }

    private void UpdateUI()
    {
        if(_currentCollectible != null)
        {
            _plantName.text = PlantIdentificationInfos.Instance.GetPlantCurrentName(_currentCollectible.collectible);
        }
    }
}

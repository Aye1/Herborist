using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Linq;

[Serializable]
public class ProbabilityPair
{
    private uint _weight = 1;

    [OdinSerialize, ShowInInspector, HorizontalGroup(Width = 100), PropertyOrder(-1)]
    public uint Weight
    {
        get { return _weight; }
        set
        {
            if (value != _weight)
            {
                _weight = value;
                OnWeightChanged?.Invoke(this);
            }
        }
    }
    
    [HorizontalGroup(), HideLabel]
    public GameObject obj;
    [ReadOnly, MinMaxSlider(0.0f, 1.0f, ShowFields = true), PropertySpace(SpaceBefore = 20)]
    public Vector2 probabilityRange;

    [ShowInInspector, DisplayAsString, HideLabel]
    public string probabilityText
    {
        get {
            string formattedProba = (probabilityRange.y - probabilityRange.x).ToString("P");
            return "Probability: " +  formattedProba;
        }
    }

    public delegate void WeightChange(ProbabilityPair obj);
    public static WeightChange OnWeightChanged;

}

[ExecuteAlways]
[CreateAssetMenu(fileName = "Probability Map", menuName = "ScriptableObjects/Probability map")]
public class ProbabilityMap : ScriptableObject
{
    [OnCollectionChanged("UpdateProbabilities")]
    public List<ProbabilityPair> probabilities;

    /// <summary>
    /// Number of possible elements in the map
    /// </summary>
    public int Count
    {
        get { return probabilities.Count; }
    }

    /// <summary>
    /// Exactly the same as Count
    /// </summary>
    public int Length
    {
        get { return probabilities.Count; }
    }

    [Button("Balance probabilities")]
    public void BalanceProbabilities()
    {
        probabilities.ForEach(p => p.Weight = 10);
        UpdateProbabilities();
    }

    private void OnEnable()
    {
        ProbabilityPair.OnWeightChanged += OnProbabilityWeightChanged;
        UpdateProbabilities();
    }

    private void OnDisable()
    {
        ProbabilityPair.OnWeightChanged -= OnProbabilityWeightChanged;
    }

    private void OnProbabilityWeightChanged(ProbabilityPair pair)
    {
        UpdateProbabilities();
    }

    private void UpdateProbabilities()
    {
        if(probabilities == null || probabilities.Count == 0)
        {
            return;
        }
        uint totalWeight = Convert.ToUInt32(probabilities.Sum(p => p.Weight));
        float minRange = 0.0f;

        foreach(ProbabilityPair pair in probabilities)
        {
            float range = pair.Weight / (float)totalWeight;
            // Clamp max value to 1.0f, in case of rounding errors accumulating
            float maxRange = Mathf.Min(minRange + range, 1.0f);
            pair.probabilityRange = new Vector2(minRange, maxRange);
            minRange = maxRange;
        }
    }

    public GameObject GetRandomObject()
    {
        float alea = Alea.GetFloat(0.0f, 1.0f);
        return GetRandomObject(alea);
    }

    public GameObject GetRandomObject(float random)
    {
        if(random < 0.0f || random > 1.0f)
        {
            throw new ArgumentOutOfRangeException("Random number 'random' must be between 0.0f and 1.0f");
        }
        return probabilities.Where(p => p.probabilityRange.x <= random && p.probabilityRange.y > random).First().obj;
    }
}

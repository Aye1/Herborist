using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using System.Linq;

[Serializable]
public class SpriteQuantityBinder
{
    [HideLabel, PreviewField, HorizontalGroup("Group", Width = 50), PropertyOrder(2)]
    public Sprite sprite;

    [MinMaxSlider(0,100, true), HideLabel]
    [GUIColor("GetFieldColor")]
    [PropertySpace(SpaceBefore = 20), HorizontalGroup("Group"), PropertyOrder(1)]
    public Vector2Int minMaxPercentage;

    [HideInInspector]
    public SpriteDependsOnQuantity parent;

    public override string ToString()
    {
        return "[" + minMaxPercentage.x + " - " + minMaxPercentage.y + "]";
    }

    private Color GetFieldColor()
    {
        return parent == null ? Color.white : parent.GetValidationColor(this);
    }
}

[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteDependsOnQuantity : MonoBehaviour
{
    [InfoBox("@this.GetConsistencyErrorMessage()", VisibleIf = "@!this.IsConsistent()", InfoMessageType = InfoMessageType.Warning)]

    [SerializeField]
    [ListDrawerSettings(CustomAddFunction = "AddElement")]
    private List<SpriteQuantityBinder> _quantities;

    private List<SpriteQuantityBinder> _conflictingQuantities;

    private List<Vector2Int> _missingRanges;

    private SpriteRenderer _renderer;

    private int _currentPercentage;

    public int CurrentPercentage
    {
        get { return _currentPercentage; }
        set
        {
            if(value != _currentPercentage)
            {
                _currentPercentage = value;
                UpdateSprite();
            }
        }
    }

    public List<Vector2Int> MissingRanges
    {
        get
        {
            if(_missingRanges == null)
            {
                _missingRanges = new List<Vector2Int>();
            }
            return _missingRanges;
        }
    }

    void Awake()
    {
        _conflictingQuantities = new List<SpriteQuantityBinder>();
        _missingRanges = new List<Vector2Int>();
        _renderer = GetComponent<SpriteRenderer>();
        UpdateSprite();
    }

    // Update is called once per frame
    void Update()
    {
        CheckConsistency();
    }

    private void UpdateSprite()
    {
        SpriteQuantityBinder binder = _quantities.Where(b => b.minMaxPercentage.x <= _currentPercentage
            && b.minMaxPercentage.y >= _currentPercentage).FirstOrDefault();
        if(binder != default(SpriteQuantityBinder) && _renderer != null)
        {
            _renderer.sprite = binder.sprite;
        }
    }

    private SpriteQuantityBinder AddElement()
    {
        SpriteQuantityBinder newBinder = new SpriteQuantityBinder();
        newBinder.minMaxPercentage = new Vector2Int(0, 100);
        newBinder.parent = this;
        return newBinder;
    }

    private void CheckConsistency()
    {
        _conflictingQuantities.Clear();
        MissingRanges.Clear();
        if(_quantities.Count == 0)
        {
            return;
        }

        SpriteQuantityBinder[] ordered = _quantities.OrderBy(q => q.minMaxPercentage.x).ToArray();
        if(ordered[0].minMaxPercentage.x > 0)
        {
            MissingRanges.Add(new Vector2Int(0, ordered[0].minMaxPercentage.x - 1));
        }
        for(int i=0; i < ordered.Length - 1; i++)
        {
            int currentMax = ordered[i].minMaxPercentage.y;
            int nextMin = ordered[i + 1].minMaxPercentage.x;
            if(currentMax >= nextMin)
            {
                if(!_conflictingQuantities.Contains(ordered[i]))
                    _conflictingQuantities.Add(ordered[i]);
                if(!_conflictingQuantities.Contains(ordered[i+1]))
                    _conflictingQuantities.Add(ordered[i + 1]);
            }

            if(nextMin > currentMax + 1)
            {
                MissingRanges.Add(new Vector2Int(currentMax+1, nextMin-1));
            }
        }
        if(ordered[ordered.Length-1].minMaxPercentage.y < 100)
        {
            MissingRanges.Add(new Vector2Int(ordered[ordered.Length - 1].minMaxPercentage.y + 1, 100));
        }
    }

    public bool IsConsistent()
    {
        if(MissingRanges != null && _conflictingQuantities != null)
            return MissingRanges.Count == 0 && _conflictingQuantities.Count == 0;
        return true;
    }

    public Color GetValidationColor(SpriteQuantityBinder binder)
    {
        if (_conflictingQuantities == null)
            return Color.white;
        return _conflictingQuantities.Contains(binder) ? Color.yellow : Color.white;
    }

    private string GetConsistencyErrorMessage()
    {
        string res = "Consistency error: \n";
        if(_conflictingQuantities.Count > 0)
        {
            res += "* Overlapping ranges: \n";
            foreach(SpriteQuantityBinder b in _conflictingQuantities)
            {
                res += "    Object at position " + _quantities.IndexOf(b) + " " + b.ToString() + "\n";
            }
        }
        if(MissingRanges.Count > 0)
        {
            res += "* Missing ranges: \n";
            foreach(Vector2Int range in MissingRanges)
            {
                res += "    [" + range.x + " - " + range.y + "] \n";
            }
        }
        return res;
    }
}

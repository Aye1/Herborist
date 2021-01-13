using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

public class BookPageUI : MonoBehaviour
{
    [SerializeField, Required, ChildGameObjectsOnly] private TextMeshProUGUI _pageNumberText;

    private IdentificationKeyData _plant;
    public IdentificationKeyData Plant
    {
        get { return _plant; }
        set
        {
            if (_plant != value)
            {
                _plant = value;
                UpdateUI();
            }
        }
    }

    private int _pageNumber = -1; //-1 forces the first update for page 0
    public int PageNumber
    {
        get { return _pageNumber; }
        set
        {
            if(_pageNumber != value)
            {
                _pageNumber = value;
                UpdatePageNumber();
            }
        }
    }

    protected virtual void UpdateUI() { }

    private void UpdatePageNumber()
    {
        _pageNumberText.text = _pageNumber.ToString();
    }
}

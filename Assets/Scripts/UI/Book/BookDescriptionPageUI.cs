using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

public class BookDescriptionPageUI : BookPageUI
{
    [SerializeField, Required, ChildGameObjectsOnly] private TextMeshProUGUI _plantNameText;

    protected override void UpdateUI()
    {
        if (Plant == null)
        {
            UpdateEmptyUI();
        }
        else
        {
            _plantNameText.text = Plant.identificationTitle;
        }
    }

    void UpdateEmptyUI()
    {
        _plantNameText.text = "Unknown";
    }
}

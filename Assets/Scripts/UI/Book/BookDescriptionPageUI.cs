using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.UI;

public class BookDescriptionPageUI : BookPageUI
{
    [SerializeField, Required, ChildGameObjectsOnly] private TextMeshProUGUI _plantNameText;
    [SerializeField, Required, ChildGameObjectsOnly] private Image _plantSpriteImage;

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

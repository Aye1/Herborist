using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;

public class BookPlateUI : BookPageUI
{
    [SerializeField, Required, ChildGameObjectsOnly] private TextMeshProUGUI _plantNameText;
    [SerializeField, Required, ChildGameObjectsOnly] private Image _plantFullImage;

    protected override void UpdateUI()
    {
        if (Plant == null)
        {
            UpdateEmptyUI();
        }
        else
        {
            _plantNameText.text = Plant.name;
            _plantFullImage.sprite = Plant.fullPicture;
        }
    }

    void UpdateEmptyUI()
    {
        _plantNameText.text = "Unknown";
        _plantFullImage.sprite = null;
    }
}

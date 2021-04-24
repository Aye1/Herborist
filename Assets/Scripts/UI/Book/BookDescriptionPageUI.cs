using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.UI;

public class BookDescriptionPageUI : BookPageUI
{
    [SerializeField, Required, ChildGameObjectsOnly] private TextMeshProUGUI _plantNameText;
    [SerializeField, Required, ChildGameObjectsOnly] private TextMeshProUGUI _plantBotanicalNameText;
    [SerializeField, Required, ChildGameObjectsOnly] private Image _plantSpriteImage;
    [SerializeField, Required, ChildGameObjectsOnly] private BaseButton _plantIdentificationButton;

    public override void UpdateUI()
    {
        if (Plant != null && PlantIdentificationInfos.Instance.IsIdentified(Plant))
        {
            UpdateFullUI();
        }
        else
        {
            UpdateEmptyUI();
        }
    }

    private void UpdateFullUI()
    {
        _plantNameText.text = Plant.commonNameLocKey;
        _plantBotanicalNameText.text = Plant.familyName;
        _plantSpriteImage.sprite = Plant.inGameSprite;
        _plantIdentificationButton.GetComponent<Button>().interactable = false;
        _plantIdentificationButton.gameObject.SetActive(false);
    }

    private void UpdateEmptyUI()
    {
        _plantNameText.text = "Unknown";
        _plantBotanicalNameText.text = "";
        _plantSpriteImage.sprite = null;
        bool isButtonHidden = true;
        int plantCount = 0;
        if (Plant != null)
        {
            List<PlantScriptableObject> plantList = PlantSearcher.Instance.GetPlantPossibilitiesFromDiscoveredComponents(Plant);
            plantCount = plantList.Count;
            if (plantCount > 0)
            {
                isButtonHidden = false;
            }
        }
        _plantIdentificationButton.GetComponent<Button>().interactable = !isButtonHidden;
        _plantIdentificationButton.gameObject.SetActive(!isButtonHidden);

        if (plantCount > 1)
        {
            _plantIdentificationButton.GetComponentInChildren<TextMeshProUGUI>().text = plantCount + " plants possibles";
        }
        else if (plantCount == 1)
        {
            NavigationManager.Instance.SetFocus(_plantIdentificationButton.gameObject);
        }
    }
}

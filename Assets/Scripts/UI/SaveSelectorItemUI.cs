using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using Unisloth.Localization;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SaveSelectorItemUI : MonoBehaviour
{
    [Title("Static parameters")]
    [TranslationKey]
    public string saveNameLocKey;

    [Title("GameObjects bindings")]
    [SerializeField, Required, ChildGameObjectsOnly] private TextMeshProUGUI _saveNameText;

    public GameInfo associatedInfo;

    private void OnEnable()
    {
        UpdateUI();
        BindOnClick();
    }

    private void UpdateUI()
    {
        _saveNameText.text = LocalizationManager.Instance.GetTranslation(saveNameLocKey) + " " + associatedInfo.saveNumber;
    }

    private void BindOnClick()
    {
        GetComponent<Button>().onClick.AddListener(LaunchGame);
    }

    private void LaunchGame()
    {
        GameManager.Instance.LaunchGame(associatedInfo);
    }
}

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
    [TranslationKey, SerializeField]
    private string _saveNameLocKey;

    [TranslationKey, SerializeField]
    private string _timePlayedLocKey;


    [Title("GameObjects bindings")]
    [SerializeField, Required, ChildGameObjectsOnly] private TextMeshProUGUI _saveNameText;
    [SerializeField, Required, ChildGameObjectsOnly] private TextMeshProUGUI _timePlayedText;
    [SerializeField, Required, ChildGameObjectsOnly] private GameObject _newGameInfos;
    [SerializeField, Required, ChildGameObjectsOnly] private GameObject _startedGameInfos;

    public GameInfo associatedInfo;

    private void OnEnable()
    {
        UpdateUI();
        BindOnClick();
    }

    private void UpdateUI()
    {
        _saveNameText.text = LocalizationManager.Instance.GetTranslation(_saveNameLocKey) + " " + associatedInfo.saveNumber;
        _timePlayedText.text = LocalizationManager.Instance.GetTranslation(_timePlayedLocKey) + " " + FormattedTimeString(associatedInfo.TimePlayedSeconds);
        _newGameInfos.SetActive(!associatedInfo.isGameStarted);
        _startedGameInfos.SetActive(associatedInfo.isGameStarted);
    }

    private void BindOnClick()
    {
        GetComponent<Button>().onClick.AddListener(LaunchGame);
    }

    private void LaunchGame()
    {
        GameManager.Instance.LaunchGame(associatedInfo);
    }

    private string FormattedTimeString(float timeSeconds)
    {
        int fullTime = Mathf.FloorToInt(timeSeconds);
        int hours = fullTime / 3600;
        int minutes = fullTime % 3600 / 60;
        int seconds = fullTime % 3600 % 60;
        return hours.ToString("00") + ":" + minutes.ToString("00") + ":" + seconds.ToString("00");
    }
}

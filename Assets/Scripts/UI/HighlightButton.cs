using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using MoreMountains.Feedbacks;
using UnityEngine.EventSystems;
using Unisloth.Localization;
using TMPro;

[ExecuteAlways]
public class HighlightButton : BaseButton
{
    [SerializeField, Required, ChildGameObjectsOnly] private TextMeshProUGUI _buttonText;
    [SerializeField, Required, ChildGameObjectsOnly] private TextMeshProUGUI _highlightText;
    [SerializeField, Required, ChildGameObjectsOnly] private MMFeedbacks _selectFeedbacks;
    [SerializeField, Required, ChildGameObjectsOnly] private MMFeedbacks _deselectFeedbacks;

    [TranslationKey, OnValueChanged("UpdateTexts")]
    public string localizationKey;

    private void Awake()
    {
        _selectFeedbacks?.Initialization();
        _deselectFeedbacks?.Initialization();
    }

    private void Start()
    {
        UpdateTexts();
    }

    public override void OnSelect(BaseEventData eventData)
    {
        if (Application.isPlaying)
        {
            Debug.Log("onselect " + gameObject.name);
            base.OnSelect(eventData);
            _deselectFeedbacks.StopFeedbacks();
            _selectFeedbacks.PlayFeedbacks();
        }
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        if (Application.isPlaying)
        {
            Debug.Log("ondeselect " + gameObject.name);
            base.OnDeselect(eventData);

            _selectFeedbacks.StopFeedbacks();
            _deselectFeedbacks?.PlayFeedbacks();
        }
    }

    void UpdateTexts()
    {
        _buttonText.text = LocalizationManager.Instance.GetTranslation(localizationKey);
        _highlightText.text = _buttonText.text;
    }
}

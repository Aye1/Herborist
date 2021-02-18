using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.UI;
using Unisloth.Localization;


[RequireComponent(typeof(Button))]
public class IdentificationAnswerUI : MonoBehaviour
{
    [Title("Editor bindings")]
    [SerializeField, Required, ChildGameObjectsOnly] private TextMeshProUGUI _answerText;
    [SerializeField, Required, ChildGameObjectsOnly] private Image _answerIcon;

    private PlantIdentificationValueScriptableObject _answer;

    public PlantIdentificationValueScriptableObject Answer
    {
        get { return _answer; }
        set
        {
            if(_answer != value)
            {
                _answer = value;
                UpdateUI();
            }
        }
    }

    public Button SelfButton
    {
        get { return GetComponent<Button>(); }
    }

    private void UpdateUI()
    {
        if(Answer != null)
        {
            _answerText.text = LocalizationManager.Instance.GetTranslation(Answer.valueNameLocKey);
            _answerIcon.sprite = Answer.icon;
        }
    }
}

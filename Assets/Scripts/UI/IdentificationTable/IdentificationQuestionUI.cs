using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using Unisloth.Localization;

public class IdentificationQuestionUI : MonoBehaviour
{
    [Title("Editor bindings - Assets")]
    [SerializeField, Required, AssetsOnly] private IdentificationAnswerUI _answerTemplate;

    [Title("Editor bindings - Scene")]
    [SerializeField, Required, ChildGameObjectsOnly] private TextMeshProUGUI _questionText;
    [SerializeField, Required, ChildGameObjectsOnly] private Transform _answersHolder;

    private PlantIdentificationParameterScriptableObject _question;
    private PlantIdentificationValueScriptableObject _selectedAnswer;

    [HideInInspector]public IdentificationQuestionsListUI parentQuestionList;

    public PlantIdentificationParameterScriptableObject Question
    {
        get { return _question; }
        set
        {
            if (_question != value)
            {
                _question = value;
                UpdateUI();
            }
        }
    }

    private void UpdateUI()
    {
        _questionText.text = LocalizationManager.Instance.GetTranslation(Question.questionLocKey);
        bool isFirst = true;
        foreach (PlantIdentificationValueScriptableObject answer in Question.possibleValues)
        {
            IdentificationAnswerUI newAnswer = Instantiate(_answerTemplate, _answersHolder);
            newAnswer.Answer = answer;
            newAnswer.SelfButton.onClick.AddListener(() => OnAnswerSelected(answer));
            if (isFirst)
            {
                NavigationManager.Instance.SetFocus(newAnswer.gameObject);
                isFirst = false;
            }
        }
    }

    private void OnAnswerSelected(PlantIdentificationValueScriptableObject answer)
    {
        parentQuestionList.SetAnswer(Question, answer);
        _selectedAnswer = answer;
    }

    private void ReselectAnswer()
    {
        if (_selectedAnswer != null)
        {
            foreach (IdentificationAnswerUI child in _answersHolder.GetComponentsInChildren<IdentificationAnswerUI>())
            {
                if (child.Answer == _selectedAnswer)
                {
                    NavigationManager.Instance.SetFocus(child.gameObject);
                }
            }
        } else
        {
            NavigationManager.Instance.SetFocus(_answersHolder.GetChild(0).gameObject);
        }
    }

    public void Display(bool displayed)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(displayed);
        }
        if(displayed)
        {
            ReselectAnswer();
        }
    }

    //#region INavigable
    //public bool IsRemovable()
    //{
    //    return true;
    //}

    //public void OnCancel()
    //{
    //    //Display(false);
    //    parentQuestionList.GoToPreviousQuestion();
    //}

    //public void OnComingBack()
    //{
    //    Display(true);
    //    ReselectAnswer();
    //}

    //public void OnNavigate()
    //{
    //    Display(true);
    //}
    //#endregion
}

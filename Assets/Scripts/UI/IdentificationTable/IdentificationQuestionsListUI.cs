using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using Unisloth.Localization;
using UnityEngine.EventSystems;

public class IdentificationQuestionsListUI : MonoBehaviour
{
    [Title("Editor bindings - Assets")]
    [SerializeField, Required, AssetsOnly] private IdentificationQuestionsScriptableObject _questions;
    [SerializeField, Required, AssetsOnly] private IdentificationAnswerUI _answerTemplate;

    [Title("Editor bindings - Child objects")]
    [SerializeField, Required, ChildGameObjectsOnly] private TextMeshProUGUI _questionText;
    [SerializeField, Required] private Transform _answersHolder;
    [SerializeField, Required, ChildGameObjectsOnly] private IdentificationTableFoundPlantsUI _foundPlantsUI;

    [Title("Localization")]
    [SerializeField, TranslationKey]
    private string _foundPlantsLocKey;

    private PlantIdentificationParameterScriptableObject _currentQuestion;
    private Dictionary<PlantIdentificationParameterScriptableObject, PlantIdentificationValueScriptableObject> _givenAnswers;
    private int _currentQuestionIndex;

    public PlantIdentificationParameterScriptableObject CurrentQuestion
    {
        get { return _currentQuestion; }
        set
        {
            if(_currentQuestion != value)
            {
                _currentQuestion = value;
                UpdateQuestionUI();
            }
        }
    }

    private void OnEnable()
    {
        Reset();
    }

    public void Reset()
    {
        TogglePossiblePlantsVisibility(false);
        if(_givenAnswers == null)
        {
            _givenAnswers = new Dictionary<PlantIdentificationParameterScriptableObject, PlantIdentificationValueScriptableObject>();
        } else
        {
            _givenAnswers.Clear();
        }
        SetQuestion(0);
    }

    public void SetQuestion(int index)
    {
        if (index >= 0 && index < _questions.questions.Count)
        {
            _currentQuestionIndex = index;
            CurrentQuestion = _questions.questions[index];
        }
        else
        {
            Debug.LogErrorFormat("Index ({0}) out of range (1)", index, _questions.questions.Count);
        }
    }

    public void GoToNextQuestion()
    {
        if (!IsLastQuestion())
        {
            SetQuestion(_currentQuestionIndex + 1);
        }
    }

    public void GoToPreviousQuestion()
    {
        if (_currentQuestionIndex > 0)
        {
            SetQuestion(_currentQuestionIndex - 1);
        }
    }

    public bool IsLastQuestion()
    {
        return _currentQuestionIndex == _questions.questions.Count-1;
    }

    void TogglePossiblePlantsVisibility(bool active)
    {
        _foundPlantsUI.gameObject.SetActive(active);
        _answersHolder.gameObject.SetActive(!active);
        if (active)
        {
            UpdatePossiblePlants();
        }
    }

    void UpdatePossiblePlants()
    {
        _questionText.text = LocalizationManager.Instance.GetTranslation(_foundPlantsLocKey);
        _foundPlantsUI.Filters = _givenAnswers;
    }

    void OnAnswerSelected(PlantIdentificationValueScriptableObject answer)
    {
        SetAnswer(answer);
        if(!IsLastQuestion())
        {
            GoToNextQuestion();
        } else
        {
            TogglePossiblePlantsVisibility(true);
        }
    }

    void SetAnswer(PlantIdentificationValueScriptableObject answer)
    {
        if (_givenAnswers.ContainsKey(CurrentQuestion))
        {
            _givenAnswers.Remove(CurrentQuestion);
        }
        _givenAnswers.Add(CurrentQuestion, answer);
    }

    void UpdateQuestionUI()
    {
        CleanAnswers();
        _questionText.text = LocalizationManager.Instance.GetTranslation(CurrentQuestion.questionLocKey);
        foreach(PlantIdentificationValueScriptableObject answer in CurrentQuestion.possibleValues)
        {
            IdentificationAnswerUI newAnswer = Instantiate(_answerTemplate, _answersHolder);
            newAnswer.Answer = answer;
            newAnswer.SelfButton.onClick.AddListener(() => OnAnswerSelected(answer));
        }
        SetFocus(_answersHolder.GetChild(0).gameObject);
    }

    void CleanAnswers()
    {
        if (_answersHolder.childCount == 0)
            return;
        foreach(Transform child in _answersHolder.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void SetFocus(GameObject obj)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(obj);
    }
}

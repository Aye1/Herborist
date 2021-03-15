using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using Unisloth.Localization;
using UnityEngine.UI;

public class IdentificationQuestionsListUI : MonoBehaviour, INavigable
{
    [Title("Editor bindings - Assets")]
    [SerializeField, Required, AssetsOnly] private IdentificationQuestionsScriptableObject _questions;
    [SerializeField, Required, AssetsOnly] private IdentificationQuestionUI _questionTemplate;

    [Title("Editor bindings - Children objects")]
    [SerializeField, Required, ChildGameObjectsOnly] private Transform _questionHolder;
    [SerializeField, Required, ChildGameObjectsOnly] private IdentificationTableFoundPlantsUI _foundPlantsUI;
    [SerializeField, Required, ChildGameObjectsOnly] private PlantSearcher _plantSearcher;
    [SerializeField, Required, ChildGameObjectsOnly] private GameObject _resultsUI;
    [SerializeField, Required, ChildGameObjectsOnly] private TextMeshProUGUI _resultsText;
    [SerializeField, Required, ChildGameObjectsOnly] private Image _componentToIdentifyImage;
    [SerializeField, Required, ChildGameObjectsOnly] private Button _componentIdentifiedButton;

    [Title("Localization")]
    [SerializeField, TranslationKey]
    private string _foundPlantsLocKey;
    [SerializeField, TranslationKey]
    private string _resultsTitleLocKey;

    private PlantComponentScriptableObject _currentComponent;
    private Dictionary<PlantIdentificationParameterScriptableObject, PlantIdentificationValueScriptableObject> _givenAnswers;
    private Dictionary<PlantIdentificationParameterScriptableObject, IdentificationQuestionUI> _questionUIs;

    private int _currentQuestionIndex;

    [HideInInspector] public IdentificationTableV2UI tableParent;

    public delegate void ComponentIdentified(PlantComponentScriptableObject component);
    public ComponentIdentified OnComponentIdentified;

    public PlantComponentScriptableObject CurrentComponent
    {
        get { return _currentComponent; }
        set
        {
            if(_currentComponent != value)
            {
                _currentComponent = value;
                _componentToIdentifyImage.sprite = _currentComponent.componentPicture;
                Reset();
            }
        }
    }

    public void Reset()
    {
        TogglePossiblePlantsVisibility(false);
        ToggleResultsVisibility(false);
        if(_givenAnswers == null)
        {
            _givenAnswers = new Dictionary<PlantIdentificationParameterScriptableObject, PlantIdentificationValueScriptableObject>();
        } else
        {
            _givenAnswers.Clear();
        }
        if(_questionUIs == null)
        {
            _questionUIs = new Dictionary<PlantIdentificationParameterScriptableObject, IdentificationQuestionUI>();
        }
        CreateQuestionsUI();
        SetQuestion(0);
    }

    public void SetQuestion(int index, bool pushToNav = true)
    {
        if (index >= 0 && index < _questions.questions.Count)
        {
            _currentQuestionIndex = index;
            OpenQuestionUI(_questions.questions[index], pushToNav);
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
            SetQuestion(_currentQuestionIndex - 1, false);
        }
    }

    public bool IsLastQuestion()
    {
        return _currentQuestionIndex == _questions.questions.Count-1;
    }

    public void SetAnswer(PlantIdentificationParameterScriptableObject question, PlantIdentificationValueScriptableObject answer)
    {
        if (_givenAnswers.ContainsKey(question))
        {
            _givenAnswers.Remove(question);
        }
        _givenAnswers.Add(question, answer);
        if (!IsLastQuestion())
        {
            GoToNextQuestion();
        }
        else
        {
            //TogglePossiblePlantsVisibility(true);
            ToggleResultsVisibility(true);
        }
    }

    private void CreateQuestionsUI()
    {
        if(_questionUIs != null)
        {
            if(_questionUIs.Count == _questions.questions.Count)
            {
                return;
            }
            foreach(PlantIdentificationParameterScriptableObject question in _questions.questions)
            {
                CreateQuestionUI(question);
            }
        }
    }

    private void CreateQuestionUI(PlantIdentificationParameterScriptableObject question)
    {
        IdentificationQuestionUI newQuestion = Instantiate(_questionTemplate, _questionHolder);
        newQuestion.Question = question;
        newQuestion.parentQuestionList = this;
        _questionUIs.Add(question, newQuestion);
    }

    private void OpenQuestionUI(PlantIdentificationParameterScriptableObject question, bool pushToNavigation)
    {
        foreach(IdentificationQuestionUI q in _questionUIs.Values)
        {
            q.Display(q.Question == question);
        }
        /*if(_questionUIs.ContainsKey(question))
        {
            _questionUIs[question].Display(true);
            if(pushToNavigation)
            {
                NavigationManager.Instance.PushNavigation(_questionUIs[question]);
            }
        }
        else
        {
            IdentificationQuestionUI newQuestion = Instantiate(_questionTemplate, _questionHolder);
            newQuestion.Question = question;
            newQuestion.parentQuestionList = this;
            _questionUIs.Add(question, newQuestion);
            NavigationManager.Instance.PushNavigation(newQuestion);
        }*/
    }

    void ToggleResultsVisibility(bool active)
    {
        _resultsUI.gameObject.SetActive(active);
        _questionHolder.gameObject.SetActive(!active);
        _foundPlantsUI.gameObject.SetActive(false);
        if(active)
        {
            SetResultsUI();
        }
    }

    void TogglePossiblePlantsVisibility(bool active)
    {
        _foundPlantsUI.gameObject.SetActive(active);
        _questionHolder.gameObject.SetActive(!active);
        if (active)
        {
            UpdatePossiblePlants();
        }
    }

    void UpdatePossiblePlants()
    {
        //_questionText.text = LocalizationManager.Instance.GetTranslation(_foundPlantsLocKey);
        _foundPlantsUI.Filters = _givenAnswers;
    }

    void SetResultsUI()
    {
        List<PlantComponentScriptableObject> results = _plantSearcher.FindComponents(_givenAnswers);
        bool isValid = results.Contains(CurrentComponent);
        //_questionText.text = LocalizationManager.Instance.GetTranslation(_resultsTitleLocKey);
        _resultsText.text = isValid ? "Component correctly identified" : "Error in component identification, try again";
        _componentIdentifiedButton.gameObject.SetActive(isValid);
        _componentIdentifiedButton.onClick.AddListener(() => OnComponentIdentified?.Invoke(CurrentComponent));
    }

    #region INavigable
    public void OnNavigate()
    {
        Reset();
    }

    public void OnComingBack()
    {
        return;
    }

    public void OnCancel()
    {
        tableParent.GoBackToSelector();
    }

    public bool IsRemovable()
    {
        return true;
    }
    #endregion
}

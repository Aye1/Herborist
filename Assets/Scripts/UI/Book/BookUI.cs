using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using System;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BookUI : BasePopup
{
    [Title("Assets bindings")]
    [SerializeField, Required, AssetsOnly] private BookDatabaseScriptableObject _database;
    [SerializeField, Required, AssetsOnly] private BookPlateUI _plateTemplate;
    [SerializeField, Required, AssetsOnly] private BookPageUI _rightPageTemplate;
    [SerializeField, Required, AssetsOnly] private IdentificationTableV2UI _identificationTableUITemplate;


    [Title("Child objects bindings")]
    [SerializeField, Required, ChildGameObjectsOnly] private Transform _leftPageHolder;
    [SerializeField, Required, ChildGameObjectsOnly] private Transform _rightPageHolder;
    [SerializeField, Required, ChildGameObjectsOnly] private Image _leftControlImage;
    [SerializeField, Required, ChildGameObjectsOnly] private Image _rightControlImage;

    [Title("Scene bindings")]
    [SerializeField, Required] private Transform _canvas;

    [Title("Parameters")]
    [SerializeField] private float _pageTurnTime = 0.5f;
    [SerializeField] private AnimationCurve _pageOpeningAnimationCurve;
    [SerializeField] private AnimationCurve _pageClosingAnimationCurve;

    private int _currentPageNumber = 0;
    public int CurrentPageNumber
    {
        get { return _currentPageNumber; }
        set
        {
            if (_currentPageNumber != value && IsPageNumberCorrect(value))
            {
                _currentPageNumber = value;
                GoToPage(_currentPageNumber, true);
            }
        }
    }

    private IdentificationTableV2UI _identificationTable;
    public IdentificationTableV2UI IdentificationTable
    {
        get
        {
            if (_identificationTable == null)
            {
                _identificationTable = Instantiate(_identificationTableUITemplate, _canvas);
            }
            return _identificationTable;
        }
    }

    public bool IsAnimating { get; private set; }

    private int _maxPageNumber;

    // Simplified pool system, with two objects for each part left/right
    private BookPlateUI _leftPageA;
    private BookPlateUI _leftPageB;
    private BookPageUI _rightPageA;
    private BookPageUI _rightPageB;

    private BookPlateUI _currentLeftPage;
    private BookPageUI _currentRightPage;
    private BookPlateUI _hiddenLeftPage;
    private BookPageUI _hiddenRightPage;

    private InputActionAsset _actions;
    private InputAction _switchLeftAction;
    private InputAction _switchRightAction;

    private readonly string SWITCH_TAB_LEFT = "Custom UI/Switch Tab Left";
    private readonly string SWITCH_TAB_RIGHT = "Custom UI/Switch Tab Right";

    private void Awake()
    {
        InitPages();
        _maxPageNumber = _database.plants.Count();
        GoToPage(0);
        DisplayOnlyNecessaryPages();
    }

    private void InitPages()
    {
        _leftPageA = Instantiate(_plateTemplate, _leftPageHolder);
        _leftPageB = Instantiate(_plateTemplate, _leftPageHolder);
        _leftPageA.bookParent = this;
        _leftPageB.bookParent = this;
        _rightPageA = Instantiate(_rightPageTemplate, _rightPageHolder);
        _rightPageB = Instantiate(_rightPageTemplate, _rightPageHolder);
        _currentLeftPage = _leftPageA;
        _currentRightPage = _rightPageA;
        _hiddenLeftPage = _leftPageB;
        _hiddenRightPage = _rightPageB;
        SetPivotPoints();
    }

    private void SetPivotPoints()
    {
        Vector2 leftPagePivot = new Vector2(1.0f, 0.5f);
        Vector2 rightPagePivot = new Vector2(0.0f, 0.5f);
        _leftPageA.GetComponent<RectTransform>().pivot = leftPagePivot;
        _leftPageB.GetComponent<RectTransform>().pivot = leftPagePivot;
        _rightPageA.GetComponent<RectTransform>().pivot = rightPagePivot;
        _rightPageB.GetComponent<RectTransform>().pivot = rightPagePivot;
    }

    private void DisplayOnlyNecessaryPages()
    {
        GoToFront(_currentLeftPage);
        GoToFront(_currentRightPage);
        GoToBack(_hiddenLeftPage);
        GoToBack(_hiddenRightPage);
    }

    private void DisplayNecessaryControls()
    {
        _leftControlImage.gameObject.SetActive(CurrentPageNumber > 0);
        _rightControlImage.gameObject.SetActive(CurrentPageNumber < _maxPageNumber - 1);
    }

    private void UpdateControlsSprite()
    {
        _leftControlImage.sprite = UIKeyMapper.Instance.GetSprite(_switchLeftAction.GetBindingDisplayString());
        _rightControlImage.sprite = UIKeyMapper.Instance.GetSprite(_switchRightAction.GetBindingDisplayString());
    }

    private void GoToPage(int pageNumber, bool alternatePages = false)
    {
        PlantScriptableObject data = GetDataAtPage(pageNumber);
        if (alternatePages)
        {
            int currentPage = _currentLeftPage.PageNumber / 2;
            _hiddenLeftPage.Plant = data;
            _hiddenRightPage.Plant = data;

            _hiddenLeftPage.PageNumber = pageNumber * 2;
            _hiddenRightPage.PageNumber = pageNumber * 2 + 1;

            StartCoroutine(AnimatePageChange(currentPage < pageNumber));
        }
        else
        {
            _currentLeftPage.Plant = data;
            _currentRightPage.Plant = data;

            _currentLeftPage.PageNumber = pageNumber * 2;
            _currentRightPage.PageNumber = pageNumber * 2 + 1;
        }
        DisplayNecessaryControls();
    }

    private IEnumerator AnimatePageChange(bool readingDirection)
    {
        IsAnimating = true;
        //TODO: propably possible to refactor
        if (readingDirection)
        {
            yield return StartCoroutine(ClosePage(_hiddenLeftPage));
            yield return StartCoroutine(ClosePage(_currentRightPage, true));
            GoToBack(_currentLeftPage);
            GoToBack(_currentRightPage);
            GoToFront(_hiddenLeftPage);
            GoToFront(_hiddenRightPage);
            yield return StartCoroutine(OpenPage(_hiddenLeftPage, true));
            yield return StartCoroutine(OpenPage(_currentRightPage));
        }
        else
        {
            yield return StartCoroutine(ClosePage(_hiddenRightPage));
            yield return StartCoroutine(ClosePage(_currentLeftPage, true));
            GoToBack(_currentRightPage);
            GoToBack(_currentLeftPage);
            GoToFront(_hiddenRightPage);
            GoToFront(_hiddenLeftPage);
            yield return StartCoroutine(OpenPage(_hiddenRightPage, true));
            yield return StartCoroutine(OpenPage(_currentLeftPage));
        }
        SwapPages();
        IsAnimating = false;
    }

    private IEnumerator ClosePage(BookPageUI page, bool animate = false)
    {
        Vector3 originScale = page.transform.localScale;
        Vector3 targetScale = new Vector3(0.0f, 1.0f, 1.0f);
        if (!animate)
        {
            page.transform.localScale = targetScale;
        }
        else
        {
            yield return StartCoroutine(AnimateScaleChange(page, originScale, targetScale, _pageTurnTime, _pageClosingAnimationCurve));
        }
    }

    private IEnumerator OpenPage(BookPageUI page, bool animate = false)
    {
        Vector3 originScale = page.transform.localScale;
        Vector3 targetScale = new Vector3(1.0f, 1.0f, 1.0f);
        if (!animate)
        {
            page.transform.localScale = targetScale;
        }
        else
        {
            yield return StartCoroutine(AnimateScaleChange(page, originScale, targetScale, _pageTurnTime, _pageOpeningAnimationCurve));
        }
    }

    private IEnumerator AnimateScaleChange(BookPageUI page, Vector3 originScale, Vector3 targetScale, float timeSeconds, AnimationCurve curve)
    {
        float currentTime = 0.0f;
        while (page.transform.localScale != targetScale)
        {
            currentTime += Time.deltaTime;
            float progress = currentTime / timeSeconds;
            float curveValue = curve.Evaluate(progress);
            page.transform.localScale = Vector3.Lerp(originScale, targetScale, curveValue);
            yield return null;
        }
    }

    private void SwapPages()
    {
        _currentLeftPage = _currentLeftPage == _leftPageA ? _leftPageB : _leftPageA;
        _hiddenLeftPage = _hiddenLeftPage == _leftPageA ? _leftPageB : _leftPageA;

        _currentRightPage = _currentRightPage == _rightPageA ? _rightPageB : _rightPageA;
        _hiddenRightPage = _hiddenRightPage == _rightPageA ? _rightPageB : _rightPageA;
    }

    private bool IsPageNumberCorrect(int pageNumber)
    {
        return pageNumber >= 0 && pageNumber < _maxPageNumber;
    }

    private PlantScriptableObject GetDataAtPage(int page)
    {
        if (IsPageNumberCorrect(page))
        {
            PlantScriptableObject node = _database.plants[page];
            return node == null ? null : node;
        }

        return null;
    }

    private void GoToFront(BookPageUI page)
    {
        page.SetSortingOrder(2);
        page.ActivateFocusableElements(true);
    }

    private void GoToBack(BookPageUI page)
    {
        page.SetSortingOrder(1);
        page.ActivateFocusableElements(false);
    }

    public void OpenIdentificationTable(PlantComponentScriptableObject component)
    {
        IdentificationTable.gameObject.SetActive(true);
        IdentificationTable.bypassSelector = true;
        IdentificationTable.OnComponentSelected(component);
    }

    #region Input management
    private void BindInputs()
    {
        _actions = GameManager.Instance.Actions;

        _switchLeftAction = _actions.FindAction(SWITCH_TAB_LEFT);
        _switchRightAction = _actions.FindAction(SWITCH_TAB_RIGHT);

        _switchLeftAction.performed += OnSwitchLeftButtonPressed;
        _switchRightAction.performed += OnSwitchRightButtonPressed;

        _switchLeftAction.Enable();
        _switchRightAction.Enable();
        Debug.Log(_switchLeftAction.GetBindingDisplayString());
        Debug.Log(_switchRightAction.GetBindingDisplayString());
    }

    private void UnBindInputs()
    {
        _switchLeftAction.performed -= OnSwitchLeftButtonPressed;
        _switchRightAction.performed -= OnSwitchRightButtonPressed;
    }

    private void OnSwitchLeftButtonPressed(InputAction.CallbackContext ctx)
    {
        if (CurrentPageNumber > 0 && !IsAnimating)
        {
            CurrentPageNumber--;
        }
    }

    private void OnSwitchRightButtonPressed(InputAction.CallbackContext ctx)
    {
        if (CurrentPageNumber < _maxPageNumber - 1 && !IsAnimating)
        {
            CurrentPageNumber++;
        }
    }
    #endregion

    #region BasePopup implementation
    protected override void CustomOnDisable()
    {
        UnBindInputs();
    }

    protected override void CustomOnEnable()
    {
        Debug.Log("custom onenable");
        BindInputs();
        UpdateControlsSprite();
        _currentLeftPage.ActivateFocusableElements(true);
        _currentLeftPage.UpdateUI();
        _currentRightPage.UpdateUI();
    }

    protected override void OnPopupClosing()
    {
    }
    #endregion
}

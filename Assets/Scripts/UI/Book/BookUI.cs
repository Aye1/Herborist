using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using System;
using UnityEngine.InputSystem;

public class BookUI : BasePopup
{
    [Title("Assets binding")]
    [SerializeField, Required, AssetsOnly] private BookDatabaseScriptableObject _database;
    [SerializeField, Required, AssetsOnly] private BookPlateUI _plateTemplate;
    [SerializeField, Required, AssetsOnly] private BookPageUI _rightPageTemplate;

    [Title("Child objects binding")]
    [SerializeField, Required, ChildGameObjectsOnly] private Transform _leftPageHolder;
    [SerializeField, Required, ChildGameObjectsOnly] private Transform _rightPageHolder;

    private int _currentPageNumber = 0;
    public int CurrentPageNumber
    {
        get { return _currentPageNumber; }
        set
        {
            if(_currentPageNumber != value && IsPageNumberCorrect(value))
            {
                _currentPageNumber = value;
                GoToPage(_currentPageNumber);
            }
        }
    }

    private int _maxPageNumber;

    // Simplified pool system, with two objects for each part left/right
    private BookPlateUI _leftPageA;
    private BookPlateUI _leftPageB;
    private BookPageUI _rightPageA;
    private BookPageUI _rightPageB;

    private BookPlateUI _currentLeftPage;
    private BookPageUI _currentRightPage;

    private InputActionAsset _actions;

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
        _rightPageA = Instantiate(_rightPageTemplate, _rightPageHolder);
        _rightPageB = Instantiate(_rightPageTemplate, _rightPageHolder);
        _currentLeftPage = _leftPageA;
        _currentRightPage = _rightPageA;
    }

    private void DisplayOnlyNecessaryPages()
    {
        Display(_currentLeftPage, true);
        Display(_currentRightPage, true);
        Display(GetHiddenLeftPage(), false);
        Display(GetHiddenRightPage(), false);
    }

    private void GoToPage(int pageNumber)
    {
        IdentificationKeyData data = GetDataAtPage(pageNumber);
        _currentLeftPage.Plant = data;
        _currentRightPage.Plant = data;

        _currentLeftPage.PageNumber = pageNumber * 2;
        _currentRightPage.PageNumber = pageNumber * 2 + 1;
    }

    private bool IsPageNumberCorrect(int pageNumber)
    {
        return pageNumber >= 0 && pageNumber < _maxPageNumber;
    }

    private IdentificationKeyData GetDataAtPage(int page)
    {
        if(IsPageNumberCorrect(page))
        {
            IdentificationKeyNode node = _database.plants[page];
            return node == null ? null : node.identificationData;
        }

        return null;
    }

    private void Display(BookPageUI page, bool display)
    {
        page.gameObject.SetActive(display);
    }

    private BookPageUI GetHiddenLeftPage()
    {
        return _currentLeftPage == _leftPageA ? _leftPageB : _leftPageA;

    }

    private BookPageUI GetHiddenRightPage()
    {
        return _currentRightPage == _rightPageA ? _rightPageB : _rightPageA;
    }

    #region Input management
    private void BindInputs()
    {
        _actions = GameManager.Instance.Actions;

        InputAction switchLeftAction = _actions.FindAction(SWITCH_TAB_LEFT);
        InputAction switchRightAction = _actions.FindAction(SWITCH_TAB_RIGHT);

        switchLeftAction.performed += OnSwitchLeftButtonPressed;
        switchRightAction.performed += OnSwitchRightButtonPressed;

        switchLeftAction.Enable();
        switchRightAction.Enable();
    }

    private void UnBindInputs()
    {
        InputAction switchLeftAction = _actions.FindAction(SWITCH_TAB_LEFT);
        InputAction switchRightAction = _actions.FindAction(SWITCH_TAB_RIGHT);

        switchLeftAction.performed -= OnSwitchLeftButtonPressed;
        switchRightAction.performed -= OnSwitchRightButtonPressed;
    }

    private void OnSwitchLeftButtonPressed(InputAction.CallbackContext ctx)
    {
        Debug.Log("left switch");
        if(CurrentPageNumber > 0)
        {
            CurrentPageNumber--;
        }
    }

    private void OnSwitchRightButtonPressed(InputAction.CallbackContext ctx)
    {
        Debug.Log("right switch");
        if(CurrentPageNumber < _maxPageNumber - 1)
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
        BindInputs();
    }

    protected override GameObject GetObjectToDeactivate()
    {
        return gameObject;
    }

    protected override void OnPopupClosing()
    {
    }
    #endregion
}

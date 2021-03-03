using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

public class NavigationManager : MonoBehaviour
{
    private readonly string CANCEL_ACTION = "Custom UI/Cancel";

    public static NavigationManager Instance { get; private set; }

    [ShowInInspector] private Stack<INavigable> _navigationStack;
    [SerializeField] private GameObject _currentFocus; // Serialized just to be correctly updated in the editor in realtime

    public bool IsPopupOpen
    {
        get { return _navigationStack.Count > 0; }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _navigationStack = new Stack<INavigable>();
            BindControls();
            BindEvents();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        ManageFocus();
    }

    void BindControls()
    {
        InputAction cancelAction = GameManager.Instance.Actions.FindAction(CANCEL_ACTION);

        cancelAction.performed += OnCancelInput;

        cancelAction.Enable();
    }

    void BindEvents()
    {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    void OnCancelInput(InputAction.CallbackContext ctx)
    {
        PopNavigation();
    }

    bool IsCurrentNavigableRemovable()
    {
        if(_navigationStack.Count == 0)
        {
            return false;
        }
        INavigable navigable = _navigationStack.Peek();
        return navigable.IsRemovable();
    }

    public void SetFocus(GameObject focusObj)
    {
        EventSystem.current.SetSelectedGameObject(focusObj);

        ISelectHandler[] selects = focusObj.GetComponents<ISelectHandler>();
        if(selects != null)
        {
            foreach(ISelectHandler select in selects)
            {
                select.OnSelect(null);
            }
        }
    }

    public void PushNavigation(INavigable navigable)
    {
        _navigationStack.Push(navigable);
        navigable.OnNavigate();
    }

    public INavigable PopNavigation()
    {
        if (_navigationStack.Count > 0 && IsCurrentNavigableRemovable())
        {
            INavigable toPop = _navigationStack.Pop();
            toPop.OnCancel();
            UpdateCurrentNavigable();
            return toPop;
        }
        return null;
    }

    private void UpdateCurrentNavigable()
    {
        if(_navigationStack.Count > 0)
        {
            INavigable currentNavigable = _navigationStack.Peek();
            currentNavigable.OnComingBack();
        }
    }

    private void OnGameStateChanged(GameState newState)
    {
        _navigationStack.Clear();
    }

    private void ManageFocus()
    {
        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;
        if(currentSelected == null && _currentFocus != null)
        {
            SetFocus(_currentFocus);
        } else
        {
            _currentFocus = currentSelected;
        }
    }
}

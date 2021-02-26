﻿using System.Collections.Generic;
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
        _currentFocus = EventSystem.current.currentSelectedGameObject;
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
        Debug.Log("OnCancel called");
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
    }

    public void PushNavigation(INavigable navigable)
    {
        Debug.Log("Pushing " + navigable.ToString());
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
            Debug.Log("Popping " + toPop.ToString());
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
}
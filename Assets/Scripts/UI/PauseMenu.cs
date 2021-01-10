using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField, Required, AssetsOnly] InputActionAsset _inputs;

    [SerializeField, Required, ChildGameObjectsOnly] GameObject _pauseMenu;
    [SerializeField, Required, ChildGameObjectsOnly] Button _resumeButton;
    [SerializeField, Required, ChildGameObjectsOnly] Button _saveButton;
    [SerializeField, Required, ChildGameObjectsOnly] Button _loadButton;

    private readonly string PAUSE_ACTION = "Custom UI/Pause Menu";
    private readonly string CANCEL_ACTION = "Custom UI/Cancel";


    private bool _isMenuOpen = false;
    public bool IsMenuOpen
    {
        get
        {
            return _isMenuOpen;
        }
        private set
        {
            if(_isMenuOpen != value)
            {
                _isMenuOpen = value;
                UpdateMenuVisibility();
            }
        }
    }

    void Awake()
    {
        BindControls();
        BindButtons();
    }

    private void OnDestroy()
    {
        UnBindControls();
    }

    void BindControls()
    {
        InputAction pauseAction = _inputs.FindAction(PAUSE_ACTION);
        InputAction cancelAction = _inputs.FindAction(CANCEL_ACTION);

        pauseAction.performed += OnPauseMenu;
        cancelAction.performed += OnCancel;

        pauseAction.Enable();
        cancelAction.Enable();
    }

    void UnBindControls()
    {
        InputAction pauseAction = _inputs.FindAction(PAUSE_ACTION);
        InputAction cancelAction = _inputs.FindAction(CANCEL_ACTION);

        pauseAction.performed -= OnPauseMenu;
        cancelAction.performed -= OnCancel;
    }

    void BindButtons()
    {
        _resumeButton.onClick.AddListener(ClosePauseMenu);
        _saveButton.onClick.AddListener(LaunchSave);
        _loadButton.onClick.AddListener(LaunchLoad);
    }

    void LaunchSave()
    {
        SaveManager.Instance.SaveGame();
    }

    void LaunchLoad()
    {
        SaveManager.Instance.LoadGame();
    }

    void TogglePause()
    {
        IsMenuOpen = !IsMenuOpen;
    }

    void ClosePauseMenu()
    {
        IsMenuOpen = false;
    }

    #region Input Callbacks
    void OnPauseMenu(InputAction.CallbackContext ctx)
    {
        TogglePause();
    }

    void OnCancel(InputAction.CallbackContext ctx)
    {
        ClosePauseMenu();
    }
    #endregion

    void UpdateMenuVisibility()
    {
        _pauseMenu.SetActive(_isMenuOpen);
    }
}

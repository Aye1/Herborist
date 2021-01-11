using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    [SerializeField, Required, AssetsOnly] InputActionAsset _inputs;

    [SerializeField, Required, ChildGameObjectsOnly] GameObject _pauseMenu;
    [SerializeField, Required, ChildGameObjectsOnly] Button _resumeButton;
    [SerializeField, Required, ChildGameObjectsOnly] Button _saveButton;
    [SerializeField, Required, ChildGameObjectsOnly] Button _loadButton;

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
                if (_isMenuOpen)
                    SetUINavigationFirstElement();
            }
        }
    }

    void Awake()
    {
        BindControls();
        BindButtons();
        BindEvents();
        IsMenuOpen = GameManager.Instance.IsInPause;
        UpdateMenuVisibility();
    }

    private void OnDestroy()
    {
        UnBindEvents();
        UnBindControls();
    }

    void BindControls()
    {
        InputAction cancelAction = _inputs.FindAction(CANCEL_ACTION);

        cancelAction.performed += OnCancel;

        cancelAction.Enable();
    }

    void UnBindControls()
    {
        InputAction cancelAction = _inputs.FindAction(CANCEL_ACTION);

        cancelAction.performed -= OnCancel;
    }

    void BindButtons()
    {
        _resumeButton.onClick.AddListener(ClosePauseMenu);
        _saveButton.onClick.AddListener(LaunchSave);
        _loadButton.onClick.AddListener(LaunchLoad);
    }

    void BindEvents()
    {
        GameManager.Instance.OnPauseStateChanged += TogglePause;
    }

    void UnBindEvents()
    {
        GameManager.Instance.OnPauseStateChanged -= TogglePause;
    }

    void SetUINavigationFirstElement()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_resumeButton.gameObject);
    }

    void LaunchSave()
    {
        SaveManager.Instance.SaveGame();
    }

    void LaunchLoad()
    {
        SaveManager.Instance.LoadGame();
    }

    void TogglePause(bool pauseState)
    {
        IsMenuOpen = pauseState;
    }

    void ClosePauseMenu()
    {
        GameManager.Instance.SetPause(false);
    }

    #region Input Callbacks

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

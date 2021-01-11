using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class PauseMenu : BasePopup
{
    [SerializeField, Required, ChildGameObjectsOnly] GameObject _pauseMenu;
    [SerializeField, Required, ChildGameObjectsOnly] Button _resumeButton;
    [SerializeField, Required, ChildGameObjectsOnly] Button _saveButton;
    [SerializeField, Required, ChildGameObjectsOnly] Button _loadButton;

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
        BindButtons();
        BindEvents();
        IsMenuOpen = GameManager.Instance.IsInPause;
        UpdateMenuVisibility();
    }

    private void OnDestroy()
    {
        UnBindEvents();
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

    void UpdateMenuVisibility()
    {
        _pauseMenu.SetActive(_isMenuOpen);
    }

    #region BasePopup implementation
    protected override GameObject GetObjectToDeactivate()
    {
        return _pauseMenu;
    }

    protected override void OnPopupClosing()
    {
        ClosePauseMenu();
    }
    #endregion
}
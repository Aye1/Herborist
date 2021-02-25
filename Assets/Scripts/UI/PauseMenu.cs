using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class PauseMenu : BasePopup
{
    [SerializeField, Required, ChildGameObjectsOnly] Button _resumeButton;
    [SerializeField, Required, ChildGameObjectsOnly] Button _saveButton;
    [SerializeField, Required, ChildGameObjectsOnly] Button _loadButton;
    [SerializeField, Required, ChildGameObjectsOnly] Button _mainMenuButton;

    void Awake()
    {
        BindButtons();
    }

    void BindButtons()
    {
        _resumeButton.onClick.AddListener(ClosePauseMenu);
        _saveButton.onClick.AddListener(LaunchSave);
        _loadButton.onClick.AddListener(LaunchLoad);
        _mainMenuButton.onClick.AddListener(GoToMainMenu);
    }

    void LaunchSave()
    {
        SaveManager.Instance.SaveGame();
    }

    void LaunchLoad()
    {
        SaveManager.Instance.LoadGame();
    }

    void ClosePauseMenu()
    {
        GameManager.Instance.SetPause(false);
    }

    void GoToMainMenu()
    {
        SaveManager.Instance.SaveGame();
        GameManager.Instance.GoToMainMenu();
    }

    #region BasePopup implementation

    protected override void OnPopupClosing()
    {
        ClosePauseMenu();
    }

    protected override void CustomOnEnable()
    {
        return;
    }

    protected override void CustomOnDisable()
    {
        return;
    }
    #endregion
}
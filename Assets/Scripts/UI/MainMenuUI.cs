using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public enum MenuState { Main, SaveSelection, Parameters }

public class MainMenuUI : MonoBehaviour, INavigable
{
    [SerializeField, Required, ChildGameObjectsOnly] private GameObject _buttonsContainer;
    [SerializeField, Required, ChildGameObjectsOnly] private Button _playButton;
    [SerializeField, Required, ChildGameObjectsOnly] private Button _quitButton;
    [SerializeField, Required, ChildGameObjectsOnly] private SaveSelectorUI _saveSelectionPopup;

    private MenuState _currentMenuState = MenuState.Main;

    void Awake()
    {
        NavigationManager.Instance.PushNavigation(this);
        BindButtons();
    }

    void BindButtons()
    {
        _playButton.onClick.AddListener(DisplaySaveSelectionPopup);
        _quitButton.onClick.AddListener(Quit);
    }

    private void DisplaySaveSelectionPopup()
    {
        _buttonsContainer.gameObject.SetActive(false);
        _saveSelectionPopup.gameObject.SetActive(true);
        _currentMenuState = MenuState.SaveSelection;
    }

    private void HideSaveSelectionPopup()
    {
        _buttonsContainer.gameObject.SetActive(true);
        _saveSelectionPopup.gameObject.SetActive(false);
        _currentMenuState = MenuState.Main;
    }

    /*private void OnCancelInput(InputAction.CallbackContext ctx)
    {
        if(_currentMenuState == MenuState.SaveSelection)
        {
            HideSaveSelectionPopup();
        }
    }*/

    private void Quit()
    {
        Application.Quit();
    }

    #region INavigable implementation
    public void OnCancel()
    {
        return;
    }

    public void OnComingBack()
    {
        HideSaveSelectionPopup();
    }

    public void OnNavigate()
    {
        HideSaveSelectionPopup();
    }

    public bool IsRemovable()
    {
        return false;
    }
    #endregion
}

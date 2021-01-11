using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public enum MenuState { Main, SaveSelection, Parameters }

public class MainMenuUI : MonoBehaviour
{
    [SerializeField, Required, ChildGameObjectsOnly] private GameObject _buttonsContainer;
    [SerializeField, Required, ChildGameObjectsOnly] private Button _playButton;
    [SerializeField, Required, ChildGameObjectsOnly] private Button _quitButton;
    [SerializeField, Required, ChildGameObjectsOnly] private SaveSelectorUI _saveSelectionPopup;

    private MenuState _currentMenuState = MenuState.Main;

    void Awake()
    {
        BindButtons();
        BindInputs();
    }

    void BindButtons()
    {
        _playButton.onClick.AddListener(DisplaySaveSelectionPopup);
        _quitButton.onClick.AddListener(Quit);
    }

    void BindInputs()
    {
        //TODO: refacto
        InputAction cancelAction = GameManager.Instance.Actions.FindAction("Custom UI/Cancel");

        cancelAction.performed += OnCancelInput;

        cancelAction.Enable();
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

    private void OnCancelInput(InputAction.CallbackContext ctx)
    {
        if(_currentMenuState == MenuState.SaveSelection)
        {
            HideSaveSelectionPopup();
        }
    }

    private void Quit()
    {
        Application.Quit();
    }
}

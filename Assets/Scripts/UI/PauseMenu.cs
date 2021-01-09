using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance { get; private set; }
    [SerializeField, Required, ChildGameObjectsOnly] GameObject _pauseMenu;
    [SerializeField, Required, ChildGameObjectsOnly] Button _resumeButton;


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
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            BindButtons();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void BindButtons()
    {
        _resumeButton.onClick.AddListener(OnCancel);
    }

    #region Input Messages
    void OnPauseMenu()
    {
        IsMenuOpen = !IsMenuOpen;
    }

    void OnCancel()
    {
        IsMenuOpen = false;
    }
    #endregion

    void UpdateMenuVisibility()
    {
        _pauseMenu.SetActive(_isMenuOpen);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine.InputSystem;

public enum GameState { Unknown, MainMenu, Game };

public class GameManager : SerializedMonoBehaviour
{
    [SerializeField, Required, AssetsOnly] private InputActionAsset _actions;


    private GameState _currentState;
    [SerializeField, ReadOnly]
    private bool _isInPause;

    private readonly string PAUSE_ACTION = "Custom UI/Pause Menu";


    public delegate void GameStateChange(GameState gameState);
    public GameStateChange OnGameStateChanged;

    public delegate void PauseStateChange(bool status);
    public PauseStateChange OnPauseStateChanged;

    public static GameManager Instance { get; private set; }

    public GameState CurrentState
    {
        get { return _currentState; }
        set
        {
            if(_currentState != value)
            {
                _currentState = value;
                OnGameStateChanged?.Invoke(_currentState);
            }
        }
    }

    public bool IsInPause
    {
        get { return _isInPause; }
        private set
        {
            if(value != _isInPause)
            {
                _isInPause = value;
                OnPauseStateChanged?.Invoke(_isInPause);
            }
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            BindInputs();
            CurrentState = GameState.Game;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        UnBindInputs();
    }

    void BindInputs()
    {
        InputAction pauseAction = _actions.FindAction(PAUSE_ACTION);

        pauseAction.performed += OnPauseButtonPushed;

        pauseAction.Enable();
    }

    void UnBindInputs()
    {
        InputAction pauseAction = _actions.FindAction(PAUSE_ACTION);

        pauseAction.performed -= OnPauseButtonPushed;
    }

    void OnPauseButtonPushed(InputAction.CallbackContext ctx)
    {
        if(CurrentState == GameState.Game)
            TogglePause();
    }

    public void SetPause(bool pause)
    {
        IsInPause = pause;
    }

    public void TogglePause()
    {
        IsInPause = !IsInPause;
    }
}

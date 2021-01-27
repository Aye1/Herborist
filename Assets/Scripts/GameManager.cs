using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;

public enum GameState { Unknown, MainMenu, Game };

public class GameManager : SerializedMonoBehaviour
{
    [SerializeField, Required, AssetsOnly] private InputActionAsset _actions;

    private GameState _currentState;
    [SerializeField, ReadOnly]
    private bool _isInPause;
    private bool _shouldLoadGameSave;
    private bool _isCountingTime;
    private float _gameTimer; // Time played, in seconds

    private readonly string PAUSE_ACTION = "Custom UI/Pause Menu";

    public GameInfo gameInfo;

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

    public InputActionAsset Actions
    {
        get { return _actions; }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CurrentState = GameState.Game;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        BindInputs();
        BindEvents();        
    }

    private void OnDestroy()
    {
        UnBindInputs();
        UnBindEvents();
    }

    private void Update()
    {
        if(_isCountingTime)
        {
            _gameTimer += Time.deltaTime;
            gameInfo.AddGameTime(Time.deltaTime); // Time spent is always updated, so that we are sur to save it properly
        }
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

    void BindEvents()
    {
        SceneSwitcher.Instance.OnSceneLoaded += OnSceneLoaded;
    }

    void UnBindEvents()
    {
        SceneSwitcher.Instance.OnSceneLoaded -= OnSceneLoaded;
    }

    void OnPauseButtonPushed(InputAction.CallbackContext ctx)
    {
        if(CurrentState == GameState.Game)
            TogglePause();
    }

    void OnSceneLoaded(SceneType type)
    {
        if(_shouldLoadGameSave)
        {
            _shouldLoadGameSave = false;
            SaveManager.Instance.LoadGame();
        }
    }

    public void SetPause(bool pause)
    {
        IsInPause = pause;
    }

    public void TogglePause()
    {
        IsInPause = !IsInPause;
    }

    public void LaunchGame(GameInfo info)
    {
        gameInfo = info;
        gameInfo.isGameStarted = true;
        CurrentState = GameState.Game;
        _shouldLoadGameSave = true;
        SceneSwitcher.Instance.GoToScene(SceneType.House, forceLoadingScreen:true);
        LaunchGameTimer();
    }

    public void GoToMainMenu()
    {
        IsInPause = false;
        CurrentState = GameState.MainMenu;
        SceneSwitcher.Instance.GoToScene(SceneType.MainMenu);
        StopGameTimer();
    }

    private void LaunchGameTimer()
    {
        _isCountingTime = true;
    }

    private void StopGameTimer()
    {
        _isCountingTime = false;
    }
}

using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;

public enum GameState { Unknown, MainMenu, Game };

public class GameManager : SerializedMonoBehaviour
{
    [Title("Editor bindings - Assets")]
    [SerializeField, Required, AssetsOnly] private InputActionAsset _actions;
    [SerializeField, Required, AssetsOnly] private PauseMenu _pauseMenuTemplate;
    [SerializeField, Required, AssetsOnly] private InventoryUI _openInventoryUITemplate;

    private GameState _currentState;
    [SerializeField, ReadOnly]
    private bool _isInPause;
    private bool _shouldLoadGameSave;
    private bool _isCountingTime;
    private float _gameTimer; // Time played, in seconds
    private PauseMenu _pauseMenu;
    private InventoryUI _openInventory;

    private readonly string PAUSE_ACTION = "Custom UI/Pause Menu";
    private readonly string OPEN_INVENTORY_ACTION = "Player/OpenInventory";

    [Title("Public variables")]
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
            if (_currentState != value)
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
            if (value != _isInPause)
            {
                _isInPause = value;
                OnPauseStateChanged?.Invoke(_isInPause);
                PauseMenuWindow.gameObject.SetActive(_isInPause);
                if (!_isInPause)
                {
                    NavigationManager.Instance.PopNavigation();
                }
            }
        }
    }

    private PauseMenu PauseMenuWindow
    {
        get
        {
            if (_pauseMenu == null)
            {
                _pauseMenu = Instantiate(_pauseMenuTemplate);
                _pauseMenu.transform.localPosition = Vector3.zero;
            }
            return _pauseMenu;
        }
    }

    private InventoryUI OpenInventoryWindow
    {
        get
        {
            if (_openInventory == null)
            {
                _openInventory = Instantiate(_openInventoryUITemplate);
                _openInventory.transform.localPosition = Vector3.zero;
                _openInventory.isPlayerInventory = true;
            }
            return _openInventory;
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
            CurrentState = GameState.MainMenu;
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
        if (_isCountingTime)
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

        InputAction openInventoryAction = _actions.FindAction(OPEN_INVENTORY_ACTION);
        openInventoryAction.performed += OnOpenInventoryButtonPushed;
        openInventoryAction.Enable();
    }

    void UnBindInputs()
    {
        InputAction pauseAction = _actions.FindAction(PAUSE_ACTION);
        pauseAction.performed -= OnPauseButtonPushed;

        InputAction openInventoryAction = _actions.FindAction(OPEN_INVENTORY_ACTION);
        openInventoryAction.performed -= OnOpenInventoryButtonPushed;
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
        if (CurrentState == GameState.Game)
            TogglePause();
    }

    void OnOpenInventoryButtonPushed(InputAction.CallbackContext ctx)
    {
        if (CurrentState == GameState.Game)
        {
            OpenInventoryWindow.gameObject.SetActive(true);
        }

    }

    void OnSceneLoaded(SceneType type)
    {
        if (_shouldLoadGameSave)
        {
            _shouldLoadGameSave = false;
            SaveManager.Instance.LoadGame();
        }
    }

    public void SetPause(bool pause)
    {
        IsInPause = pause;
        //PauseMenuWindow.gameObject.SetActive(pause);
    }

    public void TogglePause()
    {
        IsInPause = !IsInPause;
        //PauseMenuWindow.gameObject.SetActive(IsInPause);
    }

    public void LaunchGame(GameInfo info)
    {
        gameInfo = info;
        gameInfo.isGameStarted = true;
        CurrentState = GameState.Game;
        _shouldLoadGameSave = true;
        SceneSwitcher.Instance.GoToScene(SceneType.House, forceLoadingScreen: true);
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

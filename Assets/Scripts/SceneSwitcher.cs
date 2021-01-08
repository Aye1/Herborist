using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.Linq;

public enum SceneType { Unknown, MainMenu, Garden, House, Forest}

[Serializable]
public class SceneBinding
{
    public SceneType sceneType;
    public int sceneId;
    public bool shouldDisplayLoadingScreen;
}

public class SceneSwitcher : SerializedMonoBehaviour
{
    public static SceneSwitcher Instance { get; private set; }

    [SerializeField]
    private List<SceneBinding> _bindings;

    [SerializeField, Required, ChildGameObjectsOnly]
    private GameObject _loadingScreen;

    public bool IsSceneLoading
    {
        get; private set;
    }

    public int PreviousSceneId
    {
        get; private set;
    }

    public SceneType PreviousScene
    {
        get { return GetSceneWithId(PreviousSceneId); }
    }

    public int CurrentSceneId
    {
        get; private set;
    }

    public SceneType CurrentScene
    {
        get { return GetSceneWithId(CurrentSceneId); }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            PreviousSceneId = -1;
            CurrentSceneId = SceneManager.GetActiveScene().buildIndex;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GoToScene(SceneType scene)
    {
        int sceneId = GetSceneId(scene);
        if (sceneId != -1)
        {
            StartCoroutine(LoadSceneAsync(sceneId));
            PreviousSceneId = CurrentSceneId;
            CurrentSceneId = sceneId;
        }
    }

    private IEnumerator LoadSceneAsync(int sceneId)
    {
        IsSceneLoading = true;
        bool displayLoadingScreen = ShouldDisplayLoadingScreen(sceneId);
        DisplayLoadingScreen(displayLoadingScreen);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);
        while(!operation.isDone)
        {
            yield return null;
        }
        IsSceneLoading = false;

        DisplayLoadingScreen(false);
    }

    public int GetSceneId(SceneType scene)
    {
        SceneBinding binding = _bindings.Where(b => b.sceneType == scene).FirstOrDefault();
        return binding == default(SceneBinding) ? -1 : binding.sceneId;
    }

    public SceneType GetSceneWithId(int id)
    {
        SceneBinding binding = _bindings.Where(b => b.sceneId == id).FirstOrDefault();
        return binding == default(SceneBinding) ? SceneType.Unknown : binding.sceneType;
    }

    public bool ShouldDisplayLoadingScreen(SceneType scene)
    {
        SceneBinding binding = _bindings.Where(b => b.sceneType == scene).FirstOrDefault();
        return binding == default(SceneBinding) ? false : binding.shouldDisplayLoadingScreen;
    }

    public bool ShouldDisplayLoadingScreen(int sceneId)
    {
        SceneBinding binding = _bindings.Where(b => b.sceneId == sceneId).FirstOrDefault();
        return binding == default(SceneBinding) ? false : binding.shouldDisplayLoadingScreen;
    }

    private void DisplayLoadingScreen(bool visibility)
    {
        _loadingScreen.gameObject.SetActive(visibility);
    }
}

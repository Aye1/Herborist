using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.Linq;

public enum Scene { Unknown, MainMenu, Garden, House, Forest}

public class SceneSwitcher : SerializedMonoBehaviour
{
    public static SceneSwitcher Instance { get; private set; }

    [SerializeField, DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.ExpandedFoldout, KeyLabel = "Scene", ValueLabel = "Scene Id")]
    private Dictionary<Scene, int> _sceneBindings;

    public int PreviousSceneId
    {
        get; private set;
    }

    public Scene PreviousScene
    {
        get { return GetSceneWithId(PreviousSceneId); }
    }

    public int CurrentSceneId
    {
        get; private set;
    }

    public Scene CurrentScene
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

    public void GoToScene(Scene scene)
    {
        int sceneId = GetSceneId(scene);
        if (sceneId != -1)
        {
            SceneManager.LoadScene(sceneId);
            PreviousSceneId = CurrentSceneId;
            CurrentSceneId = sceneId;
        }
    }

    public int GetSceneId(Scene scene)
    {
        return _sceneBindings.ContainsKey(scene) ? _sceneBindings[scene] : -1;
    }

    public Scene GetSceneWithId(int id)
    {
        if(_sceneBindings.ContainsValue(id))
        {
            return _sceneBindings.Where(b => b.Value == id).First().Key;
        }
        return Scene.Unknown;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;

public class PlantIdentificationState : SaveState
{
    public string[] componentName;
    public bool[] isIdentified;
}

public class PlantIdentificationInfos : SerializedMonoBehaviour, ISavable
{
    [DictionaryDrawerSettings(KeyLabel = "Plant Component", ValueLabel = "Is Identified")]
    public Dictionary<PlantComponentScriptableObject, bool> identificationData;

    public static PlantIdentificationInfos Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SaveManager.Instance.objectsToSave.Add(this);
            BindEvents();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        UnBindEvents();
    }

    void BindEvents()
    {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    void UnBindEvents()
    {
        GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }

    void OnGameStateChanged(GameState newState)
    {
        if (newState == GameState.MainMenu)
        {
            SaveManager.Instance.objectsToSave.Remove(this);
            Destroy(gameObject);
        }
    }

    #region ISavable implementation
    public SaveState GetObjectToSave()
    {
        PlantIdentificationState state = new PlantIdentificationState()
        {
            componentName = identificationData.Keys.Select(k => k.name).ToArray(),
            isIdentified = identificationData.Values.ToArray()
        };
        return state;
    }

    public string GetSaveName()
    {
        return gameObject.name + ".identificationData";
    }

    public void LoadObject(SaveState saveState)
    {
        PlantIdentificationState state = saveState as PlantIdentificationState;

        // We don't totally clear the dictionary, in case we added some new data not present in the save
        for(int i=0; i<state.componentName.Length; i++)
        {
            string name = state.componentName[i];

            // Potential versioning issues here
            PlantComponentScriptableObject component = identificationData.Keys.Where(k => k.name == name).First();
            if(component != null)
            {
                identificationData.Remove(component);
            }
            identificationData.Add(component, state.isIdentified[i]);
        }
    }
    #endregion
}

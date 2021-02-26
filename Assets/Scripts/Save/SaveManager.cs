using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using System.IO;
using Sirenix.OdinInspector;

public class SaveManager : SerializedMonoBehaviour
{
    private readonly string SAVE_EXTENSION = ".save";

    public List<ISavable> objectsToSave;

    [ListDrawerSettings(HideAddButton = true, CustomRemoveElementFunction = "DeleteSave", DraggableItems = false)]
    [ShowInInspector]
    private List<string> _existingSaves;

    public static SaveManager Instance { get; private set; }
    public bool IsSaving { get; private set; }
    public bool IsLoading { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveGame()
    {
        foreach(ISavable toSave in objectsToSave)
        {
            Save(toSave);
        }
    }

    public void Save(ISavable toSave)
    {
        if (toSave != null)
        {
            IsSaving = true;
            SaveState state = toSave.GetObjectToSave();
            byte[] bytes = SerializationUtility.SerializeValue(state, DataFormat.Binary);
            string path = GetSavePath(toSave.GetSaveName());
            File.WriteAllBytes(path, bytes);
            IsSaving = false;
            Debug.Log("Saved " + path);
        }
    }

    public void LoadGame()
    {
        foreach(ISavable toLoad in objectsToSave)
        {
            Load(toLoad);
        }
    }

    public void Load(ISavable toLoad)
    {
        if(toLoad == null)
        {
            Debug.LogWarning("Trying to load null save object");
            return;
        }
        IsLoading = true;
        string filepath = GetSavePath(toLoad.GetSaveName());
        if (File.Exists(filepath))
        {
            byte[] bytes = File.ReadAllBytes(filepath);
            SaveState state = SerializationUtility.DeserializeValue<SaveState>(bytes, DataFormat.Binary);
            toLoad.LoadObject(state);
            Debug.Log("Loaded " + filepath);
        }
        IsLoading = false;
    }

    [Button("Search Saves")]
    public void SearchSaves()
    {
        UpdateSaveList();
    }

    [Button("Clean Saves")]
    public void CleanAllSaves()
    {
        foreach(string path in Directory.GetFiles(Application.persistentDataPath))
        {
            if(path.EndsWith(SAVE_EXTENSION))
            {
                Debug.Log("Deleting " + path);
                File.Delete(path);
            }
        }
        UpdateSaveList();
    }

    private string GetSavePath(string fileName)
    {
        GameInfo info = GameManager.Instance.gameInfo;
        string middlePart = info == null ? "/" : ("/game" + info.saveNumber + ".");
        return Application.persistentDataPath + middlePart + fileName + SAVE_EXTENSION;
    }

    private void MergeSaveLists(List<ISavable> newSavables)
    {
        foreach(ISavable sav in newSavables)
        {
            if(!objectsToSave.Contains(sav))
            {
                objectsToSave.Add(sav);
            }
        }
    }

    private void DeleteSave(string path)
    {
        foreach (string fullPath in Directory.GetFiles(Application.persistentDataPath))
        {
            // TODO: bug when deleting "default" saves created when not going through the main menu
            if (fullPath.EndsWith(path))
            {
                File.Delete(fullPath);
            }
        }
        UpdateSaveList();
    }

    private void UpdateSaveList()
    {
        if (_existingSaves == null)
        {
            _existingSaves = new List<string>();
        }
        _existingSaves.Clear();
        foreach (string path in Directory.GetFiles(Application.persistentDataPath))
        {
            if (path.EndsWith(SAVE_EXTENSION))
            {
                string[] split = path.Split('/');
                _existingSaves.Add(split[split.Length - 1]);
            }
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using System.IO;
using Sirenix.OdinInspector;

public class SaveManager : SerializedMonoBehaviour
{
    private readonly string SAVE_EXTENSION = ".save";

    public List<ISavable> objectsToSave;

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
        IsSaving = true;
        SaveState state = toSave.GetObjectToSave();
        byte[] bytes = SerializationUtility.SerializeValue(state, DataFormat.Binary);
        string path = GetSavePath(toSave.GetSaveName());
        File.WriteAllBytes(path, bytes);
        IsSaving = false;
        Debug.Log("Saved " + path);
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
        IsLoading = true;
        string filepath = GetSavePath(toLoad.GetSaveName());
        if (File.Exists(filepath))
        {
            byte[] bytes = File.ReadAllBytes(filepath);
            SaveState state = SerializationUtility.DeserializeValue<SaveState>(bytes, DataFormat.Binary);
            toLoad.LoadObject(state);
        }
        IsLoading = false;
        Debug.Log("Loaded " + filepath);
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
    }

    private string GetSavePath(string fileName)
    {
        return Application.persistentDataPath + "/" + fileName + SAVE_EXTENSION;
    }
}

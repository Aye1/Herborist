using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using System.IO;
using System;
using Sirenix.OdinInspector;

public class SaveManager : SerializedMonoBehaviour
{

    public static SaveManager Instance { get; private set; }

    public List<ISavable> objectsToSave;

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
            SaveState state = toSave.GetObjectToSave();
            byte[] bytes = SerializationUtility.SerializeValue(state, DataFormat.Binary);
            File.WriteAllBytes(GetSavePath(toSave.GetSaveName()), bytes);
        }
        Debug.Log("Save finished");
    }

    public void LoadGame()
    {
        foreach(ISavable toLoad in objectsToSave)
        {
            string filepath = GetSavePath(toLoad.GetSaveName());
            if(File.Exists(filepath))
            {
                byte[] bytes = File.ReadAllBytes(filepath);
                SaveState state = SerializationUtility.DeserializeValue<SaveState>(bytes, DataFormat.Binary);
                toLoad.LoadObject(state);
            }
        }
        Debug.Log("Load finished");
    }

    private string GetSavePath(string fileName)
    {
        return Application.persistentDataPath + "/" + fileName;
    }
}

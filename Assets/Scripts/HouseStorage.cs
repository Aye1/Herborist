using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class HouseStorage : MonoBehaviour
{
    public static HouseStorage Instance { get; private set; }

    public Inventory StorageInventory
    {
        get; private set;
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            StorageInventory = GetComponent<Inventory>();
            SaveManager.Instance.objectsToSave.Add(StorageInventory);
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
        SaveManager.Instance.objectsToSave.Remove(StorageInventory);
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
        if(newState == GameState.MainMenu)
        {
            SaveManager.Instance.objectsToSave.Remove(StorageInventory);
            Destroy(gameObject);
        }
    }

    public List<CollectibleScriptableObject> GetUnidentifiedComponents()
    {
        return StorageInventory.GetUnidentifiedComponents();
    }
}

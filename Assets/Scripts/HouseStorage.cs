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
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

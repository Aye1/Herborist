using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class UIKeyMapper : MonoBehaviour
{
    public static UIKeyMapper Instance { get; private set; }

    [SerializeField, Required, AssetsOnly]
    private UIKeyBindings _bindings;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            DontDestroyOnLoad(gameObject);
        }
    }

    public Sprite GetSprite(KeyCode key)
    {
        if(_bindings == null || _bindings.bindings == null)
        {
            return null;
        }
        return _bindings.GetSprite(key);
    }
}

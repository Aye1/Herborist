using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;

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
        if(_bindings == null)
        {
            return null;
        }
        return _bindings.GetSprite(key);
    }

    public Sprite GetSprite(string displayName)
    {
        if(_bindings == null)
        {
            return null;
        }
        return _bindings.GetSprite(displayName);
    }

    public Sprite GetSpriteForActionWithPath(string path)
    {
        InputAction action = GameManager.Instance.Actions.FindAction(path);
        return action == null ? null : GetSprite(action.GetBindingDisplayString());
    }
}

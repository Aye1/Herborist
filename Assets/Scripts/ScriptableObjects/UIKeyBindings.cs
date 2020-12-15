using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using System.Linq;

[Serializable]
public class UIKeyBinding
{
    [HorizontalGroup("Group"), HideLabel]
    public KeyCode key;
    [PreviewField, HorizontalGroup("Group", MaxWidth = 50), HideLabel]
    public Sprite sprite;
}

[CreateAssetMenu(fileName = "UIKeyBindings", menuName = "Herborist/UI/UI - Key bindings")]
[Serializable]
public class UIKeyBindings : SerializedScriptableObject
{
    public List<UIKeyBinding> bindings;

    public Sprite GetSprite(KeyCode key)
    {
        return bindings == null ? null : bindings.FirstOrDefault(b => b.key == key).sprite;
    }
}

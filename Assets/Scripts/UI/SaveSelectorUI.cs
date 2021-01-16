using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

public class SaveSelectorUI : MonoBehaviour
{
    [SerializeField, Required, ChildGameObjectsOnly]
    public List<GameObject> saveButtons;

    private void OnEnable()
    {
        SetKeyboardFocus();
    }

    private void SetKeyboardFocus()
    {
        if (saveButtons != null && saveButtons.Count > 0)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(saveButtons[0]);
        }
    }
}

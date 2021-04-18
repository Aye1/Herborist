using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SaveSelectorUI : BasePopup
{
    [SerializeField, Required, ChildGameObjectsOnly]
    public List<SaveSelectorItemUI> saveButtons;

    private void SetKeyboardFocus()
    {
        if (saveButtons != null && saveButtons.Count > 0)
        {
            NavigationManager.Instance.SetFocus(saveButtons[0].gameObject);
        }
    }

    #region BasePopup implementation
    protected override void CustomOnDisable()
    {
        return;
    }

    protected override void CustomOnEnable()
    {
        SetKeyboardFocus();
    }

    protected override void OnPopupClosing()
    {
        return;
    }
    #endregion
}

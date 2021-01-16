using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public abstract class BasePopup : MonoBehaviour
{
    public static bool ArePopupOpen
    {
        // Pause menu is always enabled, so that it can handle itself
        get { return OpenPopups.Count > 1; }
    }

    private static List<BasePopup> _openPopups;

    private static List<BasePopup> OpenPopups
    {
        get
        {
            if(_openPopups == null)
            {
                _openPopups = new List<BasePopup>();
            }
            return _openPopups;
        }
    }

    private readonly string CANCEL_ACTION = "Custom UI/Cancel";

    // Start is called before the first frame update
    protected void OnEnable()
    {
        BindControls();
        CustomOnEnable();
        OpenPopups.Add(this);
    }

    protected void OnDisable()
    {
        CustomOnDisable();
        UnBindControls();
        OpenPopups.Remove(this);
    }

    protected abstract void CustomOnEnable();
    protected abstract void CustomOnDisable();
    protected abstract GameObject GetObjectToDeactivate();
    protected abstract void OnPopupClosing();

    void BindControls()
    {
        InputAction cancelAction = GameManager.Instance.Actions.FindAction(CANCEL_ACTION);

        cancelAction.performed += OnCancel;

        cancelAction.Enable();
    }

    void UnBindControls()
    {
        InputAction cancelAction = GameManager.Instance.Actions.FindAction(CANCEL_ACTION);

        cancelAction.performed -= OnCancel;
    }

    void OnCancel(InputAction.CallbackContext ctx)
    {
        OnPopupClosing();
        GetObjectToDeactivate().SetActive(false);
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public abstract class BasePopup : MonoBehaviour
{
    private readonly string CANCEL_ACTION = "Custom UI/Cancel";

    // Start is called before the first frame update
    protected void OnEnable()
    {
        BindControls();
        CustomOnEnable();
    }

    protected void OnDisable()
    {
        CustomOnDisable();
        UnBindControls();
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

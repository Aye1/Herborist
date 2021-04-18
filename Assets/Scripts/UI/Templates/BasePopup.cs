using UnityEngine;

public abstract class BasePopup : MonoBehaviour, INavigable
{
    // Start is called before the first frame update
    protected void OnEnable()
    {
        NavigationManager.Instance.PushNavigation(this);
        CustomOnEnable();
    }

    protected void OnDisable()
    {
        CustomOnDisable();
    }

    protected void ClosePopup()
    {
        NavigationManager.Instance.PopNavigation();
    }

    protected abstract void CustomOnEnable();
    protected abstract void CustomOnDisable();
    protected abstract void OnPopupClosing();

    #region INavigable implementation
    public void OnNavigate()
    {
        return;
    }

    public void OnComingBack()
    {
        CustomOnEnable();
    }

    public void OnCancel()
    {
        OnPopupClosing();
        gameObject.SetActive(false);
    }

    public bool IsRemovable()
    {
        return true;
    }
    #endregion
}

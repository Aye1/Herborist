using UnityEngine;
using UnityEngine.EventSystems;

public class BaseButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public virtual void OnSelect(BaseEventData eventData)
    {
        transform.localScale = 1.1f * Vector3.one;
    }

    public virtual void OnDeselect(BaseEventData eventData)
    {
        transform.localScale = Vector3.one;
    }

    private void OnDisable()
    {
        Debug.Log("OnDeselect ?");
        OnDeselect(null);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BaseButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private Vector3 _scale;

    public void OnSelect(BaseEventData eventData)
    {
        _scale = transform.localScale;
        transform.localScale = 1.1f * Vector3.one; 
    }

    public void OnDeselect(BaseEventData eventData)
    {
        transform.localScale = _scale;
    }

}

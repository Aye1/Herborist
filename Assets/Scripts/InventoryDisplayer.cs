using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Unisloth.Localization;

public class InventoryDisplayer : MonoBehaviour, IInteractable
{
    [SerializeField, Required, SceneObjectsOnly] private InventoryUI _iventoryUI;
    [SerializeField, TranslationKey] private string _localizationKey;

    #region IInteractable implementation
    public bool CanInteract()
    {
        return true;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public string GetInteractionTextLocKey()
    {
        return _localizationKey;
    }

    public void Interact(GameObject aPLayer)
    {
        _iventoryUI.gameObject.SetActive(true);
    }
    #endregion
}

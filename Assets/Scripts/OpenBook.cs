using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Unisloth.Localization;

public class OpenBook : MonoBehaviour, IInteractable
{
    [TranslationKey]
    public string locKey;
    [Required]
    public BookUI bookUI;

    public string GetInteractionTextLocKey()
    {
        return locKey;
    }

    public bool CanInteract()
    {
        return true;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void Interact(GameObject aPLayer)
    {
        bookUI.gameObject.SetActive(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unisloth.Localization;

public class DoorInteractable : Teleport, IInteractable
{
    [TranslationKey]
    public string locKey;

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        //Do nothing, handled by IIInteractable
    }

    public void Interact(GameObject aPlayer)
    {
        TeleportPlayer(aPlayer);
    }

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
        return locKey;
    }
}


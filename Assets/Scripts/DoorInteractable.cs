using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unisloth.Localization;
using Sirenix.OdinInspector;

public class DoorInteractable : MonoBehaviour, IInteractable
{
    [TranslationKey]
    public string locKey;

    [SerializeField, Required]
    private Transform teleportPosition;

    public void Interact(GameObject aPlayer)
    {
        aPlayer.transform.position = teleportPosition.position;
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


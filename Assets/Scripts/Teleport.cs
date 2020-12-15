using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour , IInteractable
{
    public GameObject myTeleportEnd;
    public bool myNeedInteraction = false;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (myNeedInteraction == false)
        { 
            TeleportPlayer(col.gameObject);
        }
    }

    private void TeleportPlayer(GameObject aPlayer)
    {
        aPlayer.transform.position = myTeleportEnd.transform.position;
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
        return "teleport_interaction";
    }
}

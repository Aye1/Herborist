using UnityEngine;

public interface IInteractable
{
    void Interact(GameObject aPLayer);
    bool CanInteract();
    GameObject GetGameObject();
}

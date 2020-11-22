using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionManager : MonoBehaviour
{
    private List<IInteractable> myInteractables;

    // Start is called before the first frame update
    void Start()
    {
        myInteractables = new List<IInteractable>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IInteractable interactable = collision.gameObject.GetComponent<IInteractable>();

        if (interactable != null && myInteractables.Contains(interactable) == false)
        {
            myInteractables.Add(interactable);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IInteractable interactable = collision.gameObject.GetComponent<IInteractable>();

        if (interactable != null && myInteractables.Contains(interactable))
        {
            myInteractables.Remove(interactable);
        }
    }

    private void OnInteract()
    {
        if(myInteractables.Count > 0)
        {
            myInteractables[0].Interact(this.gameObject);
        }
    }
}

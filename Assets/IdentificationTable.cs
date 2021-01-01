using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdentificationTable : MonoBehaviour, IInteractable
{
    public IdentificationKeyNode identificationTreeRoot;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public string GetInteractionTextLocKey()
    {
        return "interact_Identification_table";
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
        throw new System.NotImplementedException();
    }
}

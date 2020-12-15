using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionUI : MonoBehaviour
{
    PlayerInteractionManager _player;

    // Start is called before the first frame update
    void Start()
    {
        _player = FindObjectOfType<PlayerInteractionManager>();  
    }

    // Update is called once per frame
    void Update()
    {
        if(_player != null)
        {
            Display(_player.CanInteract);
        }   
    }

    private void Display(bool visible)
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(visible);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public GameObject myTeleportEnd;
    public bool myNeedInteraction = false;
    private GameObject myPlayer = null;
    // Start is called before the first frame update
    void Start()
    {
    }

    private void Update()
    {
        if(myNeedInteraction && myPlayer != null && Input.GetKeyDown(KeyCode.A))
        {
            TeleportPlayer();
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        myPlayer = col.gameObject;
        if (myNeedInteraction)
        {

        }
        else
        {
            TeleportPlayer();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        myPlayer = null;
    }

    private void TeleportPlayer()
    {
        myPlayer.transform.position = myTeleportEnd.transform.position;
    }
}

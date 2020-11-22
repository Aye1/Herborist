using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public GameObject myTeleportEnd;
    // Start is called before the first frame update
    void Start()
    {
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("enter collision");
        col.gameObject.transform.position = myTeleportEnd.transform.position;
    }
}

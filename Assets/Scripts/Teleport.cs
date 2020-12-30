using UnityEngine;

public class Teleport : MonoBehaviour 
{
    public GameObject myTeleportEnd;

    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
         TeleportPlayer(col.gameObject);
    }

    protected void TeleportPlayer(GameObject aPlayer)
    {
        aPlayer.transform.position = myTeleportEnd.transform.position;
    }
}
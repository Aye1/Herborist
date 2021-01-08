using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Teleport : MonoBehaviour
{
    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (col.isTrigger == false)
        {
            PlayerMovement player = col.GetComponent<PlayerMovement>();
            if (player != null)
            {
                TeleportPlayer(player.transform);
            }
        }
    }

    protected abstract void TeleportPlayer(Transform aPlayer);
}
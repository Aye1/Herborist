using UnityEngine;

public class TeleportLocal : Teleport
{
    public Transform myTeleportEnd;

    protected override void TeleportPlayer(Transform aPlayer)
    {
        aPlayer.transform.position = myTeleportEnd.position;
    }
}

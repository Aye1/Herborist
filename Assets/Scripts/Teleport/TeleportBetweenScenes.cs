using UnityEngine;

public class TeleportBetweenScenes : Teleport
{
    public Scene destinationScene;

    protected override void TeleportPlayer(Transform aPlayer)
    {
        SceneSwitcher.Instance.GoToScene(destinationScene);
    }
}

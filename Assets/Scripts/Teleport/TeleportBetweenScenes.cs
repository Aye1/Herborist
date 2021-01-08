using UnityEngine;

public class TeleportBetweenScenes : Teleport
{
    public SceneType destinationScene;

    protected override void TeleportPlayer(Transform aPlayer)
    {
        SceneSwitcher.Instance.GoToScene(destinationScene);
    }
}

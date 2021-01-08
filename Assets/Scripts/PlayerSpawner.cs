using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public SceneType fromScene;

    // Start is called before the first frame update
    void Start()
    {
        if ((fromScene == SceneType.Unknown) || (fromScene == SceneSwitcher.Instance.PreviousScene))
        {
            PlayerMovement.Instance.transform.position = transform.position;
        }
    }
}

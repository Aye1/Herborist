using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public Scene fromScene;

    // Start is called before the first frame update
    void Start()
    {
        if ((fromScene == Scene.Unknown) || (fromScene == SceneSwitcher.Instance.PreviousScene))
        {
            PlayerMovement.Instance.transform.position = transform.position;
        }
    }
}

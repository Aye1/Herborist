
using UnityEngine;

public class UIFollowPlayer : MonoBehaviour
{
    private Vector2 _offset;

    public bool useOffset;
    [HideInInspector] public PlayerMovement player;

    private void Awake()
    {
        _offset = new Vector2(Alea.GetFloat(-30f, 20f), Alea.GetFloat(10f, 50f));
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = Camera.main.WorldToScreenPoint(player.transform.position);
        if (useOffset)
        {
            newPos = newPos + new Vector3(_offset.x, _offset.y, 0);
        }
        transform.position = newPos;
    }
}

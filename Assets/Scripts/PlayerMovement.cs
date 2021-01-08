using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 1.0f;
    private Rigidbody2D _body;

    private Vector2 latestMovement;

    public static PlayerMovement Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _body = GetComponent<Rigidbody2D>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        ApplyMovement();
    }

    public void OnMove(InputValue v)
    {
        latestMovement = v.Get<Vector2>();
    }

    private void ApplyMovement()
    {
        Move(latestMovement * speed);
    }

    private void Move(Vector2 movement)
    {
        _body.velocity = movement;
    }
}

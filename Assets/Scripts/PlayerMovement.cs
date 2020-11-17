using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 1.0f;

    private Vector2 latestMovement;

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
        transform.Translate(movement);
    }
}

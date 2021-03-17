using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class AlphaFadeInteraction : MonoBehaviour
{
    [PropertyRange(0.0f, 1.0f)]
    public float transparentAlpha = 0.5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject != gameObject)
            ChangeAlpha(transparentAlpha, collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject != gameObject)
            ChangeAlpha(1.0f, collision.gameObject);
    }

    private void ChangeAlpha(float targetAlpha, GameObject obj)
    {
        foreach(SpriteRenderer rend in obj.GetComponents<SpriteRenderer>())
        {
            Color tmp = rend.color;
            rend.color = new Color(tmp.r, tmp.g, tmp.b, targetAlpha);
        }

        foreach(SpriteRenderer rend in obj.GetComponentsInChildren<SpriteRenderer>())
        {
            Color tmp = rend.color;
            rend.color = new Color(tmp.r, tmp.g, tmp.b, targetAlpha);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SpriteChanger : MonoBehaviour
{
    public static SpriteChanger Instance { get; private set; }

    [Required, SerializeField]
    private Material _defaultSpriteMaterial;
    [Required, SerializeField]
    private Material _outlinedSpriteMaterial;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Outline(GameObject objectToOutline, bool toggle)
    {
        Material newMaterial = toggle ? _outlinedSpriteMaterial : _defaultSpriteMaterial;
        objectToOutline.GetComponent<SpriteRenderer>().material = newMaterial;
    }
}

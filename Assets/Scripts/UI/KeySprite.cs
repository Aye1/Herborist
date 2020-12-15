using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class KeySprite : MonoBehaviour
{
    public KeyCode associatedKey;

    void Start()
    {
        GetComponent<Image>().sprite = UIKeyMapper.Instance.GetSprite(associatedKey);   
    }
}

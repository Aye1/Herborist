using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class KeySprite : MonoBehaviour
{
    public string inputActionPath;

    void Start()
    {
        GetComponent<Image>().sprite = UIKeyMapper.Instance.GetSpriteForActionWithPath(inputActionPath);
    }
}

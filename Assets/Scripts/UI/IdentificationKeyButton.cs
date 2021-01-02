using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class IdentificationKeyButton : MonoBehaviour
{
    [SerializeField, Required, ChildGameObjectsOnly]
    private Image keySprite;
    [SerializeField, Required, ChildGameObjectsOnly]
    private TextMeshProUGUI keyText;
    private IdentificationKeyNode keyNode;

    public void SetKeyNode(IdentificationKeyNode aKeyNode)
    {
        keyNode = aKeyNode;

        if (aKeyNode.identificationData != null)
        {
            keyText.text = aKeyNode.identificationData.identificationTitle;
            keySprite.sprite = aKeyNode.identificationData.plate;
        }
    }

    public IdentificationKeyNode GetKeyNode()
    { return keyNode; }

}

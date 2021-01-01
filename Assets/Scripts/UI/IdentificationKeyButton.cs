using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class IdentificationKeyButton : MonoBehaviour
{
    private IdentificationKeyNode keyNode;
    private IdentificationKeyLeaf keyLeaf;
    private TextMeshProUGUI keyText;
    private Image keySprite;

    public void SetKeyNode(IdentificationKeyNode aKeyNode)
    {
        keyNode = aKeyNode;

        keyText = GetComponentInChildren<TextMeshProUGUI>();
        keySprite = GetComponentInChildren<Image>();

        if (aKeyNode.identificationData != null)
        {
            keyText.text = aKeyNode.identificationData.identificationTitle;
            keySprite.sprite = aKeyNode.identificationData.plate;
        }
    }
    public void SetKeyLeaf(IdentificationKeyLeaf aKeyLeaf)
    {
        keyLeaf = aKeyLeaf;

        keyText = GetComponentInChildren<TextMeshProUGUI>();
        keySprite = GetComponentInChildren<Image>();

        keyText.text = aKeyLeaf.identificationTitle;

    }
    public IdentificationKeyNode GetKeyNode()
    { return keyNode; }

    public IdentificationKeyLeaf GetKeyLeaf()
    { return keyLeaf; }
}

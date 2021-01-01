using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using TMPro;

public class IdentificationTableUI : MonoBehaviour
{
    [Required]
    public IdentificationKeyNode identificationTreeRoot;
    private IdentificationKeyLeaf leafToIdentify;
    [SerializeField, Required]
    private Image plantToIdentify;
    [SerializeField, Required]
    private TextMeshProUGUI choicesText;
    [SerializeField, Required]
    private GameObject choicesButtonsContainer;
    [SerializeField, Required, AssetsOnly]
    private IdentificationKeyButton choiceButtonTemplate;

    public void SetLeafToIdentify(IdentificationKeyLeaf aLeaf)
    {
        leafToIdentify = aLeaf;
        plantToIdentify.sprite = leafToIdentify.plate;
        DisplayNode(identificationTreeRoot);
    }
    void DisplayNode(IdentificationKeyNode aNode)
    {
        if (aNode.identificationData != null)
        {
            choicesText.text = aNode.identificationData.identificationTitle;
        }
        foreach (IdentificationKeyNode subNode in aNode.treeNodes)
        {
            IdentificationKeyButton newButton = Instantiate(choiceButtonTemplate, choicesButtonsContainer.transform);
            newButton.SetKeyNode(subNode);
            newButton.GetComponent<Button>().onClick.AddListener(() => OnIdentificationNodeButtonClicked(newButton.GetKeyNode()));
        }
        foreach (IdentificationKeyLeaf subLeaf in aNode.treeLeaves)
        {
            IdentificationKeyButton newButton = Instantiate(choiceButtonTemplate, choicesButtonsContainer.transform);
            newButton.SetKeyLeaf(subLeaf);
            newButton.GetComponent<Button>().onClick.AddListener(() => OnIdentificationLeafButtonClicked(newButton.GetKeyLeaf()));
        }
    }

    private void OnIdentificationNodeButtonClicked(IdentificationKeyNode node)
    {
        foreach (Transform child in choicesButtonsContainer.transform)
        {
            Destroy(child.gameObject);
        }
        DisplayNode(node);
    }
    private void OnIdentificationLeafButtonClicked(IdentificationKeyLeaf aLeaf)
    {
        if (leafToIdentify == aLeaf)
        {
            gameObject.SetActive(false);
        }
    }
}

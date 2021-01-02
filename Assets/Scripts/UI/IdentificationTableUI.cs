using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using TMPro;

public class IdentificationTableUI : MonoBehaviour
{
    [Required, AssetsOnly]
    public IdentificationKeyNode identificationTreeRoot;
    private IdentificationKeyData leafToIdentify;
    [SerializeField, Required, ChildGameObjectsOnly]
    private Image plantToIdentify;
    [SerializeField, Required, ChildGameObjectsOnly]
    private TextMeshProUGUI descriptionText;
    [SerializeField, Required, ChildGameObjectsOnly]
    private TextMeshProUGUI choicesText;
    [SerializeField, Required, ChildGameObjectsOnly]
    private GameObject choicesButtonsContainer;
    [SerializeField, Required, AssetsOnly]
    private IdentificationKeyButton choiceButtonTemplate;

    [SerializeField, Required, ChildGameObjectsOnly]
    private Button returnButton;

    [SerializeField, Required, ChildGameObjectsOnly]
    private Button leaveButton;

    private List<IdentificationKeyNode> path;

    private void Start()
    {
        gameObject.SetActive(false);
        path = new List<IdentificationKeyNode>();
        returnButton.GetComponent<Button>().onClick.AddListener(OnReturnButtonClicked);
        returnButton.interactable = false;
        leaveButton.GetComponent<Button>().onClick.AddListener(OnLeaveButtonClicked);

    }

    public void SetPlantToIdentify(IdentificationKeyData aLeaf)
    {
        leafToIdentify = aLeaf;
        plantToIdentify.sprite = leafToIdentify.plate;
        path.Clear();
        DisplayNode(identificationTreeRoot);
    }
    private void LeaveIdentificationTable()
    {
        gameObject.SetActive(false);
    }

    void DisplayNode(IdentificationKeyNode aNode)
    {
        path.Add(aNode);
        if (path.Count >= 2)
        {
            returnButton.interactable = true;
        }
        foreach (Transform child in choicesButtonsContainer.transform)
        {
            Destroy(child.gameObject);
        }
        if (aNode.identificationData != null)
        {
            choicesText.text = aNode.identificationData.identificationTitle;
            descriptionText.text = aNode.identificationData.identificationDescription;
        }
        foreach (IdentificationKeyNode subNode in aNode.treeNodes)
        {
            IdentificationKeyButton newButton = Instantiate(choiceButtonTemplate, choicesButtonsContainer.transform);
            newButton.SetKeyNode(subNode);
            newButton.GetComponent<Button>().onClick.AddListener(() => OnIdentificationNodeButtonClicked(newButton.GetKeyNode()));
        }
    }

    private void OnIdentificationNodeButtonClicked(IdentificationKeyNode node)
    {
        if (node.IsLeaf())
        {
            if (leafToIdentify == node.identificationData)
            {
                LeaveIdentificationTable();
            }
        }
        else
        {
            DisplayNode(node);
        }
    }
    private void OnReturnButtonClicked()
    {
        int nodeNumber = path.Count;
        if (nodeNumber >= 2)
        {
            IdentificationKeyNode nodeToGo = path[nodeNumber - 2];
            path.RemoveRange(nodeNumber - 2, 2);
            DisplayNode(nodeToGo);
        }
    }
    private void OnLeaveButtonClicked()
    {
        LeaveIdentificationTable();
    }
}

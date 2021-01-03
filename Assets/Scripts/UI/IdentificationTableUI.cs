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
    private GameObject hierarchyContainer;
    [SerializeField, Required, ChildGameObjectsOnly]
    private GameObject choicesButtonsContainer;
    [SerializeField, Required, AssetsOnly]
    private IdentificationKeyButton choiceButtonTemplate;
    [SerializeField, Required, AssetsOnly]
    private Button hierarchyNodeButtonTemplate;

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
        foreach (Transform child in choicesButtonsContainer.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in hierarchyContainer.transform)
        {
            Destroy(child.gameObject);
        }

        path.Add(aNode);
        returnButton.interactable = path.Count >= 2;

        foreach (IdentificationKeyNode subNode in path)
        {
            Button newButton = Instantiate(hierarchyNodeButtonTemplate, hierarchyContainer.transform);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = subNode.identificationData.identificationTitle;
            newButton.GetComponent<Button>().onClick.AddListener(() => OnHierarchyNodeButtonClicked(subNode));
        }

        if (aNode.identificationData != null)
        {
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
    private void OnHierarchyNodeButtonClicked(IdentificationKeyNode node)
    {
        for (int i = 0; i < path.Count; i++)
        {
            if (path[i] == node)
            {
                ReturnToNodeAtIndex(i);
                break;
            }
        }
    }

    private void ReturnToNodeAtIndex(int index)
    {
        int numberOfNodesToRemove = path.Count - index;
        IdentificationKeyNode nodeToGo = path[index];
        path.RemoveRange(index, numberOfNodesToRemove);
        DisplayNode(nodeToGo);
    }

    private void OnReturnButtonClicked()
    {
        int nodeNumber = path.Count - 2; //before last element;
        if (nodeNumber >= 0)
        {
            ReturnToNodeAtIndex(nodeNumber);
        }
    }
    private void OnLeaveButtonClicked()
    {
        LeaveIdentificationTable();
    }
}

using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Identification Key Node", menuName = "ScriptableObjects/FloraTree/IdentificationKeyNode")]
public class IdentificationKeyNode : ScriptableObject
{
    [Title("Node data", HorizontalLine = false)]
    public IdentificationKeyData identificationData;

    [Title("Tree Node Access data", HorizontalLine = false)]
    public List<IdentificationKeyNode> treeNodes;

    public bool IsLeaf()
    {
        return treeNodes.Count == 0;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Vector2Int position;
    public List<Node> connectedNodes;

    public void Connect(Node n)
    {
        if(!connectedNodes.Contains(n))
        {
            connectedNodes.Add(n);
            n.Connect(this);
        }
    } 
}

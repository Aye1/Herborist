using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph
{
    public List<Node> nodes;
    public Node firstNode;

    public void AddNode(Node node)
    {
        if(!nodes.Contains(node))
        {
            nodes.Add(node);
        }
    }

    public void ConnectNodes(Node n1, Node n2)
    {
        n1.Connect(n2);
    }
}

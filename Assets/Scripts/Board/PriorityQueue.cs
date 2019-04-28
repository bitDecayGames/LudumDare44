using System.Collections;
using System.Collections.Generic;

public class PriorityQueue {
    private IComparer NodeComp = new NodePathComparer();
    private ArrayList nodes = new ArrayList();
    public int Length {
        get {
            return nodes.Count;
        }
    }
    public bool Contains(object node) {
        return nodes.Contains(node);
    }
    public Search.NodePath GetFirstNode() {
        if (nodes.Count > 0) {
            return (Search.NodePath) nodes[0];
        }
        return null;
    }
    public void Push(Search.NodePath node) {
        nodes.Add(node);
        nodes.Sort(NodeComp);
    }
    public void Remove(Search.NodePath node) {
        nodes.Remove(node);
        // Don't sort on remove for now
        //nodes.Sort(NodeComp);
    }
}

internal class NodePathComparer : IComparer
{
    public int Compare(object x, object y)
    {
        return ((Search.NodePath)x).weight - ((Search.NodePath)y).weight;
//        return ((Search.NodePath)y).weight - ((Search.NodePath)x).weight;
    }
}
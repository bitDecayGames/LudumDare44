using System.Collections.Generic;

public class PriorityQueue {
    private NodePathComparer NodeComp = new NodePathComparer();
    private List<Search.NodePath> nodes = new List<Search.NodePath>();

    public int Length {
        get { return nodes.Count; }
    }

    public bool Contains(Search.NodePath node) {
        return nodes.Contains(node);
    }

    public Search.NodePath GetFirstNode() {
        if (nodes.Count > 0) {
            return nodes[0];
        }

        return null;
    }

    public void Push(Search.NodePath node) {
        nodes.Add(node);
    }

    public void Sort()
    {
        nodes.Sort(NodeComp);
    }

    public void Remove(Search.NodePath node) {
        nodes.Remove(node);
        // Don't sort on remove for now
        //nodes.Sort(NodeComp);
    }
}

internal class NodePathComparer : IComparer<Search.NodePath> {
    public int Compare(Search.NodePath x, Search.NodePath y) {
        return x.weight - y.weight;
    }
}
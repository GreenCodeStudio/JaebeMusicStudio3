using JaebeMusicStudio3.Core.AudioNodes;

namespace JaebeMusicStudio3.Front.Utils;

public class NodesReorganizer(
    IEnumerable<IAudioNode> nodes,
    KeyValuePair<NodeInputDefinition, NodeOutputDefinition>[] connections)
{
    public Dictionary<IAudioNode, (double x, double y)> Ret = new();

    public static Dictionary<IAudioNode, (double x, double y)> Reorganize(IAudioNode? mainOutputNode,
        IEnumerable<IAudioNode> nodes, KeyValuePair<NodeInputDefinition, NodeOutputDefinition>[] connections)
    {
        var obj = (new NodesReorganizer(nodes, connections));
        obj.Reorganize(mainOutputNode);
        obj.ReorganizeRemaining();
        return obj.Ret;
    }

    private double GetFreeY(double x)
    {
        var z = Ret.Values.Where(v => v.x == x).Select(v => v.y);
        if (z.Any())
        {
            return z.Max() + 1;
        }
        else
        {
            return 0;
        }
    }

    private void Reorganize(IAudioNode? mainOutputNode)
    {
        if (!Ret.ContainsKey(mainOutputNode))
        {
            Ret[mainOutputNode] = (0, GetFreeY(0));
        }

        foreach (var connection in connections)
        {
            if (connection.Value.Node == mainOutputNode)
            {
                var anotherNode = connection.Key.Node;
                if (!Ret.ContainsKey(anotherNode))
                {
                    Ret[anotherNode] = (Ret[mainOutputNode].x + 1, GetFreeY(Ret[mainOutputNode].x + 1));
                    Reorganize(anotherNode);
                }
            }
            else if (connection.Key.Node == mainOutputNode)
            {
                var anotherNode = connection.Value.Node;
                if (!Ret.ContainsKey(anotherNode))
                {
                    Ret[anotherNode] = (Ret[mainOutputNode].x - 1, GetFreeY(Ret[mainOutputNode].x - 1));
                    Reorganize(anotherNode);
                }
            }
        }
    }

    private void ReorganizeRemaining()
    {
        foreach (var node in nodes)
        {
            if (!Ret.ContainsKey(node))
            {
                Ret[node] = (0, GetFreeY(0));
            }
        }
    }
}
namespace JaebeMusicStudio3.Core.AudioNodes;

public class NodeOutputDefinition : IEquatable<NodeOutputDefinition>
{
    public NodeConnectionType Type { get; set; }
    public IAudioNode Node { get; set; }
    public string Name { get; set; }

    public override bool Equals(object obj)
    {
        if (obj is NodeOutputDefinition other)
        {
            return this.Name == other.Name && this.Node == other.Node;
        }

        return false;
    }

    public bool Equals(NodeOutputDefinition other)
    {
        return this.Name == other.Name && this.Node == other.Node;
    }


    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Node);
    }

    public static bool operator ==(NodeOutputDefinition left, NodeOutputDefinition right)
    {
        return left.Equals(right);
    }
    public static bool operator !=(NodeOutputDefinition left, NodeOutputDefinition right)
    {
        return !left.Equals(right);
    }
}
using System.Text.Json.Serialization;

namespace JaebeMusicStudio3.Core.AudioNodes;

public class NodeOutputDefinition : IEquatable<NodeOutputDefinition>
{
    public NodeConnectionType Type { get; set; }
    [JsonIgnore]
    public IAudioNode? Node
    {
        get;
        set
        {
            field = value;
            NodeId = value?.Id ?? Guid.Empty;
        }
    }
    
    public Guid NodeId { get; set; }
    public string Name { get; set; }

    public override bool Equals(object obj)
    {
        if (obj is NodeOutputDefinition other)
        {
            return this.Name == other.Name && this.NodeId == other.NodeId;
        }

        return false;
    }

    public bool Equals(NodeOutputDefinition other)
    {
        return this.Name == other.Name && this.NodeId == other.NodeId;
    }


    public override int GetHashCode()
    {
        return HashCode.Combine(Name, NodeId);
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
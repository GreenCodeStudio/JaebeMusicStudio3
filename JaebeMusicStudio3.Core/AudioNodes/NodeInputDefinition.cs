using System.Text.Json.Serialization;

namespace JaebeMusicStudio3.Core.AudioNodes;

public class NodeInputDefinition
{
    public string Name { get; set; }
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
    public NodeConnectionType Type { get; set; }
    
    
    public override bool Equals(object obj)
    {
        if (obj is NodeInputDefinition other)
        {
            return this.Name == other.Name && this.NodeId == other.NodeId;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, NodeId);
    }
    public static bool operator ==(NodeInputDefinition left, NodeInputDefinition right)
    {
        return left.Equals(right);
    }
    public static bool operator !=(NodeInputDefinition left, NodeInputDefinition right)
    {
        return !left.Equals(right);
    }
}
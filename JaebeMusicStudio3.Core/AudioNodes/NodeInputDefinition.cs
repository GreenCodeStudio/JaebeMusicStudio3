namespace JaebeMusicStudio3.Core.AudioNodes;

public class NodeInputDefinition
{
    public string Name { get; set; }
    public IAudioNode Node { get; set; }
    public NodeConnectionType Type { get; set; }
    
    
    public override bool Equals(object obj)
    {
        if (obj is NodeInputDefinition other)
        {
            return this.Name == other.Name && this.Node == other.Node;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Node);
    }
    
}
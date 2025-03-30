namespace JaebeMusicStudio3.Core.AudioNodes;

public class NodeOutputDefinition
{
    public NodeConnectionType Type { get; set; }
    public IAudioNode Node { get; set; }
    public string Name { get; set; }
}
using JaebeMusicStudio3.Core.AudioRendering;

namespace JaebeMusicStudio3.Core.AudioNodes;

public interface IAudioNode
{
    IEnumerable<NodeInputDefinition> Inputs { get; }
    IEnumerable<NodeOutputDefinition> Outputs { get; }
    Task<Dictionary<string, object>> Render(RenderingChunk chunk, Dictionary<string, object> inputs);
    string Title { get; }
}
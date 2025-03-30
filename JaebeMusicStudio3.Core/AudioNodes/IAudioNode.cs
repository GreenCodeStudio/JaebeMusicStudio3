using JaebeMusicStudio3.Core.AudioRendering;

namespace JaebeMusicStudio3.Core.AudioNodes;

public interface IAudioNode
{
    Dictionary<string,NodeInputDefinition> Inputs { get; }
    Dictionary<string,NodeOutputDefinition> Outputs { get; }
    Task<Dictionary<string, object>> Render(RenderingChunk chunk);
}
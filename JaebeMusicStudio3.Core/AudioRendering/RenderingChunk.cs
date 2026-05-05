using JaebeMusicStudio3.Core.AudioNodes;

namespace JaebeMusicStudio3.Core.AudioRendering;

public class RenderingChunk
{
    public RenderingProcess Process { get; set; }
    public long Start { get; set; }
    public long Length { get; set; }
    public Dictionary<IAudioNode, Task<Dictionary<string, object>>> Responses { get; } = new();
    public double StartSeconds =>Start / (double)Process.SampleRate;

    public async Task<object> GetResponse(NodeOutputDefinition x)
    {
        if (Responses.TryGetValue(x.Node, out var response))
        {
            return (await response)[x.Name];
        }
        else
        {
            return null;
        }
    }
}
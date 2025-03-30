using JaebeMusicStudio3.Core.AudioNodes;
using JaebeMusicStudio3.Core.AudioRendering;

namespace JaebeMusicStudio3.Core.Mixer;

public class Mixer
{
    private List<IAudioNode> _nodes = new List<IAudioNode>();
    public NodeOutputDefinition MainOutput { get; set; }

    public void Add(IAudioNode node)
    {
        lock (this)
        {
            _nodes.Add(node);
        }
    }

    public void Render(RenderingChunk chunk)
    {
        var initialization = new TaskCompletionSource();
        lock (chunk)
        {
            foreach (var node in _nodes)
            {
                chunk.Responses[node] = Task.Run(async () => await node.Render(chunk));
            }
        }

        initialization.SetResult();
    }
}
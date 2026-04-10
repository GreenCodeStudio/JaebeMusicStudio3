using JaebeMusicStudio3.Core.AudioNodes;
using JaebeMusicStudio3.Core.AudioRendering;

namespace JaebeMusicStudio3.Core.Mixer;

public class AudioMixer
{
    private List<IAudioNode> _nodes = new List<IAudioNode>();
    public NodeOutputDefinition MainOutput { get; set; }

    private Dictionary<NodeInputDefinition, NodeOutputDefinition> _connections =
        new Dictionary<NodeInputDefinition, NodeOutputDefinition>();

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
                chunk.Responses[node] = Task.Run(async () =>
                {
                    var inputs = new Dictionary<string, object>();
                    foreach (var input in node.Inputs)
                    {
                        if (_connections.TryGetValue(input, out NodeOutputDefinition output))
                        {
                            if ((await chunk.Responses[output.Node]).TryGetValue(output.Name, out object value))
                            {
                                inputs[input.Name] = value;
                            }
                        }
                    }

                    return await node.Render(chunk, inputs);
                });
            }
        }

        initialization.SetResult();
    }

    public void Connect(NodeOutputDefinition output, NodeInputDefinition input)
    {
        lock (this)
        {
            _connections.Add(input, output);
        }
    }

    public IEnumerable<IAudioNode> Nodes
    {
        get
        {
            lock (this)
            {
                return _nodes.ToArray();
            }
        }
    }

    public KeyValuePair<NodeInputDefinition, NodeOutputDefinition>[] Connections
    {
        get
        {
            lock (this)
            {
                return _connections.ToArray();
            }
        }
    }
}
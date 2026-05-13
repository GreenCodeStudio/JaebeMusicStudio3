using System.Text.Json.Serialization;
using JaebeMusicStudio3.Core.AudioNodes;
using JaebeMusicStudio3.Core.AudioRendering;
using JaebeMusicStudio3.Front.Utils;

namespace JaebeMusicStudio3.Core.Mixer;

public class AudioMixer : IJsonOnDeserialized
{
    public static AudioMixer Current = new AudioMixer();

    [JsonInclude] [JsonPropertyName("Nodes")]
    private List<IAudioNode> _nodes = new List<IAudioNode>();

    public NodeOutputDefinition MainOutput { get; set; }


    private Dictionary<NodeInputDefinition, NodeOutputDefinition> _connections =
        new Dictionary<NodeInputDefinition, NodeOutputDefinition>();

    private Dictionary<Guid, VisualPosition> _positions = new Dictionary<Guid, VisualPosition>();
    public event Action Changed;

    public void Add(IAudioNode node)
    {
        lock (this)
        {
            _nodes.Add(node);
        }

        Changed?.Invoke();
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

        Changed?.Invoke();
    }

    [JsonIgnore]
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

    [JsonIgnore]
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

    public VisualPosition GetPosition(IAudioNode node)
    {
        lock (this)
        {
            if (_positions.ContainsKey(node.Id))
                return _positions[node.Id];
            else
            {
                var pos = new VisualPosition();
                _positions[node.Id] = pos;
                return pos;
            }
        }
    }

    public void SetPosition(IAudioNode node, VisualPosition position)
    {
        lock (this)
        {
            _positions[node.Id] = position;
        }

        Changed?.Invoke();
    }

    public void Disconnect(KeyValuePair<NodeInputDefinition, NodeOutputDefinition> keyValuePair)
    {
        lock (this)
        {
            _connections.Remove(keyValuePair.Key);
        }

        Changed?.Invoke();
    }

    public void LoadVstFile(string filePath)
    {
    }

    [JsonPropertyName("Connections")]
    [JsonInclude]
    public IEnumerable<KeyValuePair<NodeInputDefinition, NodeOutputDefinition>> SerializableConnections
    {
        get
        {
            lock (this)
            {
                return _connections.ToArray();
            }
        }
        set
        {
            lock (this)
            {
                _connections = value.ToDictionary(kv => kv.Key, kv => kv.Value);
            }
        }
    }

    [JsonPropertyName("Positions")]
    [JsonInclude]
    public IEnumerable<KeyValuePair<Guid, VisualPosition>> SerializablePositions
    {
        get
        {
            lock (this)
            {
                return _positions.ToDictionary(kv => kv.Key, kv => kv.Value).ToArray();
            }
        }
        set
        {
            lock (this)
            {
                _positions = value.ToDictionary(kv => kv.Key, kv => kv.Value);
            }
        }
    }

    public void OnDeserialized()
    {
        _connections = _connections.ToDictionary(
            kv => _nodes.Find(n => n.Id == kv.Key.NodeId).Inputs.First(i => i.Name == kv.Key.Name),
            kv => _nodes.Find(n => n.Id == kv.Value.NodeId).Outputs.First(i => i.Name == kv.Value.Name)
        );
    }

    public void ReorganizePositions()
    {
        var newPositions = NodesReorganizer.Reorganize(MainOutput.Node, Nodes, Connections);
        var minX = newPositions.Values.Min(x => x.x);
        var minY = newPositions.Values.Min(x => x.y);
        foreach (var kv in newPositions)
        {
            SetPosition(kv.Key, new VisualPosition()
            {
                X = (kv.Value.x - minX) * 250,
                Y = (kv.Value.y - minY) * 150
            });
        }
    }
}
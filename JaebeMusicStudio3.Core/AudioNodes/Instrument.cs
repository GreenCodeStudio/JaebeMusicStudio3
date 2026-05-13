using System.Collections;
using System.Text.Json.Serialization;
using JaebeMusicStudio3.Core.AudioRendering;
using JaebeMusicStudio3.Core.IO;
using JaebeMusicStudio3.Core.Mixer;
using JaebeMusicStudio3.Core.Timeline;
using JaebeMusicStudio3.Front.Utils;

namespace JaebeMusicStudio3.Core.AudioNodes;

public class Instrument : IAudioNode
{
    public Guid Id { get; set; } = Guid.NewGuid();

    private List<IAudioNode> _nodes = new List<IAudioNode>();
    public NodeOutputDefinition MainOutput { get; set; }


    private Dictionary<NodeInputDefinition, NodeOutputDefinition> _connections =
        new Dictionary<NodeInputDefinition, NodeOutputDefinition>();

    private Dictionary<Guid, VisualPosition> _positions = new Dictionary<Guid, VisualPosition>();

    [JsonIgnore]
    public IEnumerable<NodeInputDefinition> Inputs => new List<NodeInputDefinition>()
    {
    };

    [JsonIgnore]
    public IEnumerable<NodeOutputDefinition> Outputs => new List<NodeOutputDefinition>()
    {
        new NodeOutputDefinition()
        {
            Name = "output",
            Node = this,
            Type = NodeConnectionType.SingleChannelAudio
        }
    };

    private class NoteReformed
    {
        public double Pitch { get; set; }
        public double Start { get; set; }
        public double Length { get; set; }
    }

    public async Task<Dictionary<string, object>> Render(RenderingChunk chunk, Dictionary<string, object> inputs)
    {
        var output = new SingleChannelAudioBuffer(chunk.Process.SampleRate, chunk.Length);
        List<NoteReformed> notes = new();
        if (chunk.Process.UseTimeline)
        {
            List<NoteLine> noteLines;
            lock (Timeline.Timeline.Current)
            {
                noteLines = Timeline.Timeline.Current.Items.OfType<NoteLine>().Where(l => l.Instrument == this)
                    .ToList();

                notes.AddRange(noteLines.SelectMany(x => x.Notes.Select(y => new NoteReformed
                {
                    Pitch = y.Pitch,
                    Start = y.Start * 60 / x.Tempo + x.OffsetSeconds,
                    Length = y.Length * 60 / x.Tempo,
                })));
            }
        }

        foreach (var x in IOWrapper.NotesInputs)
        {
            if (x.Instrument == this)
            {
                lock (x)
                {
                    notes.AddRange(x.GetNotes(chunk).Select(y => new NoteReformed
                    {
                        Pitch = y.Pitch,
                        Start = y.Start,
                        Length = y.Length,
                    }));
                }
            }
        }

        var chunkStartSeconds = chunk.Start / (double)chunk.Process.SampleRate;
        var chunkLengthSeconds = chunk.Length / (double)chunk.Process.SampleRate;
        foreach (var note in notes)
        {
            var startOffset = note.Start - chunkStartSeconds;
            var endOffset = note.Start + note.Length - chunkStartSeconds;
            if (startOffset < chunkLengthSeconds && endOffset > 0)
            {
                var i = startOffset < 0 ? 0 : (int)(startOffset * chunk.Process.SampleRate);
                var offset = startOffset > 0 ? 0 : (int)(-startOffset * chunk.Process.SampleRate);
                var length = (int)(chunkLengthSeconds * chunk.Process.SampleRate);
                if (length > chunk.Length)
                    length = (int)chunk.Length;
                if (i < length)
                {
                    var subChunk = new RenderingChunk()
                    {
                        Start = offset,
                        Length = length,
                        Process = chunk.Process
                    };
                    foreach (var node in _nodes)
                    {
                        subChunk.Responses[node] = Task.Run(async () =>
                        {
                            var inputs = new Dictionary<string, object>();
                            foreach (var input in node.Inputs)
                            {
                                if (_connections.TryGetValue(input, out NodeOutputDefinition output))
                                {
                                    if ((await subChunk.Responses[output.Node]).TryGetValue(output.Name,
                                            out object value))
                                    {
                                        inputs[input.Name] = value;
                                    }
                                }
                                else if (input.Type == NodeConnectionType.Note)
                                {
                                    inputs[input.Name] = new Note()
                                    {
                                        Pitch = note.Pitch,
                                        Start = 0,
                                        Length = note.Length
                                    };
                                }
                            }

                            return await node.Render(subChunk, inputs);
                        });
                    }

                    var responseMain = await subChunk.Responses[MainOutput.Node];
                    if (responseMain.TryGetValue(MainOutput.Name, out object valueMain))
                    {
                        var mainBuffer = valueMain as SingleChannelAudioBuffer;
                        for (var j = 0; j < length; j++)
                        {
                            output.Data[j] += mainBuffer.Data[i + j];
                        }
                    }
                }
            }
        }

        return new Dictionary<string, object>()
        {
            {
                "output", output
            }
        };
    }

    public string Title => "Instrument";

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

    public event Action Changed;

    public void Add(IAudioNode node)
    {
        lock (this)
        {
            _nodes.Add(node);
        }

        Changed?.Invoke();
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
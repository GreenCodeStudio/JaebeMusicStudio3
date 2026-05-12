using System.Numerics;
using System.Text.Json.Serialization;
using JaebeMusicStudio3.Core.AudioRendering;

namespace JaebeMusicStudio3.Core.AudioNodes;

public class MixNode : IAudioNode
{
    public Guid Id { get; }=Guid.NewGuid();
    [JsonIgnore]
    public IEnumerable<NodeInputDefinition> Inputs => new List<NodeInputDefinition>()
    {
        new NodeInputDefinition()
        {
            Name = "input1",
            Node = this,
            Type = NodeConnectionType.SingleChannelAudio
        },
        new NodeInputDefinition()
        {
            Name = "input2",
            Node = this,
            Type = NodeConnectionType.SingleChannelAudio
        }
    };

    [JsonIgnore]
    public IEnumerable<NodeOutputDefinition> Outputs => new List<NodeOutputDefinition>()
    {
        new NodeOutputDefinition()
        {
            Name = "main",
            Node = this,
            Type = NodeConnectionType.SingleChannelAudio
        }
    };

    public Task<Dictionary<string, object>> Render(RenderingChunk chunk, Dictionary<string, object> inputs)
    {
        var output = new SingleChannelAudioBuffer(chunk.Process.SampleRate, chunk.Length);
        foreach (var input in inputs.Values)
        {
            if (input is SingleChannelAudioBuffer)
            {
                var inputSpan = (input as SingleChannelAudioBuffer).AsSpan;
                var outputSpan = output.AsSpan;

                for (var i = 0; i + Vector<float>.Count <= chunk.Length; i += Vector<float>.Count)
                {
                    var result = new Vector<float>(inputSpan.Slice(i, Vector<float>.Count)) +new Vector<float>(outputSpan.Slice(i, Vector<float>.Count)) ;
                    result.CopyTo(outputSpan.Slice(i, Vector<float>.Count));
                }

                for (var i = 0; i < chunk.Length; i++)
                {
                    outputSpan[i] += inputSpan[i];
                }
            }
        }

        return Task.FromResult(new Dictionary<string, object>() { { "output", output } });
    }

    public string Title => "Mix";
}
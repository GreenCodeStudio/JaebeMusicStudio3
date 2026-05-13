using System.Numerics;
using System.Text.Json.Serialization;
using JaebeMusicStudio3.Core.AudioRendering;

namespace JaebeMusicStudio3.Core.AudioNodes;

public class OverdriveNode : IAudioNode
{
    public Guid Id { get;set; }=Guid.NewGuid();
    [JsonIgnore]
    public IEnumerable<NodeInputDefinition> Inputs => new List<NodeInputDefinition>()
    {
        new NodeInputDefinition()
        {
            Name = "input",
            Node = this,
            Type = NodeConnectionType.SingleChannelAudio
        }
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

    public float Volume { get; set; } = 0.5f;
    public float Limit { get; set; } = 1f;

    public Task<Dictionary<string, object>> Render(RenderingChunk chunk, Dictionary<string, object> inputs)
    {
        var volume = Volume;
        var input = inputs.ContainsKey("input") ? inputs["input"] as SingleChannelAudioBuffer : null;
        var output = new SingleChannelAudioBuffer(chunk.Process.SampleRate, chunk.Length);
        if (input != null)
        {
            var inputSpan = input.AsSpan;
            var outputSpan = output.AsSpan;
            var limit = Limit;
            for (var i = 0; i + Vector<float>.Count <= chunk.Length; i += Vector<float>.Count)
            {
                var result = new Vector<float>(inputSpan.Slice(i, Vector<float>.Count)) * volume;
                result=Vector.Max(result, new Vector<float>(-limit));
                result=Vector.Min(result, new Vector<float>(limit));
                result.CopyTo(outputSpan.Slice(i, Vector<float>.Count));
            }

            for (var i = 0; i < chunk.Length; i++)
            {
                outputSpan[i] = MathF.Max(MathF.Min(inputSpan[i] * volume, limit),-limit);
                
            }
        }

        return Task.FromResult(new Dictionary<string, object>() { { "output", output } });
    }

    public string Title => "Overdrive";
}
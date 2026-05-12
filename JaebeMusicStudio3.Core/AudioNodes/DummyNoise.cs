using System.Text.Json.Serialization;
using JaebeMusicStudio3.Core.AudioRendering;

namespace JaebeMusicStudio3.Core.AudioNodes;

public class DummyNoise : IAudioNode
{
    public Guid Id { get; }=Guid.NewGuid();
    [JsonIgnore]
    public IEnumerable<NodeInputDefinition> Inputs => new List<NodeInputDefinition>();

    [JsonIgnore]
    public IEnumerable<NodeOutputDefinition> Outputs => new List<NodeOutputDefinition>()
    {
        new NodeOutputDefinition()
        {
            Type = NodeConnectionType.SingleChannelAudio,
            Node = this,
            Name = "main"
        }
    };

    public async Task<Dictionary<string, object>> Render(RenderingChunk chunk, Dictionary<string, object> inputs)
    {
        var buffer = new SingleChannelAudioBuffer(chunk.Process.SampleRate, chunk.Length);
        var random = new Random();
        for (var i = 0; i < chunk.Length; i++)
        {
            // var x = (chunk.Start + i) % 256;
            // buffer.Data[i] = (x<128?1:-1) * 0.01f;
            var x = (chunk.Start + i) % 256;
            buffer.Data[i] = MathF.Sin((float)x / 256f * 2f * MathF.PI) * 0.1f;
        }

        return new Dictionary<string, object>() { { "main", buffer } };
    }
    
    public string Title => "Dummy Noise";
}
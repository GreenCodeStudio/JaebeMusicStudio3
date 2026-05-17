using System.Text.Json.Serialization;
using JaebeMusicStudio3.Core.AudioRendering;

namespace JaebeMusicStudio3.Core.AudioNodes;

public class Noise : IAudioNode
{
    public Guid Id { get; set; }=Guid.NewGuid();
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
            buffer.Data[i] = random.NextSingle() * 2 - 1;
        }

        return new Dictionary<string, object>() { { "main", buffer } };
    }
    
    public string Title => "Dummy Noise";
    
    public string Name { get; set; }
}
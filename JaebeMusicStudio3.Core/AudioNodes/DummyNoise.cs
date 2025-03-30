using JaebeMusicStudio3.Core.AudioRendering;

namespace JaebeMusicStudio3.Core.AudioNodes;

public class DummyNoise : IAudioNode
{
    public Dictionary<string, NodeInputDefinition> Inputs => new Dictionary<string, NodeInputDefinition>();

    public Dictionary<string, NodeOutputDefinition> Outputs => new Dictionary<string, NodeOutputDefinition>()
    {
        {
            "main", new NodeOutputDefinition()
            {
                Type = NodeConnectionType.SingleChannelAudio,
                Node = this,
                Name="main"
            }
        }
    };

    public async Task<Dictionary<string, object>> Render(RenderingChunk chunk)
    {
        var buffer = new SingleChannelAudioBuffer(chunk.Process.SampleRate, chunk.Length);
        var random = new Random();
        for (var i = 0; i < chunk.Length; i++)
        {
            buffer.Data[i] = random.NextSingle() * 0.1f;
        }

        return new Dictionary<string, object>() { { "main", buffer } };
    }
}
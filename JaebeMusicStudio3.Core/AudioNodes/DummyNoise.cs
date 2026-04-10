using JaebeMusicStudio3.Core.AudioRendering;

namespace JaebeMusicStudio3.Core.AudioNodes;

public class DummyNoise : IAudioNode
{
    public IEnumerable<NodeInputDefinition> Inputs => new List<NodeInputDefinition>();

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
            if(i%100==0)
            Console.WriteLine(chunk.Start+ i);
            buffer.Data[i] = (float)Math.Sin((chunk.Start + i) / 100.0) * 0.1f;
        }

        return new Dictionary<string, object>() { { "main", buffer } };
    }
}
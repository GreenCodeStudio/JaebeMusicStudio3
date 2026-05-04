using System.Numerics;
using JaebeMusicStudio3.Core.AudioRendering;

namespace JaebeMusicStudio3.Core.AudioNodes;

public class VolumeNode : IAudioNode
{
    public IEnumerable<NodeInputDefinition> Inputs => new List<NodeInputDefinition>()
    {
        new NodeInputDefinition()
        {
            Name = "input",
            Node = this,
            Type = NodeConnectionType.SingleChannelAudio
        }
    };

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

    public Task<Dictionary<string, object>> Render(RenderingChunk chunk, Dictionary<string, object> inputs)
    {
        var volume = Volume;
        var input = inputs.ContainsKey("input") ? inputs["input"] as SingleChannelAudioBuffer : null;
        var output = new SingleChannelAudioBuffer(chunk.Process.SampleRate, chunk.Length);
        if (input != null)
        {
            var inputSpan = input.AsSpan;
            var outputSpan = output.AsSpan;

            for (var i = 0; i + Vector<float>.Count <= chunk.Length; i += Vector<float>.Count)
            {
                var result = new Vector<float>(inputSpan.Slice(i, Vector<float>.Count)) * volume;
                result.CopyTo(outputSpan.Slice(i, Vector<float>.Count));
            }

            for (var i = 0; i < chunk.Length; i++)
            {
                outputSpan[i] = inputSpan[i] * volume;
            }
        }
        
        
        //tmp start
        // var filledLength = (int)Math.Pow(2, Math.Ceiling(Math.Log2(output.Data.Length)));
        // var filled=new float[filledLength];
        // output.AsSpan.CopyTo(filled);
        // var fft=FFT.Execute(filled);
        //
        // for (int i = 0; i < output.Data.Length; i++)
        // {
        //     var sample = 0f;
        //     var iFloat=1-(float)i/filledLength;
        //     for(var j=0;j<filledLength;j++)
        //     {
        //         sample+=MathF.Sin(2*MathF.PI*iFloat*j)*fft.imaginary[j]+MathF.Cos(2*MathF.PI*iFloat*j)*fft.real[j];
        //     }
        //     output.Data[i] = sample/filledLength;
        // }
        //
        //
        //tmp end

        return Task.FromResult(new Dictionary<string, object>() { { "output", output } });
    }

    public string Title => "Volume";
}
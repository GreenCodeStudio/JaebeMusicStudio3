using System.Numerics;
using JaebeMusicStudio3.Core.AudioNodes;
using JaebeMusicStudio3.Core.AudioRendering;
using NAudio.Wave;

namespace JaebeMusicStudio3.Core.Timeline;

public class RecordedSound : ITimelineItem, IAudioNode
{
    public Guid Id { get; set;}=Guid.NewGuid();
    public WaveFormat WaveFormat { get; set; }

    public float[] Samples
    {
        get;
        set
        {
            field = value;
            Changed?.Invoke();
        }
    }

    public double LengthSeconds => (WaveFormat != null) ? ((Samples?.Length ?? 0) / (double)WaveFormat.SampleRate) : 0;

    public double OffsetSeconds
    {
        get;
        set
        {
            field = value;
            Changed?.Invoke();
        }
    }

    public IEnumerable<NodeInputDefinition> Inputs => new List<NodeInputDefinition>()
    {
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


    public Task<Dictionary<string, object>> Render(RenderingChunk chunk, Dictionary<string, object> inputs)
    {
        var output = new SingleChannelAudioBuffer(chunk.Process.SampleRate, chunk.Length);
        var outputSpan = output.AsSpan;
        if (Samples != null && Samples.Length > chunk.Start && chunk.Process.UseTimeline)
        {
            var slice = new Span<float>(Samples).Slice((int)chunk.Start);
            if (slice.Length > chunk.Length)
                slice = slice.Slice(0, (int)chunk.Length);
            slice.CopyTo(outputSpan); //todo recordings over 12h (in 48khz)
        }

        return Task.FromResult(new Dictionary<string, object>() { { "output", output } });
    }

    public string Title => "Recorded Sound";
    public event Action Changed;
}
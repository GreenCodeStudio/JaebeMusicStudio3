using System.Text.Json.Serialization;
using JaebeMusicStudio3.Core.AudioRendering;
using JaebeMusicStudio3.Core.Timeline;

namespace JaebeMusicStudio3.Core.AudioNodes;

public class BasicOscillatorNode : IAudioNode
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [JsonIgnore]
    public IEnumerable<NodeInputDefinition> Inputs => new List<NodeInputDefinition>()
    {
        new NodeInputDefinition()
        {
            Name = "note",
            Node = this,
            Type = NodeConnectionType.Note
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

    public Task<Dictionary<string, object>> Render(RenderingChunk chunk, Dictionary<string, object> inputs)
    {
        var output = new SingleChannelAudioBuffer(chunk.Process.SampleRate, chunk.Length);
        if (inputs.TryGetValue("note", out var note))
        {
            var noteNote = note as Note;
            for (var i = 0; i < chunk.Length; i++)
            {
                var secondsOffset = ((float)i+chunk.Start) / chunk.Process.SampleRate;
                output.Data[i] += MathF.Sin(secondsOffset * (float)noteNote.Pitch * 2 * MathF.PI) * 0.1f;
            }
        }

        return Task.FromResult(new Dictionary<string, object>()
        {
            {
                "output", output
            }
        });
    }

    public string Title => "Basic Oscillator";
}
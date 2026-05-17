using System.Text.Json.Serialization;
using JaebeMusicStudio3.Core.AudioRendering;
using JaebeMusicStudio3.Core.Timeline;

namespace JaebeMusicStudio3.Core.AudioNodes;

public class NoteGateNode : IAudioNode
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public double DesiredPitch { get; set; } = 261.62554931640625;
    public double OutputPitch { get; set; } = 100;

    [JsonIgnore]
    public IEnumerable<NodeInputDefinition> Inputs => new List<NodeInputDefinition>()
    {
        new NodeInputDefinition()
        {
            Name = "input",
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
            Type = NodeConnectionType.Note
        }
    };

    public Task<Dictionary<string, object>> Render(RenderingChunk chunk, Dictionary<string, object> inputs)
    {
        var input = inputs["input"] as Note;
        if (input != null && Math.Abs(Math.Log2(input.Pitch) - Math.Log2(DesiredPitch)) < (0.5 / 12))
        {
            var output = new Note()
            {
                Length = input.Length,
                Start = input.Start,
                Pitch = OutputPitch,
                Volume = input.Volume
            };

            return Task.FromResult(new Dictionary<string, object>() { { "output", output } });
        }
        else
        {
            return Task.FromResult(new Dictionary<string, object>() { { "output", null } });
        }
    }

    public string Title => "NoteMultiplication";

    public string Name { get; set; }
}
using System.Numerics;
using System.Text.Json.Serialization;
using JaebeMusicStudio3.Core.AudioRendering;
using JaebeMusicStudio3.Core.Timeline;

namespace JaebeMusicStudio3.Core.AudioNodes;

public class NoteModificationNode:IAudioNode
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public double Multipler { get; set; } = 1;

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
        var output = new Note()
        {
            Length = input.Length,
            Start = input.Start,
            Pitch = input.Pitch * Multipler,
            Volume=input.Volume
        };

        return Task.FromResult(new Dictionary<string, object>() { { "output", output } });
    }

    public string Title => "NoteMultiplication";
}
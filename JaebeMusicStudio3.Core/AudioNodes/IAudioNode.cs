using System.Text.Json.Serialization;
using JaebeMusicStudio3.Core.AudioRendering;
using JaebeMusicStudio3.Core.Timeline;

namespace JaebeMusicStudio3.Core.AudioNodes;
[JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
[JsonDerivedType(typeof(Noise), typeDiscriminator: "Noise")]
[JsonDerivedType(typeof(Instrument), typeDiscriminator: "Instrument")]
[JsonDerivedType(typeof(LiveAudioInput), typeDiscriminator: "LiveAudioInput")]
[JsonDerivedType(typeof(MixNode), typeDiscriminator: "MixNode")]
[JsonDerivedType(typeof(OverdriveNode), typeDiscriminator: "OverdriveNode")]
[JsonDerivedType(typeof(VolumeNode), typeDiscriminator: "VolumeNode")]
[JsonDerivedType(typeof(NoteModificationNode), typeDiscriminator: "NoteModificationNode")]
[JsonDerivedType(typeof(NoteGateNode), typeDiscriminator: "NoteGateNode")]
[JsonDerivedType(typeof(BasicOscillatorNode), typeDiscriminator: "BasicOscillatorNode")]
[JsonDerivedType(typeof(RecordedSound), typeDiscriminator: "RecordedSound")]

public interface IAudioNode
{
    Guid Id { get; set; }
    [JsonIgnore]
    IEnumerable<NodeInputDefinition> Inputs { get; }
    
    [JsonIgnore]
    IEnumerable<NodeOutputDefinition> Outputs { get; }
    Task<Dictionary<string, object>> Render(RenderingChunk chunk, Dictionary<string, object> inputs);
    /// <summary>
    /// Type of node
    /// </summary>
    string Title { get; }
    /// <summary>
    /// Defined by user
    /// </summary>
    string Name { get; set; }
}
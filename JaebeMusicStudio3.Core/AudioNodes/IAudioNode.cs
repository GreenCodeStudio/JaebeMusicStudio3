using System.Text.Json.Serialization;
using JaebeMusicStudio3.Core.AudioRendering;

namespace JaebeMusicStudio3.Core.AudioNodes;
[JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
[JsonDerivedType(typeof(DummyNoise), typeDiscriminator: "DummyNoise")]
[JsonDerivedType(typeof(Instrument), typeDiscriminator: "Instrument")]
[JsonDerivedType(typeof(LiveAudioInput), typeDiscriminator: "LiveAudioInput")]
[JsonDerivedType(typeof(MixNode), typeDiscriminator: "MixNode")]
[JsonDerivedType(typeof(OverdriveNode), typeDiscriminator: "OverdriveNode")]
[JsonDerivedType(typeof(VolumeNode), typeDiscriminator: "VolumeNode")]

public interface IAudioNode
{
    Guid Id { get; set; }
    [JsonIgnore]
    IEnumerable<NodeInputDefinition> Inputs { get; }
    
    [JsonIgnore]
    IEnumerable<NodeOutputDefinition> Outputs { get; }
    Task<Dictionary<string, object>> Render(RenderingChunk chunk, Dictionary<string, object> inputs);
    string Title { get; }
}
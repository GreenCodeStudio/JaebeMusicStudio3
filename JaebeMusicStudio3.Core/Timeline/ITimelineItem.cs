using System.Text.Json.Serialization;

namespace JaebeMusicStudio3.Core.Timeline;
[JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
[JsonDerivedType(typeof(NoteLine), typeDiscriminator: "NoteLine")]
[JsonDerivedType(typeof(RecordedSound), typeDiscriminator: "RecordedSound")]
[JsonDerivedType(typeof(RecordedSoundInProgress), typeDiscriminator: "RecordedSoundInProgress")]
public interface ITimelineItem
{
    event Action Changed;
    public double LengthSeconds { get; }
    public double OffsetSeconds { get; set; }
}
namespace JaebeMusicStudio3.Core.Timeline;

public interface ITimelineItem
{
    event Action Changed;
    public double LengthSeconds { get; }
    public double OffsetSeconds { get; }
}
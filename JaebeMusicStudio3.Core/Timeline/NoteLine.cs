

using JaebeMusicStudio3.Core.AudioNodes;

namespace JaebeMusicStudio3.Core.Timeline;

public class NoteLine : ITimelineItem
{
    public List<Note> Notes { get; set; } = new List<Note>();
    public Instrument Instrument { get; set; }
    public double LengthSeconds => Notes.Count > 0 ? Notes.Max(n => n.Start + n.Length) : 0;
    public double OffsetSeconds { get; set; }
    public double Tempo { get; set; } = 120;
    public event Action? Changed;
}
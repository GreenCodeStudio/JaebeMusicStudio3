

using System.Text.Json.Serialization;
using JaebeMusicStudio3.Core.AudioNodes;

namespace JaebeMusicStudio3.Core.Timeline;

public class NoteLine : ITimelineItem
{
    public List<Note> Notes { get; set; } = new List<Note>();
    [JsonIgnore]
    public Instrument Instrument { get; set; }
    public double LengthSeconds => (Notes.Count > 0 ? Notes.Max(n => n.Start + n.Length) : 0)*60/Tempo;

    public double OffsetSeconds
    {
        get;
        set
        {
            field = value;
            Changed?.Invoke();
        }
    }

    public double Tempo { get; set; } = 120;
    public event Action? Changed;

    public void InvokeChanged()
    {
       Changed?.Invoke();
    }
}
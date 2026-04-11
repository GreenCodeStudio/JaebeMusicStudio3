using NAudio.Wave;

namespace JaebeMusicStudio3.Core.Timeline;

public class RecordedSound:ITimelineItem
{
    public WaveFormat WaveFormat { get; set; }
    public float[] Samples { get; set; }

    public double LengthSeconds => (WaveFormat != null) ? ((Samples?.Length ?? 0) / (double)WaveFormat.SampleRate) : 0;
}
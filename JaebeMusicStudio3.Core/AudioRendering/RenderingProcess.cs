using NAudio.Wave;

namespace JaebeMusicStudio3.Core.AudioRendering;

public class RenderingProcess(bool useTimeline, WaveFormat waveFormat)
{
    public static RenderingProcess Current = new RenderingProcess(false, new WaveFormat(48000, 16, 2));
    public WaveFormat WaveFormat { get; set; } = waveFormat;
    public int SampleRate => this.WaveFormat.SampleRate;
    public long Position { get; private set; } = 0;
    public bool UseTimeline => useTimeline;

    public RenderingChunk GetChunk(long length)
    {
        lock (this)
        {
            var ret = new RenderingChunk() { Process = this, Start = Position, Length = length };
            Position += length;
            return ret;
        }
    }
}
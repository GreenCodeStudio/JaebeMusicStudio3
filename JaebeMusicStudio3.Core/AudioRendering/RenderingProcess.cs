namespace JaebeMusicStudio3.Core.AudioRendering;

public class RenderingProcess(bool useTimeline)
{
    public static RenderingProcess Current = new RenderingProcess(false);
    public int SampleRate { get; private set; } = 48000;
    public long Position { get; private set; } = 0;

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
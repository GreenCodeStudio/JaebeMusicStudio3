namespace JaebeMusicStudio3.Core.AudioRendering;

public class SingleChannelRollingAudioBuffer
{
    public List<SingleChannelAudioBuffer> Buffers { get; } = new List<SingleChannelAudioBuffer>();
    public long SampleTotalCapacity { get; set; }
    public long Size => Buffers.Sum(b => b.Data.Length);
    public long? SampleRate => Buffers.FirstOrDefault()?.SampleRate;
    public IEnumerable<float> Samples => Buffers.SelectMany(b => b.Data);

    public void Add(SingleChannelAudioBuffer? singleBuffer)
    {
        Buffers.Add(singleBuffer);
        while (Size - Buffers[0].Data.Length >= SampleTotalCapacity)
        {
            Buffers.RemoveAt(0);
        }
    }
}
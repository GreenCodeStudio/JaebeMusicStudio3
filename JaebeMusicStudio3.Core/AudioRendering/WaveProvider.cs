using JaebeMusicStudio3.Core.Mixer;
using NAudio.Wave;

namespace JaebeMusicStudio3.Core.AudioRendering;

public class WaveProvider(AudioMixer mixer) : IWaveProvider
{
    public static event Action<RenderingChunk> ChunkCreated;
    public AudioMixer Mixer { get; set; } = mixer;

    public int Read(byte[] buffer, int offset, int count)
    {
        var span = new Span<byte>(buffer, offset, count);

        var samples = count * 8 / WaveFormat.BitsPerSample / WaveFormat.Channels;
        var chunk = RenderingProcess.Current.GetChunk(samples);
        var localMixer = Mixer; //in case changed during renderign
        localMixer.Render(chunk);
        ChunkCreated?.Invoke(chunk);

        var response = chunk.GetResponse(localMixer.MainOutput).Result;

        if (response is SingleChannelAudioBuffer)
        {
            (response as SingleChannelAudioBuffer).WriteToRaw(WaveFormat, span);
        }

        return count;
    }

    public WaveFormat WaveFormat { get; set; }
}
using NAudio.Wave;

namespace JaebeMusicStudio3.Core.AudioRendering;

public class WaveProvider(RenderingProcess process, Mixer.Mixer mixer) :IWaveProvider
{
    public int Read(byte[] buffer, int offset, int count)
    {
        var span=new Span<byte>(buffer,offset,count);
        
        var samples = count * 8 / WaveFormat.BitsPerSample;
        var chunk = process.GetChunk(samples);

        mixer.Render(chunk);

        var response=chunk.GetResponse(mixer.MainOutput).Result;

        if (response is SingleChannelAudioBuffer)
        {
            (response as SingleChannelAudioBuffer).WriteToRaw(WaveFormat, span);
        }
        return count;
    }

    public WaveFormat WaveFormat { get; set; }
}
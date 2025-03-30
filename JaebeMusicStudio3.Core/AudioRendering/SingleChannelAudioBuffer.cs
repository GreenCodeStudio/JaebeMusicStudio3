using System.Runtime.InteropServices;
using NAudio.Wave;

namespace JaebeMusicStudio3.Core.AudioRendering;

public class SingleChannelAudioBuffer
{
    public SingleChannelAudioBuffer(int sampleRate, long chunkLength)
    {
        SampleRate = sampleRate;
        Data=new float[chunkLength];
    }

    public int SampleRate { get; set; }
    public float[] Data { get; set; }

    public void WriteToRaw(WaveFormat waveFormat, Span<byte> span)
    {
        //todo hevy optimization
        if(SampleRate != waveFormat.SampleRate)
            throw new Exception("SampleRate must be " + waveFormat.SampleRate);
        
        var bits=waveFormat.BitsPerSample/waveFormat.Channels;
        if (waveFormat.BitsPerSample == 8)
        {
            var subspan = MemoryMarshal.Cast<byte, sbyte>(span);
            var j = 0;
            for (var i = 0; i < Data.Length; i++)
            {
                for (var ch = 0; ch < waveFormat.Channels; ch++)
                {
                    subspan[j] = (sbyte)(Data[i] * 0x7f);
                    j++;
                    if (j >= subspan.Length)
                        break;
                }
            }
        } else if (waveFormat.BitsPerSample == 16)
        {
            var subspan = MemoryMarshal.Cast<byte, short>(span);
            var j = 0;
            for (var i = 0; i < Data.Length; i++)
            {
                for (var ch = 0; ch < waveFormat.Channels; ch++)
                {
                    subspan[j] = (short)(Data[i] * 0x7fff);
                    j++;
                    if (j >= subspan.Length)
                        return;
                }
            }
        }
        else
        {
            throw new Exception("SampleRate not implemented");
        }
    }
}
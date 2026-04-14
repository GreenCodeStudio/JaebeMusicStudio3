using System.Runtime.InteropServices;
using NAudio.Wave;

namespace JaebeMusicStudio3.Core.Utils;

public class BinaryConverter
{
    public static float[] FromBinary(WaveFormat waveFormat, Span<byte> span)
    {
        var samples = new float[span.Length / (waveFormat.BitsPerSample / 8)];
        if (waveFormat.Encoding == WaveFormatEncoding.Pcm)
        {
            if (waveFormat.BitsPerSample == 8)
            {
                for (int i = 0; i < samples.Length; i++)
                {
                    samples[i] = (span[i] - 128) / 128f;
                }
            }
            else if (waveFormat.BitsPerSample == 16)
            {
                var span16 = MemoryMarshal.Cast<byte, short>(span);
                for (int i = 0; i < samples.Length; i++)
                {
                    samples[i] = (span16[i]) / (float)0x7fff;
                }
            }
            else
            {
                throw new NotSupportedException("Only PCM audio samples are supported.");
            }
        }
        else if (waveFormat.Encoding == WaveFormatEncoding.IeeeFloat)
        {
            if (waveFormat.BitsPerSample == 32)
            {
                var span32 = MemoryMarshal.Cast<byte, float>(span);
                for (int i = 0; i < samples.Length; i++)
                {
                    samples[i] = (span32[i]);
                }
            }
            else
            {
                throw new NotSupportedException("Only PCM audio samples are supported.");
            }
        }
        else
        {
            throw new NotSupportedException("Only PCM encoding is supported");
        }

        return samples;
    }
}
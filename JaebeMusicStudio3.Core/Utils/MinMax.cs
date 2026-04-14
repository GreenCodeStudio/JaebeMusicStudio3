using System.Numerics;

namespace JaebeMusicStudio3.Core.Utils;

public class MinMax
{
    public static (float[] min, float[] max) DivideByPow2(float[] samples, int divider)
    {
        var min = new float[samples.Length / divider];
        var max = new float[samples.Length / divider];
        for (var i = 0; i < samples.Length / divider; i++)
        {
            var minValue = float.MaxValue;
            var maxValue = float.MinValue;
            for (var j = 0; j < divider; j++)
            {
                var sample = samples[i * divider + j];
                if (sample < minValue) minValue = sample;
                if (sample > maxValue) maxValue = sample;
            }

            min[i] = minValue;
            max[i] = maxValue;
        }

        return (min, max);
    }

    public static (float average, float max) SingleAbsAvgMaxFast(Span<float> responseTypedAsSpan)
    {
        var averageSum = 0f;
        var maxVector = new Vector<float>();
        for (var i = 0; i + Vector<float>.Count <= responseTypedAsSpan.Length; i += Vector<float>.Count)
        {
            var vector = new Vector<float>(responseTypedAsSpan.Slice(i));
            var absVector = Vector.Abs(vector);
            averageSum += Vector.Sum(absVector);
            maxVector = Vector.Max(maxVector, absVector);
        }

        //in case something is left, ignore due to performace
        
        var max = 0f;
        for (var i = 0; i < Vector<float>.Count; i++)
        {
            if (maxVector[i] > max) max = maxVector[i];
        }

        return (averageSum / responseTypedAsSpan.Length, max);
    }
}
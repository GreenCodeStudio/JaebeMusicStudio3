using System.Windows;
using System.Windows.Shapes;
using JaebeMusicStudio3.Core.Utils;

namespace JaebeMusicStudio3.Front.Utils;

public static class GraphDrawer
{
    public static Polyline GenerateSamplesGraph(long bufferSampleRate, IEnumerable<float> bufferSamples,
        double secondsPerPixel, double heightPixels)
    {
        var halfHeight = heightPixels / 2;
        var samplesPerPixel = bufferSampleRate * secondsPerPixel;
        var line = new Polyline();
        line.Stroke = System.Windows.Media.Brushes.Green;

        var dividerInt = (int)Math.Pow(2, Math.Round(Math.Log2(samplesPerPixel)));
        if (dividerInt >= 4)
        {
            line.Fill = System.Windows.Media.Brushes.Green;
            float[] samples;

            samples = bufferSamples.ToArray();


            var (min, max) = MinMax.DivideByPow2(samples, dividerInt);
            for (var i = 0; i < min.Length; i++)
            {
                line.Points.Add(new Point((i / samplesPerPixel*dividerInt),
                    min[i] * halfHeight + halfHeight));
            }

            for (var i = max.Length - 1; i >= 0; i--)
            {
                line.Points.Add(new Point((i / samplesPerPixel*dividerInt),
                    max[i] * halfHeight + halfHeight));
            }
        }
        else
        {
            line.Fill = null;
            var i = 0;

            foreach (var sample in bufferSamples)
            {
                line.Points.Add(new Point((i / samplesPerPixel),
                    sample * halfHeight + halfHeight));
                i++;
            }
        }

        return line;
    }
}
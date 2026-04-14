using System.Numerics;

namespace Optimalization.Fourier;

public static class FFT
{
    public static (float[] real, float[] imaginary) Execute(float[] input)
    {
        int n = input.Length;
        if ((n & (n - 1)) != 0)
            throw new ArgumentException("Długość musi być potęgą dwójki");

        byte bits = (byte)Math.Log2(n);

        var bufferR = new float[n];
        var bufferI = new float[n];
        for (var i = 0; i < n; i++)
        {
            bufferR[i] = input[i];
            bufferI[i] = 0;
        }

        for (uint i = 0; i < n; i++)
        {
            uint j = BitReverse(i, bits);
            if (j > i)
            {
                var tempR = bufferR[i];
                var tempI = bufferI[i];
                bufferR[i] = bufferR[j];
                bufferI[i] = bufferI[j];
                bufferR[j] = tempR;
                bufferI[j] = tempI;
            }
        }

        for (int len = 2; len <= n; len <<= 1)
        {
            float angle = -2 * MathF.PI / len;
            var wLenR = MathF.Cos(angle);
            var wLenI = MathF.Sin(angle);
            for (int i = 0; i < n; i += len)
            {
                var wR = 1.0f;
                var wI = 0.0f;
                for (int j = 0; j < len / 2; j++)
                {
                    var uR = bufferR[(i + j)];
                    var uI = bufferI[(i + j)];
                    var xR = bufferR[(i + j + len / 2)];
                    var xI = bufferI[(i + j + len / 2)];
                    var vR = wR * xR - wI * xI;
                    var vI = wR * xI + wI * xR;
                    bufferR[(i + j)] = uR + vR;
                    bufferI[(i + j)] = uI + vI;
                    bufferR[(i + j + len / 2)] = uR - vR;
                    bufferI[(i + j + len / 2)] = uI - vI;
                    var wR2 = wR * wLenR - wI * wLenI;
                    var wI2 = wR * wLenI + wI * wLenR;
                    wR = wR2;
                    wI = wI2;
                }
            }
        }

        return (bufferR, bufferI);
    }

    private static uint BitReverse(uint x, byte bits)
    {
        x = ((x >> 1) & 0x55555555) | ((x & 0x55555555) << 1);
        x = ((x >> 2) & 0x33333333) | ((x & 0x33333333) << 2);
        x = ((x >> 4) & 0x0F0F0F0F) | ((x & 0x0F0F0F0F) << 4);
        x = ((x >> 8) & 0x00FF00FF) | ((x & 0x00FF00FF) << 8);
        x = (x >> 16) | (x << 16);
        x = x >> (32 - bits);
        return x;
    }

    public static IEnumerable<float> GetModulos(float[] real, float[] imaginary)
    {
        var realSpan = real.AsSpan();
        var imaginarySpan = imaginary.AsSpan();
        var ret = new float[real.Length];
        var retSpan = new Span<float>(ret);
        var i = 0;
        for(;i+Vector<float>.Count<=real.Length;i+=Vector<float>.Count)
        {
            var r = new Vector<float>(realSpan.Slice(i));
            var im = new Vector<float>(imaginarySpan.Slice(i));
            var modulo = Vector.SquareRoot(r * r + im * im);
            modulo.CopyTo(retSpan.Slice(i));
        }

        for (;i<real.Length;i++)
        {
            var modulo=MathF.Sqrt(real[i] * real[i] + imaginary[i] * imaginary[i]);
            ret[i] = modulo;
        }
        return ret;
    }
}
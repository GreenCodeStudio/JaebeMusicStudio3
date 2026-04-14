using System.DirectoryServices.ActiveDirectory;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using JaebeMusicStudio3.Core.AudioNodes;
using JaebeMusicStudio3.Core.AudioRendering;
using Optimalization.Fourier;

namespace JaebeMusicStudio3.Front.MixerGui;

public partial class SoundAnalyzer : UserControl, IDisposable
{
    private SingleChannelRollingAudioBuffer Buffer = new SingleChannelRollingAudioBuffer();

    public SoundAnalyzer(NodeOutputDefinition nodeOutputDefinition)
    {
        this.NodeOutputDefinition = nodeOutputDefinition;
        InitializeComponent();
        WaveProvider.ChunkCreated += ChunkCreated;
        CompositionTarget.Rendering += (s, e) => { Render(); };
    }

    public NodeOutputDefinition NodeOutputDefinition { get; set; }

    private void Render()
    {
        OsciloscopeCanvas.Children.Clear();
        if (OsciloscopeRadio.IsChecked == true)
        {
            var verticalSize = VerticalSize.Value;
            var width = OsciloscopeCanvas.ActualWidth;
            var halfHeight = OsciloscopeCanvas.ActualHeight / 2;
            var line = new Polyline();
            line.Stroke = System.Windows.Media.Brushes.Green;
            OsciloscopeCanvas.Children.Add(line);
            if (Buffer.SampleRate != null)
            {
                var horizontalSize = HorizontalSize.Value;
                var samplesPerWidth = (double)_horizontalSizeInSamples;
                var i = 0;
                foreach (var sample in Buffer.Samples.Reverse())
                {
                    line.Points.Add(new Point((1 - i / samplesPerWidth) * width,
                        sample * verticalSize * halfHeight + halfHeight));
                    i++;
                    if (i > samplesPerWidth) break;
                }
            }
        }
        else if (FttRadio.IsChecked == true)
        {
            var verticalSize = VerticalSize.Value;
            var width = OsciloscopeCanvas.ActualWidth;
            var height = OsciloscopeCanvas.ActualHeight;
            var line = new Polyline();
            line.Stroke = System.Windows.Media.Brushes.Blue;
            OsciloscopeCanvas.Children.Add(line);
            var samples = Buffer.Samples.Reverse().Take(_horizontalSizeInSamplesRounded);
            if (samples.Count() != _horizontalSizeInSamplesRounded)
            {
                return;
            }

            var fft = FFT.Execute(samples.ToArray());
            var fftModulos=FFT.GetModulos(fft.real, fft.imaginary);
            if (Buffer.SampleRate != null)
            {
                var horizontalSize = HorizontalSize.Value;
                var samplesPerWidth = _horizontalSizeInSamplesRounded*1.0;
                var i = 0;
                foreach (var sample in fftModulos)
                {
                    line.Points.Add(new Point((i / samplesPerWidth) * width,
                        sample * verticalSize * height * -1d + height));
                    i++;
                    if (i > samplesPerWidth) break;
                }
            }
        }
    }

    int _horizontalSizeInSamples
    {
        get
        {
            if (Buffer.SampleRate == null)
                return 1;
            var horizontalSize = HorizontalSize.Value;
            var samplesPerWidth = Buffer.SampleRate.Value / horizontalSize;
            return (int)samplesPerWidth;
        }
    }

    int _horizontalSizeInSamplesRounded
    {
        get
        {
            var log = Math.Log(_horizontalSizeInSamples) / Math.Log(2);
            var logRounded = Math.Ceiling(log);
            return (int)Math.Pow(2, logRounded);
        }
    }

    private void ChunkCreated(RenderingChunk chunk)
    {
        var response = chunk.GetResponse(NodeOutputDefinition).Result;
        Dispatcher.Invoke(() =>
        {
            var horizontalSize = HorizontalSize.Value;
            if (response is SingleChannelAudioBuffer)
            {
                var singleBuffer = response as SingleChannelAudioBuffer;
                Buffer.SampleTotalCapacity = _horizontalSizeInSamplesRounded;

                Buffer.Add(singleBuffer);
            }
        });
    }

    public void Dispose()
    {
        WaveProvider.ChunkCreated -= ChunkCreated;
    }
}
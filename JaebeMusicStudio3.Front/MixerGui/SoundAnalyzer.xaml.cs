using System.DirectoryServices.ActiveDirectory;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using JaebeMusicStudio3.Core.AudioNodes;
using JaebeMusicStudio3.Core.AudioRendering;

namespace JaebeMusicStudio3.Front.MixerGui;

public partial class SoundAnalyzer : UserControl, IDisposable
{
    private SingleChannelRollingAudioBuffer Buffer = new SingleChannelRollingAudioBuffer();

    public SoundAnalyzer(NodeOutputDefinition nodeOutputDefinition)
    {
        this.NodeOutputDefinition = nodeOutputDefinition;
        InitializeComponent();
        WaveProvider.ChunkCreated += ChunkCreated;
        CompositionTarget.Rendering += (s, e) =>
        {
            Render();
        };

    }

    public NodeOutputDefinition NodeOutputDefinition { get; set; }

    private void Render()
    {
        var verticalSize = VerticalSize.Value;
        var width = OsciloscopeCanvas.ActualWidth;
        var halfHeight = OsciloscopeCanvas.ActualHeight / 2;
        OsciloscopeCanvas.Children.Clear();
        var line = new Polyline();
        line.Stroke = System.Windows.Media.Brushes.Green;
        OsciloscopeCanvas.Children.Add(line);
        if (Buffer.SampleRate != null)
        {
            var horizontalSize = HorizontalSize.Value;
            var samplesPerWidth = Buffer.SampleRate.Value / horizontalSize;
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
    private void ChunkCreated(RenderingChunk chunk)
    {
        var response = chunk.GetResponse(NodeOutputDefinition).Result;
        Dispatcher.Invoke(() =>
        {
            var horizontalSize = HorizontalSize.Value;
            if (response is SingleChannelAudioBuffer)
            {
                var singleBuffer = response as SingleChannelAudioBuffer;
                var samplesPerWidth = (response as SingleChannelAudioBuffer).SampleRate / horizontalSize;
                Buffer.SampleTotalCapacity = (long)samplesPerWidth;
                Buffer.Add(singleBuffer);
               
            }
        });
    }

    public void Dispose()
    {
        WaveProvider.ChunkCreated -= ChunkCreated;
    }
}
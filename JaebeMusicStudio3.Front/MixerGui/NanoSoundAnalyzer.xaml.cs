using System.Windows.Controls;
using System.Windows.Media;
using JaebeMusicStudio3.Core.AudioNodes;
using JaebeMusicStudio3.Core.AudioRendering;
using JaebeMusicStudio3.Core.Utils;

namespace JaebeMusicStudio3.Front.MixerGui;

public partial class NanoSoundAnalyzer : UserControl
{
    public NanoSoundAnalyzer(NodeOutputDefinition nodeOutputDefinition)
    {
        this.NodeOutputDefinition = nodeOutputDefinition;
        InitializeComponent();
        WaveProvider.ChunkCreated += ChunkCreated;
    }

    public NodeOutputDefinition NodeOutputDefinition { get; set; }

    private async void ChunkCreated(RenderingChunk chunk)
    {
        try
        {
            var response = await chunk.GetResponse(NodeOutputDefinition);

            Task.Run(() =>
            {
                if (response is SingleChannelAudioBuffer responseTyped)
                {
                    var (average, max) = MinMax.SingleAbsAvgMaxFast(responseTyped.AsSpan);
                    Dispatcher.BeginInvoke(() =>
                    {
                        var fill = MinMax.ValueToColor(average);
                        var stroke = MinMax.ValueToColor(max);
                        indicator.Fill = new SolidColorBrush(Color.FromRgb(fill.R, fill.G, fill.B));
                        indicator.Stroke = new SolidColorBrush(Color.FromRgb(stroke.R, stroke.G, stroke.B));
                    });
                }
            });
        }
        catch
        {
            //ignore
        }
    }
}
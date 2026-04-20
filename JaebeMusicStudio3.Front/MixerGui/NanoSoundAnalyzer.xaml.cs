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
    
    private void ChunkCreated(RenderingChunk chunk)
    {
        var response = chunk.GetResponse(NodeOutputDefinition).Result;
        Task.Run(() =>
        {
            if (response is SingleChannelAudioBuffer responseTyped)
            {
                var (average, max) = MinMax.SingleAbsAvgMaxFast(responseTyped.AsSpan);
                Dispatcher.BeginInvoke(() =>
                {
                    indicator.Fill=new SolidColorBrush(MinMax.ValueToColor(average));
                    indicator.Stroke=new SolidColorBrush(MinMax.ValueToColor(max));
                });
            }
        });
    }
}
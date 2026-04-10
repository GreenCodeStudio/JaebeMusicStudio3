using System.Windows.Controls;
using JaebeMusicStudio3.Core.AudioNodes;

namespace JaebeMusicStudio3.Front.AudioNodesGui;

public partial class VolumeNodeGui : UserControl
{
    public VolumeNodeGui(VolumeNode volumeNode)
    {
        InitializeComponent();
        VolumeSlider.Value = volumeNode.Volume;
        VolumeSlider.ValueChanged += (x) => { volumeNode.Volume = (float)x; };
    }
}
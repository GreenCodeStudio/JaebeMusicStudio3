using System.Windows.Controls;
using JaebeMusicStudio3.Core.AudioNodes;

namespace JaebeMusicStudio3.Front.AudioNodesGui;

public partial class OverdriveNodeGui : UserControl
{
    public OverdriveNodeGui(Overdrive overdriveNode)
    {
        InitializeComponent();
        this.VolumeValueSlider.Value = overdriveNode.Volume;
        VolumeValueSlider.ValueChanged += (x) => { overdriveNode.Volume = (float)x; };
        LimitSlider.Value = overdriveNode.Limit;
        LimitSlider.ValueChanged += (x) => { overdriveNode.Limit = (float)x; };
    }
}
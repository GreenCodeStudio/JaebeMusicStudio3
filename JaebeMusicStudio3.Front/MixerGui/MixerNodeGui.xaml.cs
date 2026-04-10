using System.Windows.Controls;
using JaebeMusicStudio3.Core.AudioNodes;
using JaebeMusicStudio3.Core.Mixer;

namespace JaebeMusicStudio3.Front.MixerGui;

public partial class MixerNodeGui : UserControl
{
    public MixerNodeGui(IAudioNode node)
    {
        InitializeComponent();
        Title.Content = node.ToString();
    }
}
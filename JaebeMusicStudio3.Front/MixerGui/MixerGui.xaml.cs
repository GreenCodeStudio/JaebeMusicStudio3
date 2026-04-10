using System.Windows.Controls;
using System.Windows.Shapes;
using JaebeMusicStudio3.Core.AudioNodes;
using JaebeMusicStudio3.Core.Mixer;

namespace JaebeMusicStudio3.Front.MixerGui;

public partial class MixerGui : UserControl
{
    public MixerGui(AudioMixer mixer)
    {
        this.Mixer = mixer;
        InitializeComponent();
        Render();
    }

    public AudioMixer Mixer { get; set; }

    private void Render()
    {
        Plane.Children.Clear();
        var map = new Dictionary<IAudioNode, UserControl>();
        var y = 10;
        foreach (var node in Mixer.Nodes)
        {
            var nodeGui = new MixerNodeGui(node);
            nodeGui.Width = 200;
            nodeGui.Height = 50;
            nodeGui.Margin = new System.Windows.Thickness(10, y, 0, 0);
            nodeGui.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            nodeGui.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            Plane.Children.Add(nodeGui);
            map.Add(node, nodeGui);
            y += 100;
            nodeGui.MouseDown += (s, e) =>
            {
                PropertiesWrapper.Children.Clear();
                var x = GuiFactory.Create(node);
                if (x != null)
                {
                    PropertiesWrapper.Children.Add(x);
                }
            };
        }

        foreach (var x in Mixer.Connections)
        {
            var lineStartNode = map[x.Key.Node];
            var lineEndNode = map[x.Value.Node];
            var line = new Line();
            line.Stroke = System.Windows.Media.Brushes.Black;
            line.X1 = lineStartNode.Margin.Left + lineStartNode.Width;
            line.Y1 = lineStartNode.Margin.Top + lineStartNode.Height / 2;
            line.X2 = lineEndNode.Margin.Left;
            line.Y2 = lineEndNode.Margin.Top + lineEndNode.Height / 2;
            Plane.Children.Add(line);
        }
    }
}
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
        mixer.Changed += () => { Dispatcher.Invoke(() => Render()); };
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

            var pos = Mixer.GetPosition(node);
            nodeGui.Margin = new System.Windows.Thickness(pos.X, pos.Y, 0, 0);
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
            nodeGui.ContextMenu = new ContextMenu();
            var zzzz = (new MenuItem()
            {
                Header = "Move"
            });
            zzzz.Click += (s, e) =>
            {
                var pos = Mixer.GetPosition(node);
                pos.X += 10;
                Mixer.SetPosition(node, pos);
            };
            nodeGui.ContextMenu.Items.Add(zzzz);

            // nodeGui.MouseDown += (sender, args) => { CaptureMouse(); };
            nodeGui.MouseMove += (sender, args) =>
            {
                if (args.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                {
                    var pos = Mixer.GetPosition(node);
                    pos.X += args.MouseDevice.GetPosition(Plane).X - nodeGui.Margin.Left - nodeGui.Width / 2;
                    pos.Y += args.MouseDevice.GetPosition(Plane).Y - nodeGui.Margin.Top - nodeGui.Height / 2;
                    Mixer.SetPosition(node, pos);
                }
            };
            // nodeGui.MouseUp += (sender, args) => { ReleaseMouseCapture(); };
            nodeGui.Connect += (input, output) =>
            {
                Mixer.Connect(output, input);
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
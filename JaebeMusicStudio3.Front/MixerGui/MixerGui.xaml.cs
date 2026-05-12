using System.Windows;
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
        this.AllowDrop = true;
        Drop += OnDrop;
        Render();
        mixer.Changed += () => { Dispatcher.Invoke(() => Render()); };
        var contextMenu = new ContextMenu();
        this.ContextMenu = contextMenu;
        var add = new MenuItem();
        add.Header = "Add";
        contextMenu.Items.Add(add);

        var mix = new MenuItem();
        mix.Header = "Mix";
        mix.Click += (s, e) =>
        {
            var node = new MixNode();
            Mixer.Add(node);
        };
        add.Items.Add(mix);

        var liveAudioInput = new MenuItem();
        liveAudioInput.Header = "LiveAudioInput";
        liveAudioInput.Click += (s, e) =>
        {
            var node = new LiveAudioInput();
            Mixer.Add(node);
        };
        add.Items.Add(liveAudioInput);
        var overdrive = new MenuItem();
        overdrive.Header = "Overdrive";
        overdrive.Click += (s, e) =>
        {
            var node = new OverdriveNode();
            Mixer.Add(node);
        };
        add.Items.Add(overdrive);

        this.MouseMove += (sender, args) =>
        {
            if (args.LeftButton == System.Windows.Input.MouseButtonState.Pressed && _movingNode != null)
            {
                var pos = Mixer.GetPosition(_movingNode);
                pos.X += args.MouseDevice.GetPosition(Plane).X - _movingPoint.Value.X;
                pos.Y += args.MouseDevice.GetPosition(Plane).Y - _movingPoint.Value.Y;
                _movingPoint = args.MouseDevice.GetPosition(Plane);
                Mixer.SetPosition(_movingNode, pos);
            }
        };
        this.MouseUp += (sender, args) =>
        {
            _movingNode = null;
            _movingPoint = null;
        };
    }

    public AudioMixer Mixer { get; set; }
    private IAudioNode _movingNode = null;
    private Point? _movingPoint = null;

    private void Render()
    {
        Plane.Children.Clear();
        var map = new Dictionary<IAudioNode, UserControl>();
        var y = 10;
        foreach (var node in Mixer.Nodes)
        {
            var nodeGui = new MixerNodeGui(node);
            nodeGui.Width = 200;
            nodeGui.Height = 100;

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

            nodeGui.MouseDown += (sender, args) =>
            {
                _movingNode = node;
                _movingPoint = args.MouseDevice.GetPosition(Plane);
            };

            // nodeGui.MouseUp += (sender, args) => { ReleaseMouseCapture(); };
            nodeGui.Connect += (input, output) => { Mixer.Connect(output, input); };
            nodeGui.DisconnectByOutput += (output) =>
            {
                foreach (var keyValuePair in Mixer.Connections.Where(x => x.Value == output))
                {
                    Mixer.Disconnect(keyValuePair);
                }
            };
            nodeGui.DisconnectByInput += (input) =>
            {
                foreach (var keyValuePair in Mixer.Connections.Where(x => x.Key == input))
                {
                    Mixer.Disconnect(keyValuePair);
                }
            };
        }

        foreach (var x in Mixer.Connections)
        {
            var lineStartNode = map[x.Key.Node];
            var lineEndNode = map[x.Value.Node];
            var line = new Line();
            var startIndex = x.Key.Node.Inputs.Select(n => n.Name).ToArray().IndexOf(x.Key.Name);
            var endIndex = x.Value.Node.Outputs.Select(n => n.Name).ToArray().IndexOf(x.Value.Name);
            line.Stroke = System.Windows.Media.Brushes.Black;
            line.X1 = lineStartNode.Margin.Left;
            line.Y1 = lineStartNode.Margin.Top + 40 + 30 * startIndex;
            line.X2 = lineEndNode.Margin.Left + lineEndNode.Width;
            line.Y2 = lineEndNode.Margin.Top + 40 + 30 * endIndex;
            Plane.Children.Add(line);
        }
    }   
    private void OnDrop(object sender, DragEventArgs e)
    {
        var fileNames = e.Data.GetData("FileDrop");
        if (fileNames != null)
        {
            foreach (var x in fileNames as string[])
            {
                Mixer.LoadVstFile(x);
            }
        }
    }
}
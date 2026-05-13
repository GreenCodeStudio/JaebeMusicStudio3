using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using JaebeMusicStudio3.Core.AudioNodes;
using JaebeMusicStudio3.Core.Mixer;

namespace JaebeMusicStudio3.Front.MixerGui;

public partial class InstrumentEditorGui : UserControl
{
    public InstrumentEditorGui(Instrument instrument)
    {
        this.Instrument = instrument;
        InitializeComponent();
        Render();
        instrument.Changed += () => { Dispatcher.Invoke(() => Render()); };
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
            Instrument.Add(node);
        };
        add.Items.Add(mix);

        var basicOscilator = new MenuItem();
        basicOscilator.Header = "Basic oscilator";
        basicOscilator.Click += (s, e) =>
        {
            var node = new BasicOscillatorNode();
            Instrument.Add(node);
        };
        add.Items.Add(basicOscilator);
        var overdrive = new MenuItem();
        overdrive.Header = "Overdrive";
        overdrive.Click += (s, e) =>
        {
            var node = new OverdriveNode();
            Instrument.Add(node);
        };
        add.Items.Add(overdrive);

        this.MouseMove += (sender, args) =>
        {
            if (args.LeftButton == System.Windows.Input.MouseButtonState.Pressed && _movingNode != null)
            {
                var pos = Instrument.GetPosition(_movingNode);
                pos.X += args.MouseDevice.GetPosition(Plane).X - _movingPoint.Value.X;
                pos.Y += args.MouseDevice.GetPosition(Plane).Y - _movingPoint.Value.Y;
                _movingPoint = args.MouseDevice.GetPosition(Plane);
                Instrument.SetPosition(_movingNode, pos);
            }
        };
        this.MouseUp += (sender, args) =>
        {
            _movingNode = null;
            _movingPoint = null;
        };
        MouseWheel += (sender, args) =>
        {
            var scaleValue = Math.Pow(2, args.Delta / 200.0);
            var mousePos = args.MouseDevice.GetPosition(PlaneWrapper);
            var translation = new TranslateTransform(-mousePos.X, -mousePos.Y);
            var translationReverse = new TranslateTransform(+mousePos.X, +mousePos.Y);
            var scale = new System.Windows.Media.ScaleTransform(scaleValue, scaleValue);
            Plane.RenderTransform = new MatrixTransform(Plane.RenderTransform.Value * translation.Value * scale.Value * translationReverse.Value);
        };
    }

    public Instrument Instrument { get; set; }
    private IAudioNode _movingNode = null;
    private Point? _movingPoint = null;
    private IAudioNode _selectedNode;

    private void Render()
    {
        Plane.Children.Clear();
        var map = new Dictionary<IAudioNode, UserControl>();
        var y = 10;
        foreach (var node in Instrument.Nodes)
        {
            var nodeGui = new MixerNodeGui(node);
            nodeGui.Width = 200;
            nodeGui.Height = 100;

            var pos = Instrument.GetPosition(node);
            nodeGui.Margin = new System.Windows.Thickness(pos.X, pos.Y, 0, 0);
            if (node == _selectedNode)
            {
                nodeGui.BorderBrush = System.Windows.Media.Brushes.Red;
                nodeGui.BorderThickness = new System.Windows.Thickness(2);
                nodeGui.Margin = new System.Windows.Thickness(pos.X - 2, pos.Y - 2, 0, 0);
                nodeGui.Width += 4;
                nodeGui.Height += 4;
            }

            nodeGui.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            nodeGui.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            Plane.Children.Add(nodeGui);
            map.Add(node, nodeGui);
            y += 100;
            nodeGui.MouseDown += (s, e) =>
            {
                _selectedNode = node;
                PropertiesWrapper.Children.Clear();
                var x = GuiFactory.Create(node);
                if (x != null)
                {
                    PropertiesWrapper.Children.Add(x);
                }

                Render();
            };
            nodeGui.ContextMenu = new ContextMenu();

            nodeGui.MouseDown += (sender, args) =>
            {
                _movingNode = node;
                _movingPoint = args.MouseDevice.GetPosition(Plane);
            };

            nodeGui.MouseUp += (sender, args) =>
            {
                _movingNode = null;
                _movingPoint = null;
            };
            nodeGui.Connect += (input, output) => { Instrument.Connect(output, input); };
            nodeGui.DisconnectByOutput += (output) =>
            {
                foreach (var keyValuePair in Instrument.Connections.Where(x => x.Value == output))
                {
                    Instrument.Disconnect(keyValuePair);
                }
            };
            nodeGui.DisconnectByInput += (input) =>
            {
                foreach (var keyValuePair in Instrument.Connections.Where(x => x.Key == input))
                {
                    Instrument.Disconnect(keyValuePair);
                }
            };
        }

        foreach (var x in Instrument.Connections)
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
}